using Garyon.Functions;
using Microsoft.CodeAnalysis;
using RoseLynn.Core;
using RoseLynn.Core.Utilities;

namespace RoseLynn.CSharp;

public static partial class CSharpSymbolNameHelpers
{
    /// <summary>Returns the fully qualified name of <paramref name="symbol"/>, substituting the C# keyword form when the symbol is a predefined type.</summary>
    /// <param name="symbol">The symbol whose name to compute.</param>
    /// <param name="compilation">The compilation used to resolve extern aliases, or <see langword="null"/> to use the global alias.</param>
    /// <returns>The keyword-form <see cref="global::RoseLynn.Core.SymbolNameHelpers.FullyQualifiedName"/> if <paramref name="symbol"/> is a predefined type; otherwise the fully qualified name.</returns>
    public static global::RoseLynn.Core.SymbolNameHelpers.FullyQualifiedName FullyQualifiedOrKeyword(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        if (symbol is ITypeSymbol type)
        {
            return FullyQualifiedOrKeyword(type, compilation);
        }
        return global::RoseLynn.Core.SymbolNameHelpers.FullyQualified(symbol, compilation);
    }

    /// <summary>Returns the fully qualified name of <paramref name="type"/>, substituting the C# keyword form when the type is a predefined type.</summary>
    /// <param name="type">The type symbol whose name to compute.</param>
    /// <param name="compilation">The compilation used to resolve extern aliases, or <see langword="null"/> to use the global alias.</param>
    /// <returns>The keyword-form <see cref="global::RoseLynn.Core.SymbolNameHelpers.FullyQualifiedName"/> if <paramref name="type"/> is a predefined type; otherwise the fully qualified name.</returns>
    public static global::RoseLynn.Core.SymbolNameHelpers.FullyQualifiedName FullyQualifiedOrKeyword(
        ITypeSymbol type,
        Compilation? compilation = null)
    {
        return KeywordIdentifierWithNullability(type)
            ?? global::RoseLynn.Core.SymbolNameHelpers.FullyQualified(type, compilation);
    }

    /// <summary>Returns the C# keyword identifier for <paramref name="type"/> with a <c>?</c> suffix when the type is nullable-annotated, or <see langword="null"/> if the type has no predefined keyword.</summary>
    /// <param name="type">The type symbol to get the keyword for.</param>
    /// <returns>The keyword string (e.g. <c>int?</c>, <c>string</c>) or <see langword="null"/> if not a predefined type.</returns>
    public static string? KeywordIdentifierWithNullability(ITypeSymbol type)
    {
        if (type.IsNullableStruct(out var underlying))
        {
            var underlyingKeyword = underlying.GetKeywordIdentifierForPredefinedType();
            return underlyingKeyword.SelectOrNull(WithNullableMarker);
        }

        var keyword = type.GetKeywordIdentifierForPredefinedType();
        if (keyword is null)
        {
            return null;
        }

        if (NeedsNullableMarker(type))
        {
            return WithNullableMarker(keyword);
        }

        return keyword;
    }

    private static string WithNullableMarker(string typeSyntax)
    {
        return $"{typeSyntax}?";
    }

    private static bool NeedsNullableMarker(ISymbol? symbol)
    {
        return symbol
            is ITypeSymbol { NullableAnnotation: NullableAnnotation.Annotated };
    }
}
