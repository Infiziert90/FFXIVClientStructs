using System.Text.Json;
using FFXIVClientStructs.SourceGenerators.Extensions;
using FFXIVClientStructs.SourceGenerators.Models;
using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FFXIVClientStructs.SourceGenerators;

[Generator]
internal sealed class InteropGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<Validation<DiagnosticInfo, GenerationTargetInfo>> generationTargetInfos =
            context.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) =>
                {
                    if (node is not StructDeclarationSyntax structSyntax)
                        return false;

                    string containingNamespace = structSyntax.GetContainingFileScopedNamespace();

                    return IsValidGenerationNamespace(containingNamespace);
                },
                static (context, token) =>
                {
                    StructDeclarationSyntax structSyntax = (StructDeclarationSyntax)context.Node;

                    if (context.SemanticModel.GetDeclaredSymbol(structSyntax) is not INamedTypeSymbol structSymbol)
                        return default;

                    return GenerationTargetInfo.FromRoslyn(structSyntax, structSymbol, context.SemanticModel, token);
                });

        context.RegisterSourceOutput(generationTargetInfos, static (sourceContext, item) =>
        {
            item.Match(
                Fail: diagnosticInfos =>
                {
                    diagnosticInfos.Iter(dInfo => sourceContext.ReportDiagnostic(dInfo.ToDiagnostic()));
                },
                Succ: generationTargetInfo =>
                {
                    if (generationTargetInfo.RequiresGeneration())
                        sourceContext.AddSource(
                            generationTargetInfo.StructInfo.Namespace + "." + generationTargetInfo.StructInfo.Name +
                            ".g.cs",
                            $"""
                                /*
                                {JsonSerializer.Serialize(generationTargetInfo, new JsonSerializerOptions { WriteIndented = true })}
                                 */
                                """);
                });
        });
    }

    private static bool IsValidGenerationNamespace(string containingNamespace)
    {
        return containingNamespace.StartsWith("FFXIVClientStructs.FFXIV") ||
               containingNamespace.StartsWith("FFXIVClientStructs.Havok");
    }
}