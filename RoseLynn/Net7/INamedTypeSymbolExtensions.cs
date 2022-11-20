using Microsoft.CodeAnalysis;
using System.Linq;

namespace RoseLynn.Net7;

/// <summary>
/// Provides useful extensions for the <seealso cref="INamedTypeSymbol"/> type,
/// accounting for .NET 7.0 and above.
/// </summary>
public static class INamedTypeSymbolExtensions
{
    /// <summary>
    /// Determines whether the provided named type symbol contains <see langword="static abstract"/> memebrs.
    /// </summary>
    /// <param name="namedSymbol">The type symbol whose members to investigate.</param>
    /// <returns><see langword="true"/> if the provided symbol contains <see langword="static abstract"/> members.</returns>
    public static bool ContainsStaticAbstracts(this INamedTypeSymbol namedSymbol)
    {
        // No restriction on interfaces as other type kinds could enable static abstracts
        return namedSymbol is not null
            && ContainsStaticMembers(namedSymbol);
    }

    private static bool ContainsStaticMembers(INamedTypeSymbol symbol)
    {
        return symbol.GetMembers()
                     // Avoid encountering nested static classes
                     .Where(m => m is not ITypeSymbol)
                     .Any(m => m.IsStatic && m.IsAbstract);
    }
}
