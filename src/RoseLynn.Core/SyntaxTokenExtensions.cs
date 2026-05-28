using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Provides extension methods for <seealso cref="SyntaxToken"/>.</summary>
public static class SyntaxTokenExtensions
{
    /// <summary>Returns <see langword="true"/> if <paramref name="token"/> is the first token in <paramref name="list"/>.</summary>
    /// <param name="token">The token to test.</param>
    /// <param name="list">The list to check against.</param>
    /// <returns><see langword="true"/> when the token's span start matches the first token in the list.</returns>
    public static bool IsFirstIn(this SyntaxToken token, SyntaxTokenList list)
    {
        if (list.Count is 0)
        {
            return false;
        }

        return list.First().SpanStart == token.SpanStart;
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="token"/> is the last token in <paramref name="list"/>.</summary>
    /// <param name="token">The token to test.</param>
    /// <param name="list">The list to check against.</param>
    /// <returns><see langword="true"/> when the token's span start matches the last token in the list.</returns>
    public static bool IsLastIn(this SyntaxToken token, SyntaxTokenList list)
    {
        if (list.Count is 0)
        {
            return false;
        }

        return list.Last().SpanStart == token.SpanStart;
    }

    /// <summary>Returns the token with leading trivia set to <paramref name="leading"/>, or the original token if <paramref name="leading"/> is <see langword="null"/>.</summary>
    /// <param name="token">The token to modify.</param>
    /// <param name="leading">The leading trivia to apply, or <see langword="null"/> to leave unchanged.</param>
    /// <returns>The token with updated or unchanged leading trivia.</returns>
    public static SyntaxToken WithLeadingTriviaOrSame(
        this SyntaxToken token,
        SyntaxTriviaList? leading)
    {
        if (leading is null)
        {
            return token;
        }

        return token.WithLeadingTrivia(leading.Value);
    }

    /// <summary>Returns the token with trailing trivia set to <paramref name="leading"/>, or the original token if <paramref name="leading"/> is <see langword="null"/>.</summary>
    /// <param name="token">The token to modify.</param>
    /// <param name="leading">The trailing trivia to apply, or <see langword="null"/> to leave unchanged.</param>
    /// <returns>The token with updated or unchanged trailing trivia.</returns>
    public static SyntaxToken WithTrailingTriviaOrSame(
        this SyntaxToken token,
        SyntaxTriviaList? leading)
    {
        if (leading is null)
        {
            return token;
        }

        return token.WithTrailingTrivia(leading.Value);
    }

    /// <summary>Returns the token with <paramref name="append"/> added to the end of its leading trivia.</summary>
    /// <param name="token">The token to modify.</param>
    /// <param name="append">The trivia to append.</param>
    /// <returns>The token with updated leading trivia.</returns>
    public static SyntaxToken AppendLeadingTrivia(
        this SyntaxToken token,
        SyntaxTriviaList append)
    {
        return token.WithLeadingTrivia(
            token.LeadingTrivia.AddRange(append));
    }

    /// <summary>Returns the token with <paramref name="prepend"/> inserted at the start of its trailing trivia.</summary>
    /// <param name="token">The token to modify.</param>
    /// <param name="prepend">The trivia to prepend.</param>
    /// <returns>The token with updated trailing trivia.</returns>
    public static SyntaxToken PrependTrailingTrivia(
        this SyntaxToken token,
        SyntaxTriviaList prepend)
    {
        return token.WithTrailingTrivia(
            token.TrailingTrivia.Prepend(prepend));
    }
}
