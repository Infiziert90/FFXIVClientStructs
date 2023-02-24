using FFXIVClientStructs.SourceGenerators.Models.CSharp;
using FFXIVClientStructs.SourceGenerators.Models.Generators;
using LanguageExt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FFXIVClientStructs.SourceGenerators.Models;

internal sealed record GenerationMethodInfo(
    Seq<MemberFunctionInfo> MemberFunctions,
    Seq<VirtualFunctionInfo> VirtualFunctions,
    Seq<StaticAddressInfo> StaticAddresses,
    Seq<MethodInfo> RemainingMethods,
    Seq<CStrOverloadInfo> CStrOverloads)
{
    public static Validation<DiagnosticInfo, GenerationMethodInfo> FromRoslyn(
        StructDeclarationSyntax structSyntax, SemanticModel model, CancellationToken token)
    {
        // look for methods that require generation
        IEnumerable<MethodDeclarationSyntax> methodSyntaxes = structSyntax.Members.OfType<MethodDeclarationSyntax>();

        Seq<Validation<DiagnosticInfo, MemberFunctionInfo>> memberFunctions =
            Seq<Validation<DiagnosticInfo, MemberFunctionInfo>>.Empty;
        Seq<Validation<DiagnosticInfo, VirtualFunctionInfo>> virtualFunctions =
            Seq<Validation<DiagnosticInfo, VirtualFunctionInfo>>.Empty;
        Seq<Validation<DiagnosticInfo, StaticAddressInfo>> staticAddresses =
            Seq<Validation<DiagnosticInfo, StaticAddressInfo>>.Empty;
        Seq<Validation<DiagnosticInfo, MethodInfo>> remainingMethods =
            Seq<Validation<DiagnosticInfo, MethodInfo>>.Empty;
        Seq<Validation<DiagnosticInfo, CStrOverloadInfo>> cStrOverloads =
            Seq<Validation<DiagnosticInfo, CStrOverloadInfo>>.Empty;

        foreach (MethodDeclarationSyntax methodSyntax in methodSyntaxes)
        {
            if (!methodSyntax.AttributeLists.Any())
                continue;

            IMethodSymbol? methodSymbol = model.GetDeclaredSymbol(methodSyntax, token);

            if (methodSymbol is null)
                continue;

            Validation<DiagnosticInfo, MethodInfo> methodInfo = MethodInfo.FromRoslyn(methodSyntax, methodSymbol);

            if (MemberFunctionInfo.IsValidTarget(methodSymbol))
                memberFunctions = memberFunctions.Add(MemberFunctionInfo.FromRoslyn(methodInfo, methodSymbol));
            else if (VirtualFunctionInfo.IsValidTarget(methodSymbol))
                virtualFunctions = virtualFunctions.Add(VirtualFunctionInfo.FromRoslyn(methodInfo, methodSymbol));
            else if (StaticAddressInfo.IsValidTarget(methodSymbol))
                staticAddresses = staticAddresses.Add(StaticAddressInfo.FromRoslyn(methodInfo, methodSymbol));
            else
                remainingMethods = remainingMethods.Add(methodInfo);

            if (CStrOverloadInfo.IsValidTarget(methodSymbol))
                cStrOverloads = cStrOverloads.Add(CStrOverloadInfo.FromRoslyn(methodInfo, methodSymbol));

            token.ThrowIfCancellationRequested();
        }

        return (memberFunctions.Sequence(), virtualFunctions.Sequence(),
                staticAddresses.Sequence(), remainingMethods.Sequence(), cStrOverloads.Sequence())
            .Apply(static (memberFunctionInfos, virtualFunctionInfos, staticAddressInfos, remainingMethodInfos,
                    cStrOverloadInfos) =>
                new GenerationMethodInfo(
                    memberFunctionInfos,
                    virtualFunctionInfos,
                    staticAddressInfos,
                    remainingMethodInfos,
                    cStrOverloadInfos)
            );
    }

    public bool RequiresGeneration()
    {
        return MemberFunctions.Any() || VirtualFunctions.Any() || StaticAddresses.Any() || CStrOverloads.Any();
    }
}