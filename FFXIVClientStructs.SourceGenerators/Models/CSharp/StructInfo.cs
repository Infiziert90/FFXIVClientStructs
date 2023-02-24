using System.Runtime.InteropServices;
using FFXIVClientStructs.SourceGenerators.Extensions;
using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static FFXIVClientStructs.SourceGenerators.DiagnosticDescriptors;
using static LanguageExt.Prelude;

namespace FFXIVClientStructs.SourceGenerators.Models.CSharp;

internal sealed record StructInfo(string Name, string FullyQualifiedTypeName, string Namespace, Seq<string> Hierarchy, Option<int> Size)
{
    public static Validation<DiagnosticInfo, StructInfo> FromRoslyn(StructDeclarationSyntax structSyntax,
        INamedTypeSymbol structSymbol)
    {
        Validation<DiagnosticInfo, Seq<string>> validHierarchy = GetHierarchy(structSyntax);

        return validHierarchy.Map(hierarchy =>
            new StructInfo(
                structSyntax.GetNameWithTypeDeclarationList(),
                structSymbol.GetFullyQualifiedNameWithGenerics(),
                structSyntax.GetContainingFileScopedNamespace(),
                hierarchy.Reverse().ToSeq(),
                GetStructSizeIfDefined(structSyntax, structSymbol)));
    }

    private static Validation<DiagnosticInfo, Seq<string>> GetHierarchy(StructDeclarationSyntax structSyntax)
    {
        Seq<string> hierarchy = new();

        TypeDeclarationSyntax? potentialContainingStruct = structSyntax.Parent as TypeDeclarationSyntax;

        while (potentialContainingStruct != null)
            if (potentialContainingStruct is StructDeclarationSyntax containingStructSyntax)
            {
                hierarchy = hierarchy.Add(containingStructSyntax.GetNameWithTypeDeclarationList());
                potentialContainingStruct = potentialContainingStruct.Parent as TypeDeclarationSyntax;
            }
            else
            {
                return Fail<DiagnosticInfo, Seq<string>>(
                    DiagnosticInfo.Create(
                        NestedStructMustBeContainedInStructs,
                        structSyntax,
                        structSyntax.Identifier.ToString()
                    ));
            }

        return Success<DiagnosticInfo, Seq<string>>(hierarchy);
    }
    
    private static Option<int> GetStructSizeIfDefined(StructDeclarationSyntax structSyntax,
        INamedTypeSymbol structSymbol)
    {
        if (!structSyntax.AttributeLists.Any())
            return None;

        Option<AttributeData> layoutAttribute =
            structSymbol.GetFirstAttributeDataByTypeName("System.Runtime.InteropServices.StructLayoutAttribute");

        if (layoutAttribute.IsNone)
            return None;

        if (!layoutAttribute.Bind(data => data.GetAttributeArgument<LayoutKind>("Value", 0))
                .Exists(static layoutKind => layoutKind == LayoutKind.Explicit))
            return None;

        return layoutAttribute.Bind(data => data.GetAttributeArgument<int>("Size", -1));
    }
}