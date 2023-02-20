using FFXIVClientStructs.SourceGenerators.Models.CSharp;
using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FFXIVClientStructs.SourceGenerators.Models;

internal sealed record GenerationTargetInfo(
    StructInfo StructInfo,
    MethodsInfo MethodsInfo)
{
    public static Validation<DiagnosticInfo, GenerationTargetInfo> FromRoslyn(
        StructDeclarationSyntax structSyntax, INamedTypeSymbol structSymbol, SemanticModel model,
        CancellationToken token)
    {
        Validation<DiagnosticInfo, MethodsInfo> methodsInfo = MethodsInfo.FromRoslyn(structSyntax, model, token);

        bool requiresGeneration = methodsInfo.BiExists(Fail: _ => true, Success: mInfo => mInfo.RequiresGeneration());

        Validation<DiagnosticInfo, StructInfo> structInfo =
            StructInfo.FromRoslyn(structSyntax, structSymbol, requiresGeneration);

        token.ThrowIfCancellationRequested();

        return (structInfo, methodsInfo)
            .Apply(static (structInfo, methodsInfo) =>
                new GenerationTargetInfo(
                    structInfo,
                    methodsInfo)
            );
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