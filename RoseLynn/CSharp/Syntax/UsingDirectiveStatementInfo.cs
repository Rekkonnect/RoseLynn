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

        private SortingComparer() { }

        public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
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
