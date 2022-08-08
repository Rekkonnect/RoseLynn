using System.Collections.Generic;

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

    /// <summary>Provides a comparer that defines the order of two <see cref="UsingDirectiveStatementInfo"/> instances, based on how the IDE behaves.</summary>
    /// <remarks>
    /// The ordering is the following:
    /// <list type="bullet">
    /// <item><see langword="global"/> over non-<see langword="global"/></item>
    /// <item>Non-<see langword="static"/> over <see langword="static"/></item>
    /// <item>Importing usings over alias usings</item>
    /// <item>Fully qualified name of the symbol -or- name of the alias, ascending</item>
    /// </list>
    /// </remarks>
    public sealed class SortingComparer : IComparer<UsingDirectiveStatementInfo>
    {
        public static SortingComparer Instance = new();

        private SortingComparer() { }

        public int Compare(UsingDirectiveStatementInfo x, UsingDirectiveStatementInfo y)
        {
            int comparison = y.IsGlobal.CompareTo(x.IsGlobal);
            if (comparison is not 0)
                return comparison;

            comparison = x.DirectiveKind.CompareTo(y.DirectiveKind);
            if (comparison is not 0)
                return comparison;

            comparison = y.IsStatic.CompareTo(x.IsStatic);
            if (comparison is not 0)
                return comparison;

            return x.IdentifierSortKey.CompareTo(y.IdentifierSortKey);
        }
    }
}
