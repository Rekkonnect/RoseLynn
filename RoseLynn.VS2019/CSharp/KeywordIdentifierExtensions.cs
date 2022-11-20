using Microsoft.CodeAnalysis;

namespace RoseLynn.CSharp;

#nullable enable

/// <summary>Contains helper methods for keyword identifiers.</summary>
public static class KeywordIdentifierExtensions
{
    /// <summary>Gets the keyword identifier for the provided predfined special type.</summary>
    /// <param name="specialType">The special type whose keyword identifier to get.</param>
    /// <returns>The keyword identifier of the provided special type, if supported; otherwise, <see langword="null"/>.</returns>
    /// <remarks><see langword="dynamic"/> is not supported as a predefined special type.</remarks>
    public static string? GetKeywordIdentifierForPredefinedType(this SpecialType specialType)
    {
        return specialType switch
        {
            SpecialType.System_Byte => "byte",
            SpecialType.System_Int16 => "short",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Int64 => "long",
            SpecialType.System_SByte => "sbyte",
            SpecialType.System_UInt16 => "ushort",
            SpecialType.System_UInt32 => "uint",
            SpecialType.System_UInt64 => "ulong",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "double",
            SpecialType.System_Decimal => "decimal",
            SpecialType.System_String => "string",
            SpecialType.System_Char => "char",
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Object => "object",
            SpecialType.System_IntPtr => "nint",
            SpecialType.System_UIntPtr => "nuint",
            SpecialType.System_Void => "void",
            _ => null,
        };
    }
    /// <summary>Gets the keyword identifier for the provided type, if it is a special type.</summary>
    /// <param name="typeSymbol">The special type whose keyword identifier to get.</param>
    /// <returns>The keyword identifier of the provided special type, if supported; otherwise, <see langword="null"/>.</returns>
    /// <remarks><see langword="dynamic"/> is supported in this overload and will be returned with its dedicated keyword.</remarks>
    public static string? GetKeywordIdentifierForPredefinedType(this ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is IDynamicTypeSymbol)
            return "dynamic";

        return (typeSymbol?.SpecialType ?? SpecialType.None).GetKeywordIdentifierForPredefinedType();
    }

    /// <summary>Determines whether the given type symbol has a keyword identifier for its represented predefined type.</summary>
    /// <param name="typeSymbol">The type to determine whether it has a keyword identifier.</param>
    /// <returns><see langword="true"/> if the type symbol can be represented by a keyword identifier, otherwise <see langword="false"/>./</returns>
    public static bool HasKeywordIdentifier(this ITypeSymbol? typeSymbol)
    {
        return typeSymbol.GetKeywordIdentifierForPredefinedType() is not null;
    }
}
