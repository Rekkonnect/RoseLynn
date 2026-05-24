using Microsoft.CodeAnalysis;

namespace RoseLynn;

public readonly struct QualifiedSymbolNameMatch(
    string name,
    string containingTypes,
    string namespaces,
    int? arity)
{
    public readonly string Name = name;
    public readonly string ContainingTypes = containingTypes;
    public readonly string Namespaces = namespaces;
    public readonly int? Arity = arity;

    public bool Matches(ISymbol? symbol)
    {
        return symbol is not null
            && symbol.Name == Name
            && MatchesArity(symbol)
            && symbol.ContainingType.HasQualifiedName(ContainingTypes)
            && symbol.ContainingNamespace.HasQualifiedName(Namespaces);
    }

    public bool MatchesAny(IEnumerable<ISymbol>? symbols)
    {
        return symbols?.Any(Matches) ?? false;
    }

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
