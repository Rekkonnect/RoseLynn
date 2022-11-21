using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace RoseLynn;

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
        return symbol?.Kind.GetRespectiveSymbolFilter() ?? SymbolFilter.None;
    }

    /// <summary>Creates a <seealso cref="IdentifierWithArity"/> representing the symbol's name and arity.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="symbolNameKind">The <seealso cref="SymbolNameKind"/> of the name to get and use in the <seealso cref="IdentifierWithArity"/>.</param>
    /// <returns>A <seealso cref="IdentifierWithArity"/> instance representing the given symbol's name and its arity.</returns>
    public static IdentifierWithArity GetIdentifierWithArity(this ISymbol symbol, SymbolNameKind symbolNameKind = SymbolNameKind.Normal)
    {
        if (symbolNameKind is SymbolNameKind.Metadata)
            return IdentifierWithArity.Parse(symbol.MetadataName);

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

        var currentContainingSymbol = symbol.GetContainingSymbol(containerSymbolKind) as TContainer;
        while (currentContainingSymbol is not null)
        {
            types.Add(currentContainingSymbol);
            currentContainingSymbol = currentContainingSymbol.GetContainingSymbol(containerSymbolKind) as TContainer;
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

    #region Attribute Name Matching
    #region HasAttributeNamed
    /// <summary>Determines whether the symbol contains an attribute whose name matches the provided name.</summary>
    /// <param name="symbol">The symbol for which to attempt to find any attributes named like the provided attribute type.</param>
    /// <param name="attributeName">The name of the attribute whose name to attempt to find.</param>
    /// <returns><see langword="true"/> if the symbol has been marked with any attribute whose name matches that of the provided attribute type's.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="HasAttributeNamed(ISymbol, FullSymbolName, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static bool HasAttributeNamed(this ISymbol? symbol, string attributeName)
    {
        return symbol?.FirstOrDefaultAttributeNamed(attributeName) is not null;
    }
    /// <summary>Determines whether the symbol contains an attribute whose name matches the provided name.</summary>
    /// <param name="symbol">The symbol whose attribute to get.</param>
    /// <param name="attributeName">The name of the attribute to get.</param>
    /// <param name="attributeNameMatchingLevel">The matching level for the full name.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the first attribute that matches the given name, or <see langword="null"/> if none was found.</returns>
    public static bool HasAttributeNamed(this ISymbol? symbol, FullSymbolName attributeName, SymbolNameMatchingLevel attributeNameMatchingLevel = SymbolNameMatchingLevel.Namespace)
    {
        return symbol?.FirstOrDefaultAttributeNamed(attributeName, attributeNameMatchingLevel) is not null;
    }
    /// <summary>Determines whether the symbol contains an attribute whose name matches the provided name.</summary>
    /// <typeparam name="T">The type of the attribute whose name to attempt to find.</typeparam>
    /// <param name="symbol">The symbol for which to attempt to find any attributes named like the provided attribute type.</param>
    /// <returns><see langword="true"/> if the symbol has been marked with any attribute whose name matches that of the provided attribute type's.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="HasAttributeNamedFully{T}(ISymbol, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static bool HasAttributeNamed<T>(this ISymbol? symbol)
        where T : Attribute
    {
        return HasAttributeNamed(symbol, typeof(T).Name);
    }
    /// <summary>Determines whether the symbol contains an attribute whose name matches the provided name.</summary>
    /// <param name="symbol">The symbol for which to attempt to find any attributes named like the provided attribute type.</param>
    /// <param name="attributeType">The type of the attribute whose name to attempt to find.</param>
    /// <returns><see langword="true"/> if the symbol has been marked with any attribute whose name matches that of the provided attribute type's.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="HasAttributeNamedFully(ISymbol, Type, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static bool HasAttributeNamed(this ISymbol? symbol, Type attributeType)
    {
        return HasAttributeNamed(symbol, attributeType.Name);
    }
    #endregion

    #region FirstOrDefaultAttributeNamed
    /// <summary>Gets the first attribute of the given <seealso cref="ISymbol"/> that matches the specified name.</summary>
    /// <param name="symbol">The symbol whose attribute to get.</param>
    /// <param name="name">The name of the attribute to get.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the first attribute that matches the given name, or <see langword="null"/> if none was found.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="FirstOrDefaultAttributeNamed(ISymbol, FullSymbolName, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static AttributeData? FirstOrDefaultAttributeNamed(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes().FirstOrDefault(AttributeNameMatcher(name));
    }
    /// <summary>Gets the first attribute of the given <seealso cref="ISymbol"/> that matches the specified <seealso cref="FullSymbolName"/>.</summary>
    /// <param name="symbol">The symbol whose attribute to get.</param>
    /// <param name="attributeName">The name of the attribute to get.</param>
    /// <param name="attributeNameMatchingLevel">The matching level for the full name.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the first attribute that matches the given name, or <see langword="null"/> if none was found.</returns>
    public static AttributeData? FirstOrDefaultAttributeNamed(this ISymbol symbol, FullSymbolName attributeName, SymbolNameMatchingLevel attributeNameMatchingLevel = SymbolNameMatchingLevel.Namespace)
    {
        return symbol.GetAttributes().FirstOrDefault(AttributeNameMatcher(attributeName, attributeNameMatchingLevel));
    }
    /// <summary>Gets the first attribute of the given <seealso cref="ISymbol"/> that matches the name of the specified attribute.</summary>
    /// <typeparam name="T">The type of the attribute whose name to attempt to find.</typeparam>
    /// <param name="symbol">The symbol whose attribute to get.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the first attribute that matches the given attribute's name, or <see langword="null"/> if none was found.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="FirstOrDefaultAttributeNamedFully{T}(ISymbol, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static AttributeData? FirstOrDefaultAttributeNamed<T>(this ISymbol symbol)
        where T : Attribute
    {
        return symbol.FirstOrDefaultAttributeNamed(typeof(T).Name);
    }
    /// <summary>Gets the first attribute of the given <seealso cref="ISymbol"/> that matches the name of the specified attribute.</summary>
    /// <param name="symbol">The symbol whose attribute to get.</param>
    /// <param name="attributeType">The type of the attribute whose name to attempt to find.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the first attribute that matches the given attribute's name, or <see langword="null"/> if none was found.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="FirstOrDefaultAttributeNamedFully(ISymbol, Type, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static AttributeData? FirstOrDefaultAttributeNamed(this ISymbol symbol, Type attributeType)
    {
        return symbol.FirstOrDefaultAttributeNamed(attributeType.Name);
    }
    #endregion

    #region GetAttributesNamed
    /// <summary>Gets all attributes of the given <seealso cref="ISymbol"/> that match the name of the specified attribute.</summary>
    /// <param name="symbol">The symbol whose attributes to get.</param>
    /// <param name="attributeType">The type of the attribute whose name to attempt to find.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the attributes that match the given attribute's name.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="GetAttributesNamed(ISymbol, FullSymbolName, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static IEnumerable<AttributeData> GetAttributesNamed(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes().Where(AttributeNameMatcher(name));
    }
    /// <summary>Gets all attributes of the given <seealso cref="ISymbol"/> that match the name of the specified attribute.</summary>
    /// <param name="symbol">The symbol whose attributes to get.</param>
    /// <param name="attributeName">The name of the attributes to get.</param>
    /// <param name="attributeNameMatchingLevel">The matching level for the full name.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the attributes that match the given attribute's name.</returns>
    public static IEnumerable<AttributeData> GetAttributesNamed(this ISymbol symbol, FullSymbolName attributeName, SymbolNameMatchingLevel attributeNameMatchingLevel = SymbolNameMatchingLevel.Namespace)
    {
        return symbol.GetAttributes().Where(AttributeNameMatcher(attributeName, attributeNameMatchingLevel));
    }
    /// <summary>Gets all attributes of the given <seealso cref="ISymbol"/> that match the name of the specified attribute.</summary>
    /// <typeparam name="T">The type of the attribute whose name to attempt to find.</typeparam>
    /// <param name="symbol">The symbol whose attributes to get.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the attributes that match the given attribute's name.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="GetAttributesNamedFully{T}(ISymbol, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static IEnumerable<AttributeData> GetAttributesNamed<T>(this ISymbol symbol)
        where T : Attribute
    {
        return GetAttributesNamed(symbol, typeof(T).Name);
    }
    /// <summary>Gets all attributes of the given <seealso cref="ISymbol"/> that match the name of the specified attribute.</summary>
    /// <param name="symbol">The symbol whose attributes to get.</param>
    /// <param name="attributeType">The type of the attribute whose name to attempt to find.</param>
    /// <returns>The <seealso cref="AttributeData"/> of the attributes that match the given attribute's name.</returns>
    /// <remarks>
    /// This method only compares the symbol names, and not any other containing symbols.
    /// Use <seealso cref="GetAttributesNamedFully(ISymbol, Type, SymbolNameMatchingLevel)"/> if you want to compare against
    /// the full name of the attribute.
    /// </remarks>
    public static IEnumerable<AttributeData> GetAttributesNamed(this ISymbol symbol, Type attributeType)
    {
        return GetAttributesNamed(symbol, attributeType.Name);
    }
    #endregion

    public static bool HasAttributeNamedFully<T>(this ISymbol? symbol, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
        where T : Attribute
    {
        return HasAttributeNamed(symbol, FullSymbolName.ForType<T>(), matchingLevel);
    }
    public static bool HasAttributeNamedFully(this ISymbol? symbol, Type attributeType, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
    {
        return HasAttributeNamed(symbol, FullSymbolName.ForType(attributeType), matchingLevel);
    }

    public static AttributeData? FirstOrDefaultAttributeNamedFully<T>(this ISymbol symbol, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
        where T : Attribute
    {
        return FirstOrDefaultAttributeNamed(symbol, FullSymbolName.ForType<T>(), matchingLevel);
    }
    public static AttributeData? FirstOrDefaultAttributeNamedFully(this ISymbol symbol, Type attributeType, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
    {
        return FirstOrDefaultAttributeNamed(symbol, FullSymbolName.ForType(attributeType), matchingLevel);
    }

    public static IEnumerable<AttributeData> GetAttributesNamedFully<T>(this ISymbol symbol, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
        where T : Attribute
    {
        return GetAttributesNamed(symbol, FullSymbolName.ForType<T>(), matchingLevel);
    }
    public static IEnumerable<AttributeData> GetAttributesNamedFully<T>(this ISymbol symbol, Type attributeType, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
    {
        return GetAttributesNamed(symbol, FullSymbolName.ForType(attributeType), matchingLevel);
    }

    private static Func<AttributeData, bool> AttributeNameMatcher(string name)
    {
        return attribute => attribute.AttributeClass?.Name == name;
    }
    private static Func<AttributeData, bool> AttributeNameMatcher(FullSymbolName fullName, SymbolNameMatchingLevel matchingLevel)
    {
        return MatchesFullName;
        
        bool MatchesFullName(AttributeData attribute)
        {
            return attribute.AttributeClass?.GetFullSymbolName()!.Matches(fullName, matchingLevel) is true;
        }
    }
    #endregion

    /// <summary>Gets the type of a symbol.</summary>
    /// <param name="symbol">The symbol whose type to get.</param>
    /// <returns>
    /// The declared type of the symbol if it is any of the following:
    /// <list type="bullet">
    /// <item><seealso cref="IFieldSymbol"/></item>
    /// <item><seealso cref="IPropertySymbol"/></item>
    /// <item><seealso cref="IEventSymbol"/></item>
    /// <item><seealso cref="ILocalSymbol"/></item>
    /// <item><seealso cref="IParameterSymbol"/></item>
    /// </list>
    /// -or- the return type of the <seealso cref="IMethodSymbol"/>
    /// -or- the symbol itself if it's an <seealso cref="ITypeSymbol"/>.
    /// </returns>
    public static ITypeSymbol? GetSymbolType(this ISymbol? symbol)
    {
        return symbol switch
        {
            IFieldSymbol f => f.Type,
            IPropertySymbol p => p.Type,
            IEventSymbol e => e.Type,
            ILocalSymbol l => l.Type,
            IParameterSymbol pa => pa.Type,
            IMethodSymbol m => m.ReturnType,

            ITypeSymbol t => t,
            _ => null,
        };
    }

    /// <summary>
    /// Determines whether the symbol is a field or a property symbol.
    /// </summary>
    /// <param name="symbol">The symbol to check for its type.</param>
    /// <returns>
    /// <see langword="true"/> if the symbol instance inherits <seealso cref="IFieldSymbol"/>
    /// or <seealso cref="IPropertySymbol"/>, otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsFieldOrProperty(this ISymbol? symbol)
    {
        return symbol
            is IFieldSymbol
            or IPropertySymbol;
    }
}
