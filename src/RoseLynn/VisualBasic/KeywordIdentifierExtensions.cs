using Microsoft.CodeAnalysis;

namespace RoseLynn.VisualBasic;

/// <summary>Contains helper methods for Visual Basic keyword identifiers.</summary>
public static class KeywordIdentifierExtensions
{
    /// <summary>Gets the Visual Basic keyword identifier for the provided predefined special type.</summary>
    /// <param name="specialType">The special type whose keyword identifier to get.</param>
    /// <returns>The keyword identifier of the provided special type, if supported; otherwise, <see langword="null"/>.</returns>
    public static string? GetKeywordIdentifierForPredefinedType(this SpecialType specialType)
    {
        return specialType switch
        {
            SpecialType.System_Boolean => "Boolean",
            SpecialType.System_Byte => "Byte",
            SpecialType.System_Char => "Char",
            SpecialType.System_DateTime => "Date",
            SpecialType.System_Decimal => "Decimal",
            SpecialType.System_Double => "Double",
            SpecialType.System_Int16 => "Short",
            SpecialType.System_Int32 => "Integer",
            SpecialType.System_Int64 => "Long",
            SpecialType.System_Object => "Object",
            SpecialType.System_SByte => "SByte",
            SpecialType.System_Single => "Single",
            SpecialType.System_String => "String",
            SpecialType.System_UInt16 => "UShort",
            SpecialType.System_UInt32 => "UInteger",
            SpecialType.System_UInt64 => "ULong",
            _ => null,
        };
    }

    /// <summary>Gets the Visual Basic keyword identifier for the provided type, if it is a special type.</summary>
    /// <param name="typeSymbol">The type symbol whose keyword identifier to get.</param>
    /// <returns>The keyword identifier of the provided special type, if supported; otherwise, <see langword="null"/>.</returns>
    public static string? GetKeywordIdentifierForPredefinedType(this ITypeSymbol? typeSymbol)
    {
        return (typeSymbol?.SpecialType ?? SpecialType.None).GetKeywordIdentifierForPredefinedType();
    }

    /// <summary>Determines whether the given type symbol has a Visual Basic keyword identifier for its represented predefined type.</summary>
    /// <param name="typeSymbol">The type to determine whether it has a keyword identifier.</param>
    /// <returns><see langword="true"/> if the type symbol can be represented by a keyword identifier, otherwise <see langword="false"/>.</returns>
    public static bool HasKeywordIdentifier(this ITypeSymbol? typeSymbol)
    {
        return typeSymbol.GetKeywordIdentifierForPredefinedType() is not null;
    }
}
