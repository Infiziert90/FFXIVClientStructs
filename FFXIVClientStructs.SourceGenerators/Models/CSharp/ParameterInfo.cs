using FFXIVClientStructs.SourceGenerators.Extensions;
using LanguageExt;
using Microsoft.CodeAnalysis;
using static FFXIVClientStructs.SourceGenerators.DiagnosticDescriptors;
using static LanguageExt.Prelude;

namespace FFXIVClientStructs.SourceGenerators.Models.CSharp;

internal sealed record ParameterInfo(string Name, string Type, Option<string> DefaultValue)
{
    public static Validation<DiagnosticInfo, ParameterInfo> FromRoslyn(IParameterSymbol parameterSymbol)
    {
        return parameterSymbol.Type.IsUnmanagedType
            ? Success<DiagnosticInfo, ParameterInfo>(
                new ParameterInfo(
                    parameterSymbol.Name,
                    parameterSymbol.Type.GetFullyQualifiedNameWithGenerics(),
                    parameterSymbol.GetDefaultValueString()
                ))
            : Fail<DiagnosticInfo, ParameterInfo>(
                DiagnosticInfo.Create(
                    MethodUsesForbiddenType,
                    parameterSymbol,
                    parameterSymbol.ContainingSymbol.Name,
                    parameterSymbol.Type.GetFullyQualifiedNameWithGenerics()
                ));
    }
}