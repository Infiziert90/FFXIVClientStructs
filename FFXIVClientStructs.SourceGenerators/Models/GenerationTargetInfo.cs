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
    GenerationFieldInfo GenerationFieldInfo,
    GenerationMethodInfo GenerationMethodInfo)
{
    public static Validation<DiagnosticInfo, GenerationTargetInfo> FromRoslyn(
        StructDeclarationSyntax structSyntax, INamedTypeSymbol structSymbol, SemanticModel model,
        CancellationToken token)
    {
        Validation<DiagnosticInfo, StructInfo> structInfo =
            StructInfo.FromRoslyn(structSyntax, structSymbol);
        
        token.ThrowIfCancellationRequested();
        
        Validation<DiagnosticInfo, GenerationMethodInfo> methodInfo = 
            GenerationMethodInfo.FromRoslyn(structSyntax, model, token);

        token.ThrowIfCancellationRequested();

        Validation<DiagnosticInfo, GenerationFieldInfo> fieldInfo =
            GenerationFieldInfo.FromRoslyn(structSyntax, structInfo.Exists(static structInfo => structInfo.Size.IsSome), model, token);

        token.ThrowIfCancellationRequested();

        return (structInfo, fieldInfo, methodInfo)
            .Apply(static (structInfo, fieldInfo, methodInfo) =>
                new GenerationTargetInfo(
                    structInfo,
                    fieldInfo,
                    methodInfo)
            );
    }

    public bool RequiresGeneration()
    {
        return GenerationMethodInfo.RequiresGeneration();
    }

    public string GetFilename()
    {
        return "";
    }
}