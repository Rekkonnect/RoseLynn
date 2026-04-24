using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace RoseLynn.CSharp.Syntax;

#nullable enable

/// <summary>Contains extension methods for the <seealso cref="NameSyntax"/> class.</summary>
public static class NameSyntaxExtensions
{
    /// <summary>Gets the rightmost identifier in the provided <seealso cref="NameSyntax"/>.</summary>
    /// <param name="nameSyntax">The <see cref="NameSyntax"/> whose rightmost identifier to get.</param>
    /// <returns>The rightmost identifier.</returns>
    public static SyntaxToken GetRightmostIdentifier(this NameSyntax nameSyntax)
    {
        return nameSyntax.GetRightmostNameSyntax().Identifier;
    }
    /// <summary>Gets the leftmost identifier in the provided <seealso cref="NameSyntax"/>.</summary>
    /// <param name="nameSyntax">The <see cref="NameSyntax"/> whose leftmost identifier to get.</param>
    /// <returns>The leftmost identifier.</returns>
    public static SyntaxToken GetLeftmostIdentifier(this NameSyntax nameSyntax)
    {
        return nameSyntax.GetLeftmostNameSyntax().Identifier;
    }

    /// <summary>Gets the rightmost <seealso cref="NameSyntax"/> in the provided <seealso cref="NameSyntax"/>.</summary>
    /// <param name="nameSyntax">The <see cref="NameSyntax"/> whose rightmost <seealso cref="NameSyntax"/> to get.</param>
    /// <returns>The rightmost <seealso cref="NameSyntax"/>.</returns>
    public static SimpleNameSyntax GetRightmostNameSyntax(this NameSyntax nameSyntax)
    {
        return nameSyntax switch
        {
            QualifiedNameSyntax qualifiedNameSyntax => qualifiedNameSyntax.Right,
            AliasQualifiedNameSyntax aliasQualifiedNameSyntax => aliasQualifiedNameSyntax.Name,

            _ => GetSelfAsParentNameSyntaxOrThrow(nameSyntax),
        };
    }
    /// <summary>Gets the leftmost <seealso cref="NameSyntax"/> in the provided <seealso cref="NameSyntax"/>.</summary>
    /// <param name="nameSyntax">The <see cref="NameSyntax"/> whose leftmost <seealso cref="NameSyntax"/> to get.</param>
    /// <returns>The leftmost <seealso cref="NameSyntax"/>.</returns>
    public static SimpleNameSyntax GetLeftmostNameSyntax(this NameSyntax nameSyntax)
    {
        return nameSyntax switch
        {
            QualifiedNameSyntax qualifiedNameSyntax => qualifiedNameSyntax.Left.GetLeftmostNameSyntax(),
            AliasQualifiedNameSyntax aliasQualifiedNameSyntax => aliasQualifiedNameSyntax.Alias,

            _ => GetSelfAsParentNameSyntaxOrThrow(nameSyntax),
        };
    }

    private static SimpleNameSyntax GetSelfAsParentNameSyntaxOrThrow(NameSyntax nameSyntax)
    {
        return GetSelfAsParentNameSyntax(nameSyntax)
            ?? throw new ArgumentException("The provided name syntax does not represent a simple name.");
    }
    private static SimpleNameSyntax? GetSelfAsParentNameSyntax(NameSyntax nameSyntax)
    {
        return nameSyntax as SimpleNameSyntax;
    }
}
