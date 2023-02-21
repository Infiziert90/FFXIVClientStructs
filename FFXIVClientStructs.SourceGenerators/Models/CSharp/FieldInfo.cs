using FFXIVClientStructs.SourceGenerators.Extensions;
using LanguageExt;
using Microsoft.CodeAnalysis;

namespace FFXIVClientStructs.SourceGenerators.Models.CSharp;

internal sealed record FieldInfo(string Name, string TypeName, int Offset)
{
    private const string FieldOffsetAttributeName = "System.Runtime.InteropServices.FieldOffsetAttribute";

    public static Validation<DiagnosticInfo, FieldInfo> FromRoslyn(IFieldSymbol fieldSymbol)
    {
        Validation<DiagnosticInfo, int> validOffset = 
            fieldSymbol
                .GetFirstAttributeDataByTypeName(FieldOffsetAttributeName)
                .GetValidAttributeArgument<int>("Value", 0, FieldOffsetAttributeName, fieldSymbol);

        return validOffset.Map(offset =>
            new FieldInfo(
                fieldSymbol.Name,
                fieldSymbol.Type.GetFullyQualifiedNameWithGenerics(),
                offset
                )
        );
    }
}