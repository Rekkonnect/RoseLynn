using Microsoft.CodeAnalysis;

namespace RoseLynn
{
    /// <summary>Provides useful extensions for the <seealso cref="ISymbol"/> type.</summary>
    public static class ISymbolExtensions
    {
        /// <summary>Gets the <seealso cref="IdentifiableSymbolKind"/> of an <seealso cref="ISymbol"/>.</summary>
        /// <param name="symbol">The symbol whose <seealso cref="IdentifiableSymbolKind"/> to get.</param>
        /// <returns>The <seealso cref="IdentifiableSymbolKind"/> of the symbol. If it's an <seealso cref="IAliasSymbol"/>, the <seealso cref="IdentifiableSymbolKind.Alias"/> flag along with the actual symbol's <seealso cref="IdentifiableSymbolKind"/> is returned.</returns>
        public static IdentifiableSymbolKind GetIdentifiableSymbolKind(this ISymbol symbol)
        {
            return symbol switch
            {
                INamespaceSymbol   => IdentifiableSymbolKind.Namespace,

                ITypeSymbol type   => GetIdentifiableSymbolKind(type),

                IParameterSymbol   => IdentifiableSymbolKind.Parameter,

                IEventSymbol       => IdentifiableSymbolKind.Event,
                IFieldSymbol       => IdentifiableSymbolKind.Field,
                IPropertySymbol    => IdentifiableSymbolKind.Property,
                IMethodSymbol      => IdentifiableSymbolKind.Method,

                IAliasSymbol alias => IdentifiableSymbolKind.Alias | GetIdentifiableSymbolKind(alias.Target),

                _ => IdentifiableSymbolKind.None,
            };
        }

        /// <summary>Gets the <seealso cref="IdentifiableSymbolKind"/> of an <seealso cref="ITypeSymbol"/>.</summary>
        /// <param name="typeSymbol">The type symbol whose <seealso cref="IdentifiableSymbolKind"/> to get.</param>
        /// <returns>The <seealso cref="IdentifiableSymbolKind"/> of the type symbol.</returns>
        public static IdentifiableSymbolKind GetIdentifiableSymbolKind(this ITypeSymbol typeSymbol)
        {
            var kind = IdentifiableSymbolKind.None;
            if (typeSymbol.IsRecord)
                kind |= IdentifiableSymbolKind.Record;

            kind |= typeSymbol.TypeKind switch
            {
                TypeKind.Class         => IdentifiableSymbolKind.Class,
                TypeKind.Struct        => IdentifiableSymbolKind.Struct,
                TypeKind.Interface     => IdentifiableSymbolKind.Interface,
                TypeKind.Enum          => IdentifiableSymbolKind.Enum,
                TypeKind.Delegate      => IdentifiableSymbolKind.Delegate,
                TypeKind.TypeParameter => IdentifiableSymbolKind.GenericParameter,

                _ => IdentifiableSymbolKind.None,
            };

            return kind;
        }
    }
}
