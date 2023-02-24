using FFXIVClientStructs.SourceGenerators.Models.CSharp;
using FFXIVClientStructs.SourceGenerators.Models.Generators;
using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static LanguageExt.Prelude;

namespace FFXIVClientStructs.SourceGenerators.Models;

internal sealed record GenerationFieldInfo(Option<Seq<ExplicitFieldInfo>> ExplicitFields)
{
    public static Validation<DiagnosticInfo, GenerationFieldInfo> FromRoslyn(
        StructDeclarationSyntax structSyntax, bool hasExplicitFields, SemanticModel model, CancellationToken token)
    {
        if (hasExplicitFields)
            return GetFieldInfos(structSyntax, model, token)
                .Bind<GenerationFieldInfo>(fields => new GenerationFieldInfo(fields));
        else
            return Success<DiagnosticInfo, GenerationFieldInfo>(new GenerationFieldInfo(None));
    }
    
    private static Validation<DiagnosticInfo, Option<Seq<ExplicitFieldInfo>>> GetFieldInfos(StructDeclarationSyntax structSyntax,
        SemanticModel model, CancellationToken token)
    {
        Seq<Validation<DiagnosticInfo, ExplicitFieldInfo>> fieldInfos = LanguageExt.Seq<Validation<DiagnosticInfo, ExplicitFieldInfo>>.Empty;
        
        IEnumerable<VariableDeclaratorSyntax> variableSyntaxes = 
            structSyntax.DescendantNodes(node => node.IsEquivalentTo(structSyntax) || node is FieldDeclarationSyntax or VariableDeclaratorSyntax or VariableDeclarationSyntax)
                .OfType<VariableDeclaratorSyntax>();

        foreach (VariableDeclaratorSyntax variableSyntax in variableSyntaxes)
        {
            if (model.GetDeclaredSymbol(variableSyntax, token) is not IFieldSymbol fieldSymbol)
                continue;

            if (fieldSymbol.IsConst || fieldSymbol.IsStatic)
                continue;

            fieldInfos = fieldInfos.Add(ExplicitFieldInfo.FromRoslyn(fieldSymbol));
            
            token.ThrowIfCancellationRequested();
        }

        return fieldInfos.Sequence().Map(static fieldInfos => Some(fieldInfos));
    }
}