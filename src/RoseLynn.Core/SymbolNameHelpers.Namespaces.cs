using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

public static partial class SymbolNameHelpers
{
    /// <summary>Gets the full metadata name of the namespace containing the given symbol.</summary>
    /// <param name="symbol">The symbol whose containing namespace name to retrieve.</param>
    public static string ContainedNamespaceName(ISymbol symbol)
    {
        return FullMetadataString(symbol.ContainingNamespace);
    }
}
