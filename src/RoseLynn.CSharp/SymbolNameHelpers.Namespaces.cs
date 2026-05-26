using Microsoft.CodeAnalysis;

namespace RoseLynn.CSharp;

public static partial class SymbolNameHelpers
{
    public static string ContainedNamespaceName(ISymbol symbol)
    {
        return FullMetadataString(symbol.ContainingNamespace);
    }
}
