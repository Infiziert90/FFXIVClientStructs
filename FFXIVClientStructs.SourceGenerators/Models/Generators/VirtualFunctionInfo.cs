using FFXIVClientStructs.SourceGenerators.Extensions;
using FFXIVClientStructs.SourceGenerators.Models.CSharp;
using LanguageExt;
using Microsoft.CodeAnalysis;
using static LanguageExt.Prelude;
using static FFXIVClientStructs.SourceGenerators.DiagnosticDescriptors;

namespace FFXIVClientStructs.SourceGenerators.Models.Generators;

internal sealed record VirtualFunctionInfo(MethodInfo MethodInfo, uint Index)
{
    private const string AttributeName = "FFXIVClientStructs.Interop.Attributes.VirtualFunctionAttribute";

    public static Validation<DiagnosticInfo, VirtualFunctionInfo> FromRoslyn(
        Validation<DiagnosticInfo, MethodInfo> methodInfo, IMethodSymbol methodSymbol)
    {
        Validation<DiagnosticInfo, MethodInfo> validMethodInfo =
            methodInfo.Bind(
                mInfo =>
                    mInfo.isPartial
                        ? Success<DiagnosticInfo, MethodInfo>(mInfo)
                        : Fail<DiagnosticInfo, MethodInfo>(
                            DiagnosticInfo.Create(
                                MethodMustBePartial,
                                methodSymbol,
                                methodSymbol.Name
                            ))
            );

        Validation<DiagnosticInfo, uint> validIndex =
            methodSymbol.GetFirstAttributeDataByTypeName(AttributeName)
                .GetValidAttributeArgument<uint>("Index", 0, AttributeName, methodSymbol);

        return (validMethodInfo, validIndex).Apply(static (methodInfo, index) =>
            new VirtualFunctionInfo(methodInfo, index));
    }

    public static bool IsValidTarget(IMethodSymbol methodSymbol)
    {
        return methodSymbol.GetFirstAttributeDataByTypeName(AttributeName).IsSome;
    }
}