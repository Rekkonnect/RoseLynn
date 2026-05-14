#nullable enable

using Microsoft.CodeAnalysis;

namespace RoseLynn;

// TODO: Place in the .CSharp version of RoseLynn
/// <summary>Provides syntax helpers that work across multiple Roslyn argument node shapes.</summary>
public static class CrossSyntaxFactsEx
{
    /// <summary>Gets the colon-based argument name from a syntax node when one exists.</summary>
    /// <typeparam name="T">The syntax node type.</typeparam>
    /// <param name="argumentSyntax">The argument syntax to inspect.</param>
    /// <returns>The colon-based argument name, or <see langword="null"/> if none is present.</returns>
    public static string? GetArgumentName<T>(T argumentSyntax)
        where T : SyntaxNode
    {
        return argumentSyntax switch
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax csArgument
                => csArgument.NameColon?.Name?.Identifier.Text,

            Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax csArgument
                => csArgument.NameColon?.Name?.Identifier.Text,

            _ => null,
        };
    }

    /// <summary>Gets the member name from an attribute named argument using equals syntax.</summary>
    /// <typeparam name="T">The syntax node type.</typeparam>
    /// <param name="argumentSyntax">The argument syntax to inspect.</param>
    /// <returns>The member name for a named attribute argument, or <see langword="null"/> if none is present.</returns>
    public static string? GetNamedArgumentMemberName<T>(T argumentSyntax)
        where T : SyntaxNode
    {
        return argumentSyntax switch
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax csArgument
                => csArgument.NameEquals?.Name?.Identifier.Text,

            _ => null,
        };
    }
}
