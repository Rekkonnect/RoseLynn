#nullable enable

using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Represents the kind of a symbol name.</summary>
public enum SymbolNameKind
{
    /// <summary>The normal symbol name, that only consists of its identifier, as returned by <seealso cref="ISymbol.Name"/>.</summary>
    Normal,
    /// <summary>The fully qualified name of the symbol, concatenating all its containing symbols' names.</summary>
    Full,
    /// <summary>The metadata name of the symbol, as returned by <seealso cref="ISymbol.MetadataName"/>.</summary>
    Metadata,
}
