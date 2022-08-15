using Microsoft.CodeAnalysis;
using RoseLynn.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn;

#nullable enable

/// <summary>Contains extensions for the <seealso cref="INamespaceSymbol"/> interface.</summary>
public static class INamespaceSymbolExtensions
{
    /// <summary>Gets a collection of all the parent namespaces including the provided one.</summary>
    /// <param name="namespaceSymbol">The final <see cref="INamespaceSymbol"/> whose ancestors to get.</param>
    /// <returns>
    /// A collection of all the ancestor namespace symbols, including the provided one in the end.<br/>
    /// For example: given the namespace A.B.C, the result will be a list containing the namespace symbols for A, B and C in that order.
    /// </returns>
    public static IEnumerable<INamespaceSymbol> AncestorNamespacesIncludingThis(this INamespaceSymbol namespaceSymbol)
    {
        return namespaceSymbol.GetContainingNamespaces().Concat(new SingleElementCollection<INamespaceSymbol>(namespaceSymbol).AsEnumerable<INamespaceSymbol>());
    }
}
