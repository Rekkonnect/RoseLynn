using Microsoft.CodeAnalysis;
using RoseLynn.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace RoseLynn;

/// <summary>Represents a full symbol name.</summary>
public sealed class FullSymbolName
{
    public const string DefaultContainerSymbolDelimiter = ".";

    public const string DefaultNamespaceDelimiter = DefaultContainerSymbolDelimiter;
    public const string DefaultContainerTypeDelimiter = "+";
    public const string DefaultContainerMethodDelimiter = "|";

    /// <summary>The list of namespaces that contain the given symbol, ordered in ascending nesting level.</summary>
    public IList<string> Namespaces;
    /// <summary>The list of names of the types that contain the given symbol, ordered in ascending nesting level.</summary>
    public IList<string> ContainerTypes;
    /// <summary>The list of names of the methods that contain the given symbol, ordered in ascending nesting level.</summary>
    public IList<string> ContainerMethods;

    /// <summary>Gets or sets the symbol name.</summary>
    public string SymbolName
    {
        get => SymbolNameWithArity.ToString();
        set => SymbolNameWithArity = new(value);
    }

    /// <summary>Gets or sets the symbol name, including its arity.</summary>
    public IdentifierWithArity SymbolNameWithArity { get; set; }

    /// <summary>Gets or sets the delimiter of the namespaces.</summary>
    public string NamespaceDelimiter { get; set; }
    /// <summary>Gets or sets the delimiter of the names of the container types.</summary>
    public string ContainerTypeDelimiter { get; set; }
    /// <summary>Gets or sets the delimiter of the names of the container methods.</summary>
    public string ContainerMethodDelimiter { get; set; }

    /// <summary>Gets the full namespace string, concatenating all the namespaces together in the given order, using the specified <seealso cref="NamespaceDelimiter"/>.</summary>
    public string FullNamespaceString => Aggregate(Namespaces, NamespaceDelimiter);
    /// <summary>Gets the full container type string, concatenating all the container types' names together in the given order, using the specified <seealso cref="ContainerTypeDelimiter"/>.</summary>
    public string FullContainerTypeString => Aggregate(ContainerTypes, ContainerTypeDelimiter);
    /// <summary>Gets the full container method string, concatenating all the container methods' names together in the given order, using the specified <seealso cref="ContainerMethodDelimiter"/>.</summary>
    public string FullContainerMethodString => Aggregate(ContainerMethods, ContainerMethodDelimiter);

    /// <summary>Gets the full name string, concatenating all the container symbols' names together in the given order, using the respective delimiters, including the <seealso cref="SymbolName"/>.</summary>
    public string FullNameString
    {
        get
        {
            var namespaceString = FullNamespaceString;
            var containerTypeString = FullContainerTypeString;
            var containerMethodString = FullContainerMethodString;

            int identifierLength = namespaceString.Length + containerTypeString.Length + containerMethodString.Length;
            int delimiterLength = NamespaceDelimiter.Length + ContainerTypeDelimiter.Length;

            var builder = new StringBuilder(identifierLength + delimiterLength);

            builder.Append(namespaceString);

            if (containerTypeString.Any())
            {
                if (namespaceString.Any())
                {
                    builder.Append(NamespaceDelimiter);
                }
                builder.Append(containerTypeString);

                // Container methods require parent container types
                if (containerMethodString.Any())
                {
                    builder.Append(ContainerTypeDelimiter);
                    builder.Append(containerMethodString);
                }
            }

            if (!ContainedDirectlyIntoGlobalNamespace)
            {
                var delimiter = GetDelimiter(NearestContainerSymbolKind);
                builder.Append(delimiter);
            }

            builder.Append(SymbolName);

            return builder.ToString();
        }
    }

    /// <summary>Determines whether the symbol is contained in the <see langword="global"/> namespace.</summary>
    public bool ContainedDirectlyIntoGlobalNamespace => NearestDeclaredContainerSymbolKind is null;

    /// <summary>Gets the <seealso cref="ContainerSymbolKind"/> of the nearest container symbol.</summary>
    /// <remarks>If the symbol is contained in the <see langword="global"/> namespace, the container symbol is considered the <see langword="global"/> namespace itself, returning <seealso cref="ContainerSymbolKind.Namespace"/>.</remarks>
    public ContainerSymbolKind NearestContainerSymbolKind => NearestDeclaredContainerSymbolKind ?? ContainerSymbolKind.Namespace;
    /// <summary>Gets the <seealso cref="ContainerSymbolKind"/> of the nearest container symbol whose name is declared in this instance.</summary>
    /// <remarks>If the symbol is contained in the <see langword="global"/> namespace, <see langword="null"/> is returned.</remarks>
    public ContainerSymbolKind? NearestDeclaredContainerSymbolKind
    {
        get
        {
            if (ContainerMethods.Any())
                return ContainerSymbolKind.Method;

            if (ContainerTypes.Any())
                return ContainerSymbolKind.Type;

            if (Namespaces.Any())
                return ContainerSymbolKind.Namespace;

            return null;
        }
    }

    // Redundant documentation here
    public FullSymbolName(string symbolName)
        : this(symbolName, Enumerable.Empty<string>(), null, null) { }
    public FullSymbolName(string symbolName, IEnumerable<string>? namespaceNames)
        : this(symbolName, namespaceNames, null, null) { }
    public FullSymbolName(string symbolName, IEnumerable<string>? namespaceNames, IEnumerable<string>? containerTypeNames)
        : this(symbolName, namespaceNames, containerTypeNames, null) { }
    public FullSymbolName(string symbolName, IEnumerable<string>? namespaceNames, IEnumerable<string>? containerTypeNames, IEnumerable<string>? containerMethodNames)
        : this(IdentifierWithArity.Parse(symbolName), namespaceNames, containerTypeNames, containerMethodNames) { }

    public FullSymbolName(ISymbol symbol, IEnumerable<INamespaceSymbol>? namespaces, IEnumerable<INamedTypeSymbol>? containerTypes, IEnumerable<IMethodSymbol>? containerMethods)
        : this(symbol, namespaces, containerTypes, containerMethods, SymbolNameKind.Normal) { }
    public FullSymbolName(ISymbol symbol, IEnumerable<INamespaceSymbol>? namespaces, IEnumerable<INamedTypeSymbol>? containerTypes, IEnumerable<IMethodSymbol>? containerMethods, SymbolNameKind symbolNameKind)
        : this(symbol.GetIdentifierWithArity(symbolNameKind), namespaces?.Where(n => !n.IsGlobalNamespace).SelectSymbolNames(symbolNameKind), containerTypes?.SelectSymbolNames(symbolNameKind), containerMethods?.SelectSymbolNames(symbolNameKind)) { }

    public FullSymbolName(IdentifierWithArity symbolNameWithArity)
        : this(symbolNameWithArity, Enumerable.Empty<string>(), null, null) { }
    public FullSymbolName(IdentifierWithArity symbolNameWithArity, IEnumerable<string>? namespaceNames)
        : this(symbolNameWithArity, namespaceNames, null, null) { }
    public FullSymbolName(IdentifierWithArity symbolNameWithArity, IEnumerable<string>? namespaceNames, IEnumerable<string>? containerTypeNames)
        : this(symbolNameWithArity, namespaceNames, containerTypeNames, null) { }
    public FullSymbolName(IdentifierWithArity symbolNameWithArity, IEnumerable<string>? namespaceNames, IEnumerable<string>? containerTypeNames, IEnumerable<string>? containerMethodNames)
    {
        SymbolNameWithArity = symbolNameWithArity;
        Namespaces = namespaceNames.ToListOrEmpty();
        ContainerTypes = containerTypeNames.ToListOrEmpty();
        ContainerMethods = containerMethodNames.ToListOrEmpty();

        ResetAllDelimiters();
    }

    /// <summary>Resets all delimiters to their individual default values.</summary>
    public void ResetAllDelimiters()
    {
        NamespaceDelimiter = DefaultNamespaceDelimiter;
        ContainerTypeDelimiter = DefaultContainerTypeDelimiter;
        ContainerMethodDelimiter = DefaultContainerMethodDelimiter;
    }
    /// <summary>Sets all delimiters to the same delimiter.</summary>
    /// <param name="delimiter">The delimiter to set all delimiter kinds to.</param>
    public void SetAllDelimiters(string delimiter)
    {
        NamespaceDelimiter = delimiter;
        ContainerTypeDelimiter = delimiter;
        ContainerMethodDelimiter = delimiter;
    }
    /// <summary>Sets all delimiters to <seealso cref="DefaultContainerSymbolDelimiter"/>.</summary>
    public void UseDefaultContainerSymbolDelimiter()
    {
        SetAllDelimiters(DefaultContainerSymbolDelimiter);
    }

    /// <summary>Gets the delimiter for the given <seealso cref="SymbolKind"/>.</summary>
    /// <param name="containerSymbolKind">The <seealso cref="SymbolKind"/> of the containers whose delimiter to get.</param>
    /// <returns>The delimiter for the given <seealso cref="SymbolKind"/>.</returns>
    public string GetDelimiter(SymbolKind containerSymbolKind)
    {
        return GetDelimiter(containerSymbolKind.ToContainerSymbolKind());
    }
    /// <summary>Gets the delimiter for the given <seealso cref="ContainerSymbolKind"/>.</summary>
    /// <param name="containerSymbolKind">The <seealso cref="ContainerSymbolKind"/> of the containers whose delimiter to get.</param>
    /// <returns>The delimiter for the given <seealso cref="ContainerSymbolKind"/>.</returns>
    public string GetDelimiter(ContainerSymbolKind containerSymbolKind)
    {
        return containerSymbolKind switch
        {
            ContainerSymbolKind.Namespace => NamespaceDelimiter,
            ContainerSymbolKind.Type => ContainerTypeDelimiter,
            ContainerSymbolKind.Method => ContainerMethodDelimiter,
        };
    }

    /// <summary>Gets the name list for the containers of the given <seealso cref="SymbolKind"/>.</summary>
    /// <param name="containerSymbolKind">The <seealso cref="SymbolKind"/> of the containers whose list of names to get.</param>
    /// <returns>The name list for the containers of the given <seealso cref="SymbolKind"/>.</returns>
    public IList<string> GetNameList(SymbolKind containerSymbolKind)
    {
        return GetNameList(containerSymbolKind.ToContainerSymbolKind());
    }
    /// <summary>Gets the name list for the containers of the given <seealso cref="ContainerSymbolKind"/>.</summary>
    /// <param name="containerSymbolKind">The <seealso cref="ContainerSymbolKind"/> of the containers whose list of names to get.</param>
    /// <returns>The name list for the containers of the given <seealso cref="ContainerSymbolKind"/>.</returns>
    public IList<string> GetNameList(ContainerSymbolKind containerSymbolKind)
    {
        return containerSymbolKind switch
        {
            ContainerSymbolKind.Namespace => Namespaces,
            ContainerSymbolKind.Type => ContainerTypes,
            ContainerSymbolKind.Method => ContainerMethods,
        };
    }

    public bool MatchesContainerMethods(FullSymbolName other)
    {
        return MatchesNameList(other, ContainerSymbolKind.Method);
    }
    public bool MatchesContainerTypes(FullSymbolName other)
    {
        return MatchesNameList(other, ContainerSymbolKind.Type);
    }
    public bool MatchesNamespaces(FullSymbolName other)
    {
        return MatchesNameList(other, ContainerSymbolKind.Namespace);
    }
    public bool MatchesNameList(FullSymbolName other, SymbolKind containerSymbolKind)
    {
        return MatchesNameList(other, containerSymbolKind.ToContainerSymbolKind());
    }
    public bool MatchesNameList(FullSymbolName other, ContainerSymbolKind containerSymbolKind)
    {
        var thisList = GetNameList(containerSymbolKind);
        var otherList = other.GetNameList(containerSymbolKind);
        return thisList.SequenceEqual(otherList);
    }

    /// <summary>Determines whether this full symbol name matches another, up to the specified <seealso cref="SymbolNameMatchingLevel"/>.</summary>
    /// <param name="other">The other <seealso cref="FullSymbolName"/> to match.</param>
    /// <param name="matchingLevel">
    /// The outermost containers whose names to match.
    /// <seealso cref="SymbolNameMatchingLevel.Assembly"/> and <seealso cref="SymbolNameMatchingLevel.Module"/> are treated as <seealso cref="SymbolNameMatchingLevel.Namespace"/>.
    /// </param>
    /// <returns><see langword="true"/> if the full symbol names match up until the specified <seealso cref="SymbolNameMatchingLevel"/>, otherwise <see langword="false"/>.</returns>
    public bool Matches(FullSymbolName? other, SymbolNameMatchingLevel matchingLevel)
    {
        if (other is null)
            return false;

        switch (matchingLevel)
        {
            case SymbolNameMatchingLevel.Assembly:
            case SymbolNameMatchingLevel.Module:
            case SymbolNameMatchingLevel.Namespace:
                if (!MatchesNamespaces(other))
                    return false;

                goto case SymbolNameMatchingLevel.ContainerType;

            case SymbolNameMatchingLevel.ContainerType:
                if (!MatchesContainerTypes(other))
                    return false;

                goto case SymbolNameMatchingLevel.ContainerMethod;

            case SymbolNameMatchingLevel.ContainerMethod:
                if (!MatchesContainerMethods(other))
                    return false;

                goto case SymbolNameMatchingLevel.SymbolName;

            case SymbolNameMatchingLevel.SymbolName:
                return SymbolNameWithArity == other.SymbolNameWithArity;
        }

        return false;
    }

    private static string Aggregate(IEnumerable<string> source, string delimiter)
    {
        if (!source.Any())
            return string.Empty;

        return source.Aggregate(Aggregator);

        string Aggregator(string left, string right) => $"{left}{delimiter}{right}";
    }
}
