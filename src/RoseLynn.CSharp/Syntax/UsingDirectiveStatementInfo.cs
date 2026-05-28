using RoseLynn.Core;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Contains fundamental information about the using directive.</summary>
/// <param name="Kind">The using kind of the using directive.</param>
/// <param name="Alias">The alias of the type that is being created. May be <see langword="null"/> if the using directive is not an alias declaration.</param>
/// <param name="QualifiedName">The qualified name of the symbol.</param>
public record UsingDirectiveStatementInfo(UsingDirectiveKind Kind, string? Alias, string QualifiedName)
{
    /// <summary>Gets whether this using directive is a <see langword="global"/> directive.</summary>
    public bool IsGlobal => Kind.IsGlobal();
    /// <summary>Gets whether this using directive is a <see langword="static"/> directive.</summary>
    public bool IsStatic => Kind.IsStatic();
    /// <summary>Gets the base directive kind, excluding <see langword="global"/> and <see langword="static"/> flags.</summary>
    public UsingDirectiveKind DirectiveKind => Kind.GetDirectiveKind();

    /// <summary>Gets the sort key used for ordering: the alias if one is set, otherwise the qualified name.</summary>
    public string IdentifierSortKey => Alias ?? QualifiedName;

    /// <summary>Initializes a new <seealso cref="UsingDirectiveStatementInfo"/> without an alias.</summary>
    /// <param name="kind">The using kind of the directive.</param>
    /// <param name="qualifiedName">The qualified name of the symbol.</param>
    public UsingDirectiveStatementInfo(UsingDirectiveKind kind, string qualifiedName)
        : this(kind, null, qualifiedName) { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var usingPart = "using";

        if (Kind.HasFlag(UsingDirectiveKind.Static))
            usingPart = $"{usingPart} static";

        if (Kind.HasFlag(UsingDirectiveKind.Global))
            usingPart = $"global {usingPart}";

        var directiveKind = Kind.GetDirectiveKind();

        var intermediate = string.Empty;
        if (directiveKind is UsingDirectiveKind.UsingAlias)
            intermediate = $"{Alias} = ";

        return $"{usingPart} {intermediate}{QualifiedName};";
    }

    /// <summary>Creates a local alias using directive mapping <paramref name="alias"/> to <paramref name="qualifiedName"/>.</summary>
    /// <param name="alias">The alias identifier.</param>
    /// <param name="qualifiedName">The fully qualified symbol name.</param>
    /// <returns>A <see cref="UsingDirectiveStatementInfo"/> of kind <see cref="UsingDirectiveKind.UsingAlias"/>.</returns>
    public static UsingDirectiveStatementInfo LocalAlias(string alias, string qualifiedName) => new(UsingDirectiveKind.UsingAlias, alias, qualifiedName);
    /// <summary>Creates a global alias using directive mapping <paramref name="alias"/> to <paramref name="qualifiedName"/>.</summary>
    /// <param name="alias">The alias identifier.</param>
    /// <param name="qualifiedName">The fully qualified symbol name.</param>
    /// <returns>A <see cref="UsingDirectiveStatementInfo"/> of kind <see cref="UsingDirectiveKind.GlobalUsingAlias"/>.</returns>
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
    public static UsingDirectiveStatementInfo UsingStaticForTypeSymbol(FullSymbolName typeSymbolName, string[]? typeArguments = null, bool isGlobal = false)
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
    public static UsingDirectiveStatementInfo UsingAliasForSymbol(FullSymbolName symbolName, string alias, string[]? typeArguments = null, bool isGlobal = false)
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

    /// <summary>Creates a using statement for the type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type whose information will be used for the creation of the using statement.</typeparam>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> of the resulting using statement.</param>
    /// <returns>A using statement of the specified kind for <typeparamref name="T"/>.</returns>
    public static UsingDirectiveStatementInfo UsingForType<T>(UsingDirectiveKind kind)
    {
        return UsingForType(kind, typeof(T));
    }
    /// <summary>
    /// Creates a using statement for the specified type, which may also include the type's name,
    /// if the using directive kind contains the <seealso cref="UsingDirectiveKind.Static"/> flag.
    /// </summary>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> of the resulting using statement.</param>
    /// <param name="type">The type whose information will be used for the creation of the using statement.</param>
    /// <returns>
    /// A using statement of the specified kind, utilizing the type's information. If the using statement kind has the
    /// <seealso cref="UsingDirectiveKind.Using"/> flag, only the namespace of the type is used, otherwise the type itself
    /// is included, as the using directive kind will contain the <seealso cref="UsingDirectiveKind.UsingStatic"/> flag.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the using directive kind contains the <seealso cref="UsingDirectiveKind.UsingAlias"/> flag.
    /// To create alias declaration statements, use the <seealso cref="AliasForType(UsingDirectiveKind, Type, string)"/> overload.
    /// </exception>
    public static UsingDirectiveStatementInfo UsingForType(UsingDirectiveKind kind, Type type)
    {
        if (kind.HasFlag(UsingDirectiveKind.UsingAlias))
        {
            throw new ArgumentException("An alias must be specified to use a using alias statement.");
        }

        var qualifiedName = type.Namespace ?? string.Empty;
        if (kind.IsStatic())
        {
            qualifiedName += $".{type.Name}";
        }
        return new(kind, qualifiedName);
    }

    /// <summary>Creates an alias using statement for the type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type for which to create the alias using statement.</typeparam>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> of the resulting using alias statement.</param>
    /// <param name="alias">The alias to assign to the type.</param>
    /// <returns>A using alias statement for <typeparamref name="T"/> with the specified alias.</returns>
    public static UsingDirectiveStatementInfo AliasForType<T>(UsingDirectiveKind kind, string alias)
    {
        return AliasForType(kind, typeof(T), alias);
    }
    /// <summary>Creates an alias using statement for the specified type.</summary>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> of the resulting using alias statement.</param>
    /// <param name="type">The type for which to create the alias using statement.</param>
    /// <param name="alias">The alias to assign to the type.</param>
    /// <returns>A using alias statement for the specified type with the given alias.</returns>
    public static UsingDirectiveStatementInfo AliasForType(UsingDirectiveKind kind, Type type, string alias)
    {
        if (!kind.HasFlag(UsingDirectiveKind.UsingAlias))
        {
            throw new ArgumentException("The using directive kind must be a using alias.");
        }

        var qualifiedName = GetQualifiedNameForType(type);
        return new(kind, alias, qualifiedName);
    }

    private static string GetQualifiedNameForType(Type type)
    {
        // For now just handle the null full name with an empty string, in the future we should be able to type it better
        return type.FullName ?? string.Empty;
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
    /// <inheritdoc cref="SortingComparer"/>
    public class SortingComparer : IComparer<UsingDirectiveStatementInfo>
    {
        /// <summary>Gets the shared singleton instance of <seealso cref="SortingComparer"/>.</summary>
        public static SortingComparer Instance = new();

        /// <summary>Gets the comparers to apply to each comparison in the order they are applied.</summary>
        public virtual BaseComparer[] ComparerOrder
        {
            get =>
            [
                GlobalOverLocal.Instance,
                AscendingDirectiveKind.Instance,
                NonStaticOverStatic.Instance,
                AscendingIdentifierSortKey.Instance,
            ];
        }

        /// <summary>Initializes a new instance of <see cref="SortingComparer"/>.</summary>
        protected SortingComparer() { }

        /// <summary>Compares two <seealso cref="UsingDirectiveStatementInfo"/> instances using all registered comparers in order.</summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The second instance.</param>
        /// <returns>A negative, zero, or positive value indicating relative order.</returns>
        public virtual int Compare(UsingDirectiveStatementInfo? x, UsingDirectiveStatementInfo? y)
        {
            foreach (var comparer in ComparerOrder)
            {
                int comparison = comparer.Compare(x, y);
                if (comparison is not 0)
                    return comparison;
            }
            return 0;
        }

        /// <summary>Provides common null-aware comparison behavior for <see cref="UsingDirectiveStatementInfo"/> comparers.</summary>
        public abstract class BaseComparer : IComparer<UsingDirectiveStatementInfo>
        {
            /// <summary>Compares two <see cref="UsingDirectiveStatementInfo"/> instances, handling <see langword="null"/> values before delegating to <see cref="CompareNonNull"/>.</summary>
            /// <param name="x">The first instance.</param>
            /// <param name="y">The second instance.</param>
            /// <returns>A negative, zero, or positive value indicating relative order.</returns>
            public int Compare(UsingDirectiveStatementInfo? x, UsingDirectiveStatementInfo? y)
            {
                if (ReferenceEquals(x, y))
                    return 0;

                if (x is null)
                    return -1;

                if (y is null)
                    return 1;

                return CompareNonNull(x, y);
            }

            /// <summary>Compares two non-<see langword="null"/> using directive statements.</summary>
            protected abstract int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y);
        }

        // I wish declaring a pattern like this would be simpler
        /// <summary>Orders global using directives before local ones.</summary>
        public sealed class GlobalOverLocal : BaseComparer
        {
            /// <summary>Gets the shared singleton instance of <see cref="GlobalOverLocal"/>.</summary>
            public static GlobalOverLocal Instance = new();
            private GlobalOverLocal() { }

            /// <inheritdoc/>
            protected override int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return y.IsGlobal.CompareTo(x.IsGlobal);
            }
        }
        /// <summary>Orders local using directives before global ones.</summary>
        public sealed class LocalOverGlobal : BaseComparer
        {
            /// <summary>Gets the shared singleton instance of <see cref="LocalOverGlobal"/>.</summary>
            public static LocalOverGlobal Instance = new();
            private LocalOverGlobal() { }

            /// <inheritdoc/>
            protected override int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return x.IsGlobal.CompareTo(y.IsGlobal);
            }
        }
        /// <summary>Orders using directives by ascending directive kind.</summary>
        public sealed class AscendingDirectiveKind : BaseComparer
        {
            /// <summary>Gets the shared singleton instance of <see cref="AscendingDirectiveKind"/>.</summary>
            public static AscendingDirectiveKind Instance = new();
            private AscendingDirectiveKind() { }

            /// <inheritdoc/>
            protected override int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return x.DirectiveKind.CompareTo(y.DirectiveKind);
            }
        }
        /// <summary>Orders using directives by descending directive kind.</summary>
        public sealed class DescendingDirectiveKind : BaseComparer
        {
            /// <summary>Gets the shared singleton instance of <see cref="DescendingDirectiveKind"/>.</summary>
            public static DescendingDirectiveKind Instance = new();
            private DescendingDirectiveKind() { }

            /// <inheritdoc/>
            protected override int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return y.DirectiveKind.CompareTo(x.DirectiveKind);
            }
        }
        /// <summary>Orders static using directives before non-static ones.</summary>
        public sealed class StaticOverNonStatic : BaseComparer
        {
            /// <summary>Gets the shared singleton instance of <see cref="StaticOverNonStatic"/>.</summary>
            public static StaticOverNonStatic Instance = new();
            private StaticOverNonStatic() { }

            /// <inheritdoc/>
            protected override int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return y.IsStatic.CompareTo(x.IsStatic);
            }
        }
        /// <summary>Orders non-static using directives before static ones.</summary>
        public sealed class NonStaticOverStatic : BaseComparer
        {
            /// <summary>Gets the shared singleton instance of <see cref="NonStaticOverStatic"/>.</summary>
            public static NonStaticOverStatic Instance = new();
            private NonStaticOverStatic() { }

            /// <inheritdoc/>
            protected override int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return x.IsStatic.CompareTo(y.IsStatic);
            }
        }
        /// <summary>Orders using directives by ascending identifier sort key.</summary>
        public sealed class AscendingIdentifierSortKey : BaseComparer
        {
            /// <summary>Gets the shared singleton instance of <see cref="AscendingIdentifierSortKey"/>.</summary>
            public static AscendingIdentifierSortKey Instance = new();
            private AscendingIdentifierSortKey() { }

            /// <inheritdoc/>
            protected override int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return string.Compare(x.IdentifierSortKey, y.IdentifierSortKey, StringComparison.Ordinal);
            }
        }
        /// <summary>Orders using directives by descending identifier sort key.</summary>
        public sealed class DescendingIdentifierSortKey : BaseComparer
        {
            /// <summary>Gets the shared singleton instance of <see cref="DescendingIdentifierSortKey"/>.</summary>
            public static DescendingIdentifierSortKey Instance = new();
            private DescendingIdentifierSortKey() { }

            /// <inheritdoc/>
            protected override int CompareNonNull(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
            {
                return string.Compare(y.IdentifierSortKey, x.IdentifierSortKey, StringComparison.Ordinal);
            }
        }
    }
}
