using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Contains fundamental information about the using directive.</summary>
/// <param name="Kind">The using kind of the using directive.</param>
/// <param name="Alias">The alias of the type that is being created. May be <see langword="null"/> if the using directive is not an alias declaration.</param>
/// <param name="QualifiedName">The qualified name of the symbol.</param>
public record UsingDirectiveStatementInfo(UsingDirectiveKind Kind, string Alias, string QualifiedName)
{
    public bool IsGlobal => Kind.IsGlobal();
    public bool IsStatic => Kind.IsStatic();
    public UsingDirectiveKind DirectiveKind => Kind.GetDirectiveKind();

    public string IdentifierSortKey
    {
        get
        {
            if (DirectiveKind is UsingDirectiveKind.UsingAlias)
                return Alias;

            return QualifiedName;
        }
    }

    public UsingDirectiveStatementInfo(UsingDirectiveKind kind, string qualifiedName)
        : this(kind, null, qualifiedName) { }

    public override string ToString()
    {
        var usingPart = "using";

        if (Kind.HasFlag(UsingDirectiveKind.Static))
            usingPart = $"{usingPart} static";

        if (Kind.HasFlag(UsingDirectiveKind.Global))
            usingPart = $"global {usingPart}";

        var directiveKind = Kind.GetDirectiveKind();

        string intermediate = null;
        if (directiveKind is UsingDirectiveKind.UsingAlias)
            intermediate = $"{Alias} = ";

        return $"{usingPart} {intermediate}{QualifiedName};";
    }

    public static UsingDirectiveStatementInfo LocalAlias(string alias, string qualifiedName) => new(UsingDirectiveKind.UsingAlias, alias, qualifiedName);
    public static UsingDirectiveStatementInfo GlobalAlias(string alias, string qualifiedName) => new(UsingDirectiveKind.GlobalUsingAlias, alias, qualifiedName);

    /// <summary>
    /// Creates a <seealso cref="UsingDirectiveStatementInfo"/> reflecting a <see langword="using"/> directive on
    /// the namespace reflected by the provided <seealso cref="FullSymbolName"/>.
    /// </summary>
    /// <param name="namespaceSymbolName">The full name of the namespace symbol for which to create a <see langword="using"/> directive.</param>
    /// <param name="isGlobal">Determines whether the <see langword="using"/> directive will be <see langword="global"/> or not.</param>
    /// <returns>The created <seealso cref="UsingDirectiveStatementInfo"/> reflecting a <see langword="using"/> directive statement.</returns>
    /// <remarks>
    /// This assumes that <paramref name="namespaceSymbolName"/> reflects the name of a namespace symbol.
    /// To create a <seealso cref="UsingDirectiveStatementInfo"/> for a non-namespace symbol's containing namespace,
    /// use <seealso cref="UsingForSymbol(FullSymbolName, bool)"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="namespaceSymbolName"/> does not reflect the name of a namespace symbol.</exception>
    public static UsingDirectiveStatementInfo UsingForNamespaceSymbol(FullSymbolName namespaceSymbolName, bool isGlobal = false)
    {
        bool isNamespace = namespaceSymbolName.NearestContainerSymbolKind is ContainerSymbolKind.Namespace
                        && namespaceSymbolName.SymbolNameWithArity.Arity is 0;

        if (!isNamespace)
        {
            throw new ArgumentException("The provided symbol name must reflect a namespace symbol.");
        }

        namespaceSymbolName = namespaceSymbolName.CloneWithDefaultContainerSymbolDelimiter();

        return new(UsingDirectiveKind.Using.WithGlobal(isGlobal), namespaceSymbolName.FullNameString);
    }
    /// <summary>
    /// Creates a <seealso cref="UsingDirectiveStatementInfo"/> reflecting a <see langword="using"/> directive on
    /// the containing namespace of the provided <seealso cref="FullSymbolName"/>.
    /// </summary>
    /// <param name="symbolName">The full name of the symbol for which to create a <see langword="using"/> directive.</param>
    /// <param name="isGlobal">Determines whether the <see langword="using"/> directive will be <see langword="global"/> or not.</param>
    /// <returns>The created <seealso cref="UsingDirectiveStatementInfo"/> reflecting a <see langword="using"/> directive statement.</returns>
    /// <remarks>
    /// This assumes that <paramref name="symbolName"/> reflects the name of a non-namespace symbol.
    /// To create a <seealso cref="UsingDirectiveStatementInfo"/> for a namespace symbol,
    /// use <seealso cref="UsingForNamespaceSymbol(FullSymbolName, bool)"/>.
    /// </remarks>
    public static UsingDirectiveStatementInfo UsingForSymbol(FullSymbolName symbolName, bool isGlobal = false)
    {
        symbolName = symbolName.CloneWithDefaultContainerSymbolDelimiter();

        return new(UsingDirectiveKind.Using.WithGlobal(isGlobal), symbolName.FullNamespaceString);
    }

    /// <summary>
    /// Creates a <seealso cref="UsingDirectiveStatementInfo"/> reflecting a <see langword="using static"/> directive on
    /// the type reflected by the provided <seealso cref="FullSymbolName"/>.
    /// </summary>
    /// <param name="typeSymbolName">The full name of the type symbol for which to create a <see langword="using"/> directive.</param>
    /// <param name="typeArguments">The type arugments of the type. <see langword="null"/> will be considered as an empty array.</param>
    /// <param name="isGlobal">Determines whether the <see langword="using"/> directive will be <see langword="global"/> or not.</param>
    /// <returns>The created <seealso cref="UsingDirectiveStatementInfo"/> reflecting a <see langword="using static"/> directive statement.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="typeSymbolName"/> does not reflect the name of a type symbol.</exception>
    public static UsingDirectiveStatementInfo UsingStaticForTypeSymbol(FullSymbolName typeSymbolName, string[] typeArguments = null, bool isGlobal = false)
    {
        if (typeSymbolName.NearestContainerSymbolKind is ContainerSymbolKind.Method)
        {
            throw new ArgumentException("The provided symbol name must reflect a type symbol.");
        }

        typeSymbolName = typeSymbolName.CloneWithDefaultContainerSymbolDelimiter();

        typeArguments ??= Array.Empty<string>();
        var argumentedName = typeSymbolName.SymbolNameWithArity.WithTypeArgumentsCSharp(typeArguments);
        var qualifiedName = $"{typeSymbolName.FullNamespaceString}.{typeSymbolName.FullContainerTypeString}.{argumentedName}";

        return new(UsingDirectiveKind.UsingStatic.WithGlobal(isGlobal), qualifiedName);
    }

    /// <summary>
    /// Creates a <seealso cref="UsingDirectiveStatementInfo"/> reflecting an alias <see langword="using"/> directive on
    /// the symbol reflected by the provided <seealso cref="FullSymbolName"/>.
    /// </summary>
    /// <param name="symbolName">The full name of the symbol for which to create an alias <see langword="using"/> directive.</param>
    /// <param name="alias">The alias to be used for the given symbol name.</param>
    /// <param name="typeArguments">The type arugments of the type. <see langword="null"/> will be considered as an empty array.</param>
    /// <param name="isGlobal">Determines whether the alias <see langword="using"/> directive will be <see langword="global"/> or not.</param>
    /// <returns>The created <seealso cref="UsingDirectiveStatementInfo"/> reflecting an alias <see langword="using"/> statement.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="symbolName"/> does not reflect the name of a namespace or type symbol.</exception>
    public static UsingDirectiveStatementInfo UsingAliasForSymbol(FullSymbolName symbolName, string alias, string[] typeArguments = null, bool isGlobal = false)
    {
        if (symbolName.NearestContainerSymbolKind is ContainerSymbolKind.Method)
        {
            throw new ArgumentException("The provided symbol name must reflect a namespace or type symbol.");
        }

        symbolName = symbolName.CloneWithDefaultContainerSymbolDelimiter();

        typeArguments ??= Array.Empty<string>();
        var argumentedName = symbolName.SymbolNameWithArity.WithTypeArgumentsCSharp(typeArguments);
        var qualifiedName = $"{symbolName.FullNamespaceString}.{symbolName.FullContainerTypeString}.{argumentedName}";

        return new(UsingDirectiveKind.UsingAlias.WithGlobal(isGlobal), alias, qualifiedName);
    }

    /// <summary>Creates a <seealso cref="UsingDirectiveStatementInfo"/> factory function for the specified <seealso cref="UsingDirectiveKind"/>.</summary>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> of the usings that the returned factory method will generate.</param>
    /// <returns>
    /// A factory method that creates <seealso cref="UsingDirectiveStatementInfo"/> instances of the specified <seealso cref="UsingDirectiveKind"/>,
    /// given the fully qualified name as a <see langword="string"/>.
    /// </returns>
    public static Func<string, UsingDirectiveStatementInfo> DirectiveFactoryForKind(UsingDirectiveKind kind)
    {
        return qualifiedName => new UsingDirectiveStatementInfo(kind, qualifiedName);
    }

    /// <summary>
    /// Creates a collection of <seealso cref="UsingDirectiveStatementInfo"/> instances of the specified
    /// <seealso cref="UsingDirectiveKind"/>, using the specified fully qualified names.
    /// </summary>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> to apply to all created <seealso cref="UsingDirectiveStatementInfo"/> instances.</param>
    /// <param name="qualifiedNames">The qualified names that will be used in the directives.</param>
    /// <returns>The collection of <seealso cref="UsingDirectiveStatementInfo"/> instances that were created for the specified qualified names.</returns>
    public static IEnumerable<UsingDirectiveStatementInfo> DirectivesOfKind(UsingDirectiveKind kind, IEnumerable<string> qualifiedNames)
    {
        var factory = DirectiveFactoryForKind(kind);
        return qualifiedNames.Select(factory);
    }
    /// <inheritdoc cref="DirectivesOfKind(UsingDirectiveKind, IEnumerable{string})"/>
    public static IEnumerable<UsingDirectiveStatementInfo> DirectivesOfKind(UsingDirectiveKind kind, params string[] qualifiedNames)
    {
        return DirectivesOfKind(kind, (IEnumerable<string>)qualifiedNames);
    }

    /// <summary>Provides a comparer that defines the order of two <see cref="UsingDirectiveStatementInfo"/> instances, based on how the IDE behaves.</summary>
    /// <remarks>
    /// The ordering is the following:
    /// <list type="bullet">
    /// <item><see langword="global"/> over non-<see langword="global"/></item>
    /// <item>Importing usings over alias usings</item>
    /// <item>Non-<see langword="static"/> over <see langword="static"/></item>
    /// <item>Fully qualified name of the symbol -or- name of the alias, ascending</item>
    /// </list>
    /// </remarks>
    public class SortingComparer : IComparer<UsingDirectiveStatementInfo>
    {
        public static SortingComparer Instance = new();

        /// <summary>Gets the comparers to apply to each comparison in the order they are applied.</summary>
        public virtual IComparer<UsingDirectiveStatementInfo>[] ComparerOrder
        {
            get => new IComparer<UsingDirectiveStatementInfo>[]
            {
                GlobalOverLocal.Instance,
                AscendingDirectiveKind.Instance,
                NonStaticOverStatic.Instance,
                AscendingIdentifierSortKey.Instance,
            };
        }

        protected SortingComparer() { }

        public virtual int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
        {
            foreach (var comparer in ComparerOrder)
            {
                int comparison = comparer.Compare(x, y);
                if (comparison is not 0)
                    return comparison;
            }
            return 0;
        }

        // I wish declaring a pattern like this would be simpler
        public sealed class GlobalOverLocal : IComparer<UsingDirectiveStatementInfo>
        {
            public static GlobalOverLocal Instance = new();
            private GlobalOverLocal() { }

            public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return y.IsGlobal.CompareTo(x.IsGlobal);
            }
        }
        public sealed class LocalOverGlobal : IComparer<UsingDirectiveStatementInfo>
        {
            public static LocalOverGlobal Instance = new();
            private LocalOverGlobal() { }

            public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return x.IsGlobal.CompareTo(y.IsGlobal);
            }
        }
        public sealed class AscendingDirectiveKind : IComparer<UsingDirectiveStatementInfo>
        {
            public static AscendingDirectiveKind Instance = new();
            private AscendingDirectiveKind() { }

            public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return x.DirectiveKind.CompareTo(y.DirectiveKind);
            }
        }
        public sealed class DescendingDirectiveKind : IComparer<UsingDirectiveStatementInfo>
        {
            public static DescendingDirectiveKind Instance = new();
            private DescendingDirectiveKind() { }

            public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return y.DirectiveKind.CompareTo(x.DirectiveKind);
            }
        }
        public sealed class StaticOverNonStatic : IComparer<UsingDirectiveStatementInfo>
        {
            public static StaticOverNonStatic Instance = new();
            private StaticOverNonStatic() { }

            public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return y.IsStatic.CompareTo(x.IsStatic);
            }
        }
        public sealed class NonStaticOverStatic : IComparer<UsingDirectiveStatementInfo>
        {
            public static NonStaticOverStatic Instance = new();
            private NonStaticOverStatic() { }

            public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return x.IsStatic.CompareTo(y.IsStatic);
            }
        }
        public sealed class AscendingIdentifierSortKey : IComparer<UsingDirectiveStatementInfo>
        {
            public static AscendingIdentifierSortKey Instance = new();
            private AscendingIdentifierSortKey() { }

            public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return x.IdentifierSortKey.CompareTo(y.IdentifierSortKey);
            }
        }
        public sealed class DescendingIdentifierSortKey : IComparer<UsingDirectiveStatementInfo>
        {
            public static DescendingIdentifierSortKey Instance = new();
            private DescendingIdentifierSortKey() { }

            public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return y.IdentifierSortKey.CompareTo(x.IdentifierSortKey);
            }
        }
    }
}
