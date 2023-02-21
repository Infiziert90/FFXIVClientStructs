using System.Runtime.InteropServices;
using FFXIVClientStructs.SourceGenerators.Extensions;
using FFXIVClientStructs.SourceGenerators.Models.CSharp;
using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static LanguageExt.Prelude;

namespace FFXIVClientStructs.SourceGenerators.Models;

internal sealed record GenerationTargetInfo(
    StructInfo StructInfo,
    Option<Seq<FieldInfo>> FieldInfos,
    MethodsInfo MethodsInfo)
{
    public static Validation<DiagnosticInfo, GenerationTargetInfo> FromRoslyn(
        StructDeclarationSyntax structSyntax, INamedTypeSymbol structSymbol, SemanticModel model,
        CancellationToken token)
    {
        Validation<DiagnosticInfo, MethodsInfo> methodsInfo = MethodsInfo.FromRoslyn(structSyntax, model, token);

        token.ThrowIfCancellationRequested();

        Validation<DiagnosticInfo, Option<Seq<FieldInfo>>> fieldInfos = GetFieldInfos(structSyntax, structSymbol, model, token);

        token.ThrowIfCancellationRequested();

        bool requiresGeneration = methodsInfo.BiExists(Fail: _ => true, Success: mInfo => mInfo.RequiresGeneration());

        Validation<DiagnosticInfo, StructInfo> structInfo =
            StructInfo.FromRoslyn(structSyntax, structSymbol, requiresGeneration);

        token.ThrowIfCancellationRequested();

        return (structInfo, fieldInfos, methodsInfo)
            .Apply(static (structInfo, fieldInfos, methodsInfo) =>
                new GenerationTargetInfo(
                    structInfo,
                    fieldInfos,
                    methodsInfo)
            );
    }

    private static Validation<DiagnosticInfo, Option<Seq<FieldInfo>>> GetFieldInfos(StructDeclarationSyntax structSyntax,
        INamedTypeSymbol structSymbol, SemanticModel model, CancellationToken token)
    {
        // only collect fields for explicit layout structs; any others can't be inherited anyway
        if (!structSyntax.AttributeLists.Any())
            return Success<DiagnosticInfo, Option<Seq<FieldInfo>>>(None);

        Option<AttributeData> layoutAttribute =
            structSymbol.GetFirstAttributeDataByTypeName("System.Runtime.InteropServices.StructLayoutAttribute");
        
        if (layoutAttribute.IsNone)
            return Success<DiagnosticInfo, Option<Seq<FieldInfo>>>(None);

        Validation<DiagnosticInfo, LayoutKind> layoutKind = 
            layoutAttribute.GetValidAttributeArgument<LayoutKind>("Value", 0, string.Empty, structSymbol);
        
        if (!layoutKind.Exists(static layoutKind => layoutKind == LayoutKind.Explicit))
            return Success<DiagnosticInfo, Option<Seq<FieldInfo>>>(None);

        Seq<Validation<DiagnosticInfo, FieldInfo>> fieldInfos = LanguageExt.Seq<Validation<DiagnosticInfo, FieldInfo>>.Empty;
        
        IEnumerable<VariableDeclaratorSyntax> variableSyntaxes = 
            structSyntax.DescendantNodes(node => node.IsEquivalentTo(structSyntax) || node is FieldDeclarationSyntax or VariableDeclaratorSyntax or VariableDeclarationSyntax)
                .OfType<VariableDeclaratorSyntax>();

        foreach (VariableDeclaratorSyntax variableSyntax in variableSyntaxes)
        {
            if (model.GetDeclaredSymbol(variableSyntax, token) is not IFieldSymbol fieldSymbol)
                continue;

            if (fieldSymbol.IsConst || fieldSymbol.IsStatic)
                continue;

            fieldInfos = fieldInfos.Add(FieldInfo.FromRoslyn(fieldSymbol));
            
            token.ThrowIfCancellationRequested();
        }

        return fieldInfos.Sequence().Map(static fieldInfos => Some(fieldInfos));
    }

    public bool RequiresGeneration()
    {
        return MethodsInfo.RequiresGeneration();
    }

    public string GetFilename()
    {
        return "";
    }
}