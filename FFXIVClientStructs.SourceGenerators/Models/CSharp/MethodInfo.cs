using FFXIVClientStructs.SourceGenerators.Extensions;
using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static FFXIVClientStructs.SourceGenerators.DiagnosticDescriptors;
using static LanguageExt.Prelude;

namespace FFXIVClientStructs.SourceGenerators.Models.CSharp;

internal sealed record MethodInfo(string Name, string Modifiers, string ReturnType, bool isPartial, bool IsStatic,
    Seq<ParameterInfo> Parameters)
{
    public static Validation<DiagnosticInfo, MethodInfo> FromRoslyn(MethodDeclarationSyntax methodSyntax,
        IMethodSymbol methodSymbol)
    {
        Validation<DiagnosticInfo, IMethodSymbol> validSymbol =
            methodSymbol.ReturnType.IsUnmanagedType
                ? Success<DiagnosticInfo, IMethodSymbol>(methodSymbol)
                : Fail<DiagnosticInfo, IMethodSymbol>(DiagnosticInfo.Create(
                    MethodUsesForbiddenType,
                    methodSyntax,
                    methodSymbol.Name,
                    methodSymbol.ReturnType.GetFullyQualifiedNameWithGenerics()
                ));
        Validation<DiagnosticInfo, IEnumerable<ParameterInfo>> paramInfos =
            methodSymbol.Parameters.Select(ParameterInfo.FromRoslyn).Sequence();

        return (validSymbol, paramInfos).Apply((symbol, pInfos) =>
            new MethodInfo(
                symbol.Name,
                methodSyntax.Modifiers.ToString(),
                symbol.ReturnType.GetFullyQualifiedNameWithGenerics(),
                methodSyntax.HasModifier(SyntaxKind.PartialKeyword),
                symbol.IsStatic,
                pInfos.ToSeq()
            ));
    }
}