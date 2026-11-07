using Microsoft.CodeAnalysis;
using RoseLynn.Core;

namespace RoseLynn.CSharp;

/// <summary>Provides C#-specific symbol name formatting helpers.</summary>
public static partial class CSharpSymbolNameHelpers
{
    /// <summary>Gets the full metadata name for a symbol, using predefined-type keywords when available.</summary>
    /// <param name="symbol">The symbol to format.</param>
    /// <param name="compilation">An optional compilation used when formatting context-sensitive details.</param>
    /// <returns>The metadata-or-keyword name wrapper.</returns>
    public static global::RoseLynn.Core.SymbolNameHelpers.FullMetadataName FullMetadataOrKeyword(
        ISymbol symbol,
        Compilation? compilation = null)
    {
        if (symbol is ITypeSymbol type)
        {
            return FullMetadataOrKeyword(type, compilation);
        }
        return global::RoseLynn.Core.SymbolNameHelpers.FullMetadata(symbol, compilation);
    }

    /// <summary>Gets the full metadata name for a type, using predefined-type keywords when available.</summary>
    /// <param name="type">The type to format.</param>
    /// <param name="compilation">An optional compilation used when formatting context-sensitive details.</param>
    /// <returns>The metadata-or-keyword name wrapper.</returns>
    public static global::RoseLynn.Core.SymbolNameHelpers.FullMetadataName FullMetadataOrKeyword(
        ITypeSymbol type,
        Compilation? compilation = null)
    {
        return type.GetKeywordIdentifierForPredefinedType()
            ?? global::RoseLynn.Core.SymbolNameHelpers.FullMetadata(type, compilation);
    }
}
