using LanguageExt;
using Microsoft.CodeAnalysis;
using static FFXIVClientStructs.SourceGenerators.DiagnosticDescriptors;
using static LanguageExt.Prelude;

namespace FFXIVClientStructs.SourceGenerators.Models;

internal sealed record SignatureInfo(string Signature)
{
    public static Validation<DiagnosticInfo, SignatureInfo> GetValidatedSignature(string signature,
        IMethodSymbol location)
    {
        return IsValid(signature)
            ? Success<DiagnosticInfo, SignatureInfo>(new SignatureInfo(PadSignature(signature)))
            : Fail<DiagnosticInfo, SignatureInfo>(
                DiagnosticInfo.Create(
                    InvalidSignature,
                    location,
                    signature
                ));
    }

    private static bool IsValid(string signature)
    {
        return signature.Split(' ').All(subString => subString.Length == 2);
    }

    private static string PadSignature(string signature)
    {
        int paddingNeeded = 8 - (signature.Length / 3 + 1) % 8;
        if (paddingNeeded == 0)
            return signature;

        Span<char> result = new(new char[signature.Length + paddingNeeded * 3]);
        signature.AsSpan().CopyTo(result);

        ReadOnlySpan<char> repeated = " ??".AsSpan();

        for (int repeatIndex = 0; repeatIndex < paddingNeeded; repeatIndex++)
            repeated.CopyTo(result.Slice(signature.Length + repeatIndex * repeated.Length));

        return result.ToString();
    }
}