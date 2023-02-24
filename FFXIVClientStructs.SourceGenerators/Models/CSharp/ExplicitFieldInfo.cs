using FFXIVClientStructs.SourceGenerators.Extensions;
using LanguageExt;
using Microsoft.CodeAnalysis;

namespace FFXIVClientStructs.SourceGenerators.Models.CSharp;

internal sealed record ExplicitFieldInfo(string Name, string TypeName, int Offset)
{
    private const string FieldOffsetAttributeName = "System.Runtime.InteropServices.FieldOffsetAttribute";

    public static Validation<DiagnosticInfo, ExplicitFieldInfo> FromRoslyn(IFieldSymbol fieldSymbol)
    {
        Validation<DiagnosticInfo, int> validOffset = 
            fieldSymbol
                .GetFirstAttributeDataByTypeName(FieldOffsetAttributeName)
                .GetValidAttributeArgument<int>("Value", 0, FieldOffsetAttributeName, fieldSymbol);

        return validOffset.Map(offset =>
            new ExplicitFieldInfo(
                fieldSymbol.Name,
                fieldSymbol.Type.GetFullyQualifiedNameWithGenerics(),
                offset
                )
        );
    }
}