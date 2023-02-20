using FFXIVClientStructs.SourceGenerators.Extensions;
using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static FFXIVClientStructs.SourceGenerators.DiagnosticDescriptors;
using static LanguageExt.Prelude;

namespace FFXIVClientStructs.SourceGenerators.Models.CSharp;

internal sealed record StructInfo(string Name, string FullyQualifiedTypeName, string Namespace, Seq<string> Hierarchy)
{
    public static Validation<DiagnosticInfo, StructInfo> FromRoslyn(StructDeclarationSyntax structSyntax,
        INamedTypeSymbol structSymbol, bool requiresGeneration)
    {
        Validation<DiagnosticInfo, StructDeclarationSyntax> validSyntax =
            !requiresGeneration || structSyntax.HasModifier(SyntaxKind.PartialKeyword)
                ? Success<DiagnosticInfo, StructDeclarationSyntax>(structSyntax)
                : Fail<DiagnosticInfo, StructDeclarationSyntax>(
                    DiagnosticInfo.Create(
                        StructMustBePartial,
                        structSyntax,
                        structSyntax.Identifier.ToString()
                    ));
        Validation<DiagnosticInfo, Seq<string>> validHierarchy = GetHierarchy(structSyntax);

        return (validSyntax, validHierarchy).Apply((syntax, hierarchy) =>
            new StructInfo(
                syntax.GetNameWithTypeDeclarationList(),
                structSymbol.GetFullyQualifiedNameWithGenerics(),
                syntax.GetContainingFileScopedNamespace(),
                hierarchy.Reverse().ToSeq()));
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
}