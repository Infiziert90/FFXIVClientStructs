using FFXIVClientStructs.SourceGenerators.Extensions;
using FFXIVClientStructs.SourceGenerators.Models.CSharp;
using LanguageExt;
using Microsoft.CodeAnalysis;

namespace FFXIVClientStructs.SourceGenerators.Models.Generators;

internal sealed record CStrOverloadInfo(MethodInfo MethodInfo, Option<string> IgnoreArgument)
{
    private const string AttributeName = "FFXIVClientStructs.Interop.Attributes.GenerateCStrOverloadsAttribute";

    public static Validation<DiagnosticInfo, CStrOverloadInfo> FromRoslyn(
        Validation<DiagnosticInfo, MethodInfo> methodInfo, IMethodSymbol methodSymbol)
    {
        Option<string> optionIgnoreArgument =
            methodSymbol.GetFirstAttributeDataByTypeName(AttributeName)
                .GetValidAttributeArgument<string>("IgnoreArgument", 0, AttributeName, methodSymbol)
                .ToOption();

        return methodInfo.Bind<CStrOverloadInfo>(mInfo =>
            new CStrOverloadInfo(mInfo, optionIgnoreArgument));
    }

    public static bool IsValidTarget(IMethodSymbol methodSymbol)
    {
        return methodSymbol.GetFirstAttributeDataByTypeName(AttributeName).IsSome;
    }
}