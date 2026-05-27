using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Identifies a symbol by its name, containing types, namespace, and optional arity, for cross-compilation matching.</summary>
public readonly struct QualifiedSymbolNameMatch(
    string name,
    string containingTypes,
    string namespaces,
    int? arity)
{
    /// <summary>The symbol's simple name.</summary>
    public readonly string Name = name;
    /// <summary>The dot-separated chain of containing type names.</summary>
    public readonly string ContainingTypes = containingTypes;
    /// <summary>The dot-separated namespace.</summary>
    public readonly string Namespaces = namespaces;
    /// <summary>The arity to match, or <see langword="null"/> to match any arity.</summary>
    public readonly int? Arity = arity;

    /// <summary>Returns <see langword="true"/> if <paramref name="symbol"/> matches this instance's name, containing types, namespace, and arity.</summary>
    /// <param name="symbol">The symbol to test.</param>
    /// <returns><see langword="true"/> when all criteria match; otherwise <see langword="false"/>.</returns>
    public bool Matches(ISymbol? symbol)
    {
        return symbol is not null
            && symbol.Name == Name
            && MatchesArity(symbol)
            && symbol.ContainingType.HasQualifiedName(ContainingTypes)
            && symbol.ContainingNamespace.HasQualifiedName(Namespaces);
    }

    /// <summary>Returns <see langword="true"/> if any symbol in <paramref name="symbols"/> matches this instance.</summary>
    /// <param name="symbols">The symbols to test.</param>
    /// <returns><see langword="true"/> when at least one symbol matches; otherwise <see langword="false"/>.</returns>
    public bool MatchesAny(IEnumerable<ISymbol>? symbols)
    {
        return symbols?.Any(Matches) ?? false;
    }

    /// <summary>Returns the first symbol in <paramref name="symbols"/> that matches this instance, or <see langword="null"/> if none match.</summary>
    /// <param name="symbols">The symbols to search.</param>
    /// <returns>The first matching symbol, or <see langword="null"/>.</returns>
    public ISymbol? FirstMatchingOrDefault(IEnumerable<ISymbol>? symbols)
    {
        return symbols?.FirstOrDefault(Matches) ?? null;
    }

    private bool MatchesArity(ISymbol symbol)
    {
        return Arity is null
            || (symbol is IMethodSymbol method && method.Arity == Arity)
            || (symbol is INamedTypeSymbol type && type.Arity == Arity);
    }

    /// <summary>Creates a <seealso cref="QualifiedSymbolNameMatch"/> matching a top-level symbol with the given name and namespace.</summary>
    /// <param name="name">The symbol's simple name.</param>
    /// <param name="namespaces">The dot-separated namespace string.</param>
    /// <returns>A match with no containing type and no arity constraint.</returns>
    public static QualifiedSymbolNameMatch WithNameAndNamespaces(
        string name, string namespaces)
    {
        return new(
            name,
            string.Empty,
            namespaces,
            arity: null);
    }
}
