using Microsoft.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace RoseLynn;

#nullable enable

/// <summary>Contains extensions for the <seealso cref="Compilation"/> class.</summary>
public static class CompilationExtensions
{
    /// <summary>Given a <seealso cref="ISymbol"/> from another <seealso cref="Compilation"/>, gets the symbol matching the same <seealso cref="SymbolKind"/> and <seealso cref="FullSymbolName"/>.</summary>
    /// <param name="compilation">The <seealso cref="Compilation"/> whose <seealso cref="ISymbol"/> will be returned, if matched.</param>
    /// <param name="match">The matching symbol whose kind and symbol name will be respected.</param>
    /// <param name="matchingLevel">The level at which the symbol names will be compared.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <seealso cref="ISymbol"/> contained in <paramref name="compilation"/> that matches the name and kind of <paramref name="match"/>,
    /// or <see langword="null"/> if the operation has been cancelled, or no symbol matched the requested symbol's properties.
    /// </returns>
    public static ISymbol? GetMatchingSymbol(this Compilation compilation, ISymbol? match, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace, CancellationToken cancellationToken = default)
    {
        if (match is null)
            return null;

        var symbols = compilation.GetSymbolsWithName(match.Name, match.GetRespectiveSymbolFilter(), cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;

        return symbols.FirstOrDefault(symbol => symbol.MatchesKindAndFullSymbolName(match, matchingLevel));
    }
}
