using FFXIVClientStructs.SourceGenerators.Extensions;
using FFXIVClientStructs.SourceGenerators.Models.CSharp;
using LanguageExt;
using Microsoft.CodeAnalysis;
using static LanguageExt.Prelude;
using static FFXIVClientStructs.SourceGenerators.DiagnosticDescriptors;

namespace FFXIVClientStructs.SourceGenerators.Models.Generators;

internal sealed record MemberFunctionInfo(MethodInfo MethodInfo, SignatureInfo SignatureInfo)
{
    private const string AttributeName = "FFXIVClientStructs.Interop.Attributes.MemberFunctionAttribute";

    public static Validation<DiagnosticInfo, MemberFunctionInfo> FromRoslyn(
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

        Validation<DiagnosticInfo, SignatureInfo> validSignature =
            methodSymbol.GetFirstAttributeDataByTypeName(AttributeName)
                .GetValidAttributeArgument<string>("Signature", 0, AttributeName, methodSymbol)
                .Bind(signatureString => SignatureInfo.GetValidatedSignature(signatureString, methodSymbol));

        return (validMethodInfo, validSignature).Apply(static (methodInfo, signature) =>
            new MemberFunctionInfo(methodInfo, signature));
    }

    public static bool IsValidTarget(IMethodSymbol methodSymbol)
    {
        return methodSymbol.GetFirstAttributeDataByTypeName(AttributeName).IsSome;
    }
}