using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

#nullable enable

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
            var kind = typeSymbol.TypeKind.GetIdentifiableSymbolKindFlag();
            if (typeSymbol.IsRecord)
                kind |= IdentifiableSymbolKind.Record;

            return kind;
        }

        /// <summary>Gets the respective <seealso cref="SymbolFilter"/> matching symbols of the same kind as the given symbol's kind.</summary>
        /// <param name="symbol">The symbol whose <seealso cref="ISymbol.Kind"/> will be matched by the returning <seealso cref="SymbolFilter"/>.</param>
        /// <returns>The minimum available <seealso cref="SymbolFilter"/> that matches the given symbol, if not null, otherwise <seealso cref="SymbolFilter.None"/>.</returns>
        public static SymbolFilter GetRespectiveSymbolFilter(this ISymbol? symbol)
        {
            return symbol?.Kind switch
            {
                SymbolKind kind => kind.GetRespectiveSymbolFilter(),
                null => SymbolFilter.None,
            };
        }

        /// <summary>Creates a <seealso cref="IdentifierWithArity"/> representing the symbol's name and arity.</summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="symbolNameKind">The <seealso cref="SymbolNameKind"/> of the name to get and use in the <seealso cref="IdentifierWithArity"/>.</param>
        /// <returns>A <seealso cref="IdentifierWithArity"/> instance representing the given symbol's name and its arity.</returns>
        public static IdentifierWithArity GetIdentifierWithArity(this ISymbol symbol, SymbolNameKind symbolNameKind = SymbolNameKind.Normal)
        {
            return new(symbol.GetName(symbolNameKind)!, symbol.GetArity());
        }

        /// <summary>Selects the symbol names of the given symbols, with the result strings representing the given <seealso cref="SymbolNameKind"/>.</summary>
        /// <param name="symbols">The symbols whose symbol names to get.</param>
        /// <param name="symbolNameKind">The <see cref="SymbolNameKind"/> representing the symbol name to get for each of the given symbols.</param>
        /// <returns>The names of the symbols in the same order, according to the requested symbol name kind.</returns>
        public static IEnumerable<string> SelectSymbolNames(this IEnumerable<ISymbol> symbols, SymbolNameKind symbolNameKind)
        {
            return symbols.Select(symbol => symbol.GetName(symbolNameKind)!);
        }
        /// <summary>Selects the symbol names of the given symbols.</summary>
        /// <param name="symbols">The symbols whose symbol names to get.</param>
        /// <returns>The names of the symbols in the same order, according to the <seealso cref="ISymbol.Name"/> property.</returns>
        public static IEnumerable<string> SelectSymbolNames(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Select(symbol => symbol.Name);
        }
        /// <summary>Selects the symbol metadata names of the given symbols.</summary>
        /// <param name="symbols">The symbols whose symbol metadata names to get.</param>
        /// <returns>The names of the symbols in the same order, according to the <seealso cref="ISymbol.MetadataName"/> property.</returns>
        public static IEnumerable<string> SelectSymbolMetadataNames(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Select(symbol => symbol.MetadataName);
        }

        /// <summary>Gets the containing namespaces of the symbol.</summary>
        /// <param name="symbol">The symbol whose containing namespaces to get.</param>
        /// <returns>The containing namespaces of the symbol, ordered from the outermost to the most nested.</returns>
        public static IEnumerable<INamespaceSymbol> GetContainingNamespaces(this ISymbol? symbol)
        {
            return GetContainingSymbols<INamespaceSymbol>(symbol, ContainerSymbolKind.Namespace);
        }
        /// <summary>Gets the containing methods of the symbol.</summary>
        /// <param name="symbol">The symbol whose containing methods to get.</param>
        /// <returns>The containing methods of the symbol, ordered from the outermost to the most nested.</returns>
        public static IEnumerable<IMethodSymbol> GetContainingMethods(this ISymbol? symbol)
        {
            return GetContainingSymbols<IMethodSymbol>(symbol, ContainerSymbolKind.Method);
        }
        /// <summary>Gets the containing types of the symbol.</summary>
        /// <param name="symbol">The symbol whose containing types to get.</param>
        /// <returns>The containing types of the symbol, ordered from the outermost to the most nested.</returns>
        public static IEnumerable<INamedTypeSymbol> GetContainingTypes(this ISymbol? symbol)
        {
            return GetContainingSymbols<INamedTypeSymbol>(symbol, ContainerSymbolKind.Type);
        }

        private static IEnumerable<TContainer> GetContainingSymbols<TContainer>(this ISymbol? symbol, ContainerSymbolKind containerSymbolKind)
            where TContainer : class, ISymbol
        {
            if (symbol is null)
                return Enumerable.Empty<TContainer>();

            var types = new List<TContainer>();

            var currentType = symbol.GetContainingSymbol(containerSymbolKind) as TContainer;
            while (true)
            {
                if (currentType is null)
                    break;

                types.Add(currentType);
                currentType = currentType.GetContainingSymbol(containerSymbolKind) as TContainer;
            }

            types.Reverse();
            return types;
        }

        /// <summary>Gets the directly containing symbol given its <seealso cref="ContainerSymbolKind"/>.</summary>
        /// <param name="symbol">The symbol whose directly containing symbol of the required kind to get.</param>
        /// <param name="containerSymbolKind">The <seealso cref="ContainerSymbolKind"/> of the directly containing symbol to get.</param>
        /// <returns>The given symbol's directly containing symbol of the specified <seealso cref="ContainerSymbolKind"/>.</returns>
        public static ISymbol? GetContainingSymbol(this ISymbol? symbol, ContainerSymbolKind containerSymbolKind)
        {
            if (symbol is null)
                return null;

            return containerSymbolKind switch
            {
                ContainerSymbolKind.Any => symbol.ContainingSymbol,

                // The as cast ensures that the containing symbol is a method
                ContainerSymbolKind.Method => symbol.ContainingSymbol as IMethodSymbol,
                ContainerSymbolKind.Type => symbol.ContainingType,
                ContainerSymbolKind.Namespace => symbol.ContainingNamespace,
                ContainerSymbolKind.Module => symbol.ContainingModule,
                ContainerSymbolKind.Assembly => symbol.ContainingAssembly,
            };
        }

        /// <summary>Gets the full symbol name of the given <seealso cref="ISymbol"/>, with each of the relevant symbols having the specified <seealso cref="SymbolNameKind"/>, using <seealso cref="FullSymbolName.DefaultContainerSymbolDelimiter"/> as the delimiter.</summary>
        /// <param name="symbol">The symbol whose full name to get.</param>
        /// <param name="symbolNameKind">The <seealso cref="SymbolNameKind"/> of the names of each symbol, including the containing symbols. <seealso cref="SymbolNameKind.Full"/> will be considered <see cref="SymbolNameKind.Normal"/>.</param>
        /// <returns>The <seealso cref="FullSymbolName"/> containing information about the full symbol name, or <see langword="null"/> if <paramref name="symbol"/> is <see langword="null"/>.</returns>
        public static FullSymbolName? GetFullSymbolNameDefaultDelimiters(this ISymbol? symbol, SymbolNameKind symbolNameKind = SymbolNameKind.Normal)
        {
            var fullName = symbol.GetFullSymbolName(symbolNameKind);
            fullName?.UseDefaultContainerSymbolDelimiter();
            return fullName;
        }
        /// <summary>Gets the full symbol name of the given <seealso cref="ISymbol"/>, with each of the relevant symbols having the specified <seealso cref="SymbolNameKind"/>.</summary>
        /// <param name="symbol">The symbol whose full name to get.</param>
        /// <param name="symbolNameKind">The <seealso cref="SymbolNameKind"/> of the names of each symbol, including the containing symbols. <seealso cref="SymbolNameKind.Full"/> will be considered <see cref="SymbolNameKind.Normal"/>.</param>
        /// <returns>The <seealso cref="FullSymbolName"/> containing information about the full symbol name, or <see langword="null"/> if <paramref name="symbol"/> is <see langword="null"/>.</returns>
        public static FullSymbolName? GetFullSymbolName(this ISymbol? symbol, SymbolNameKind symbolNameKind = SymbolNameKind.Normal)
        {
            if (symbol is null)
                return null;

            if (symbolNameKind is SymbolNameKind.Full)
                symbolNameKind = SymbolNameKind.Normal;

            var namespaces = symbol.GetContainingNamespaces();
            var containerTypes = symbol.GetContainingTypes();
            var containerMethods = symbol.GetContainingMethods();

            return new(symbol, namespaces, containerTypes, containerMethods, symbolNameKind);
        }

        /// <summary>Gets the name of the given <seealso cref="ISymbol"/> for the specified <seealso cref="SymbolNameKind"/>.</summary>
        /// <param name="symbol">The symbol whose name to get.</param>
        /// <param name="nameKind">The <seealso cref="SymbolNameKind"/> of the name to get.</param>
        /// <returns>The name of the given <seealso cref="ISymbol"/> for the specified <seealso cref="SymbolNameKind"/>.</returns>
        public static string? GetName(this ISymbol? symbol, SymbolNameKind nameKind)
        {
            if (symbol is null)
                return null;

            return nameKind switch
            {
                SymbolNameKind.Normal => symbol.Name,
                SymbolNameKind.Full => symbol.GetFullSymbolNameDefaultDelimiters()!.FullNameString,
                SymbolNameKind.Metadata => symbol.MetadataName,
                _ => null,
            };
        }

        /// <summary>Determines whether two <seealso cref="ISymbol"/> instances match w.r.t. their <seealso cref="SymbolKind"/> and <seealso cref="FullSymbolName"/> values.</summary>
        /// <param name="symbol">The source symbol to compare.</param>
        /// <param name="match">The target symbol to compare.</param>
        /// <param name="matchingLevel">The highest level at which to compare the <seealso cref="FullSymbolName"/> instances of the symbols.</param>
        /// <returns><see langword="true"/> if the symbols' <seealso cref="SymbolKind"/> values match, and the <seealso cref="FullSymbolName"/> instances match up to the specified matching level, otherwise <see langword="false"/>.</returns>
        public static bool MatchesKindAndFullSymbolName(this ISymbol symbol, ISymbol match, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.SymbolName)
        {
            return symbol.Kind == match.Kind
                && symbol.MatchesFullSymbolName(match, matchingLevel);
        }
        /// <summary>Determines whether two <seealso cref="ISymbol"/> instances match w.r.t. their <seealso cref="FullSymbolName"/> values.</summary>
        /// <param name="symbol">The source symbol whose <seealso cref="FullSymbolName"/> to compare.</param>
        /// <param name="match">The target symbol whose <seealso cref="FullSymbolName"/> to compare.</param>
        /// <param name="matchingLevel">The highest level at which to compare the <seealso cref="FullSymbolName"/> instances of the symbols.</param>
        /// <returns><see langword="true"/> if the <seealso cref="FullSymbolName"/> instances match up to the specified matching level, otherwise <see langword="false"/>.</returns>
        public static bool MatchesFullSymbolName(this ISymbol symbol, ISymbol match, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.SymbolName)
        {
            // Eliminate the expensive operation of constructing the full symbol name
            if (matchingLevel is SymbolNameMatchingLevel.SymbolName)
                return symbol.Name == match.Name;

            return symbol.GetFullSymbolName()!.Matches(match.GetFullSymbolName()!, matchingLevel);
        }

        /// <summary>Gets the first attribute of the given <seealso cref="ISymbol"/> that matches the specified name.</summary>
        /// <param name="symbol">The symbol whose attribute to get.</param>
        /// <param name="name">The name of the attribute to get.</param>
        /// <returns>The <seealso cref="AttributeData"/> of the first attribute that matches the given name, or <see langword="null"/> if none was found.</returns>
        public static AttributeData? FirstOrDefaultAttributeNamed(this ISymbol symbol, string name)
        {
            return symbol.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass?.Name == name);
        }
        /// <summary>Gets the first attribute of the given <seealso cref="ISymbol"/> that matches the specified <seealso cref="FullSymbolName"/>.</summary>
        /// <param name="symbol">The symbol whose attribute to get.</param>
        /// <param name="attributeName">The name of the attribute to get.</param>
        /// <param name="attributeNameMatchingLevel">The matching level for the full name.</param>
        /// <returns>The <seealso cref="AttributeData"/> of the first attribute that matches the given name, or <see langword="null"/> if none was found.</returns>
        public static AttributeData? FirstOrDefaultAttributeNamed(this ISymbol symbol, FullSymbolName attributeName, SymbolNameMatchingLevel attributeNameMatchingLevel = SymbolNameMatchingLevel.Namespace)
        {
            return symbol.GetAttributes().FirstOrDefault(MatchesFullName);

            bool MatchesFullName(AttributeData attribute)
            {
                return attribute.AttributeClass?.GetFullSymbolName()!.Matches(attributeName, attributeNameMatchingLevel) is true;
            }
        }
    }
}
