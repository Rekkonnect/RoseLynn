using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoseLynn;

/// <summary>Contains extensions for the <seealso cref="INamespaceOrTypeSymbol"/> interface.</summary>
public static class INamespaceOrTypeSymbolExtensions
{
    /// <summary>Gets the members of the specified symbol type contained in the provided <seealso cref="INamespaceOrTypeSymbol"/>.</summary>
    /// <typeparam name="TMember">The type of the members that are contained.</typeparam>
    /// <param name="symbol">The symbol whose members of the specified type to get.</param>
    /// <returns>The contained symbols of the specified symbol type.</returns>
    public static IEnumerable<TMember> GetMembers<TMember>(this INamespaceOrTypeSymbol symbol)
        where TMember : ISymbol
    {
        return symbol.GetMembers().OfType<TMember>();
    }
}
