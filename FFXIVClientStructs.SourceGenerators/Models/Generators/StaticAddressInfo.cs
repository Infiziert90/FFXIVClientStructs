using FFXIVClientStructs.SourceGenerators.Extensions;
using FFXIVClientStructs.SourceGenerators.Models.CSharp;
using LanguageExt;
using Microsoft.CodeAnalysis;
using static LanguageExt.Prelude;
using static FFXIVClientStructs.SourceGenerators.DiagnosticDescriptors;

namespace FFXIVClientStructs.SourceGenerators.Models.Generators;

internal sealed record StaticAddressInfo(MethodInfo MethodInfo, SignatureInfo SignatureInfo, int Offset,
    bool IsPointer)
{
    private const string AttributeName = "FFXIVClientStructs.Interop.Attributes.StaticAddressAttribute";

    public static Validation<DiagnosticInfo, StaticAddressInfo> FromRoslyn(
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

        Option<AttributeData> staticAddressAttribute = methodSymbol.GetFirstAttributeDataByTypeName(AttributeName);

        Validation<DiagnosticInfo, SignatureInfo> validSignature =
            staticAddressAttribute
                .GetValidAttributeArgument<string>("Signature", 0, AttributeName, methodSymbol)
                .Bind(signatureString => SignatureInfo.GetValidatedSignature(signatureString, methodSymbol));
        Validation<DiagnosticInfo, int> validOffset =
            staticAddressAttribute.GetValidAttributeArgument<int>("Offset", 1, AttributeName, methodSymbol);
        Validation<DiagnosticInfo, bool> validIsPointer =
            staticAddressAttribute.GetValidAttributeArgument<bool>("IsPointer", 2, AttributeName, methodSymbol);

        return (validMethodInfo, validSignature, validOffset, validIsPointer).Apply(
            static (methodInfo, signature, offset, isPointer) =>
                new StaticAddressInfo(methodInfo, signature, offset, isPointer));
    }

    public static bool IsValidTarget(IMethodSymbol methodSymbol)
    {
        return methodSymbol.GetFirstAttributeDataByTypeName(AttributeName).IsSome;
    }
}