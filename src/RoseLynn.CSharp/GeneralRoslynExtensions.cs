using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn.CSharp;

/// <summary>Provides C#-specific general-purpose Roslyn helper extensions.</summary>
public static class GeneralRoslynExtensions
{
    /// <summary>
    /// Constructs a new syntax token, preserving the kind and trivia of the
    /// given token, and adjusting the text to the target value. The value
    /// text is also set to that new text.
    /// </summary>
    /// <param name="token">The token whose text will be replaced.</param>
    /// <param name="text">The new token text and value text.</param>
    /// <returns>A token with the same kind and trivia as <paramref name="token"/>, but with updated text.</returns>
    public static SyntaxToken WithText(this SyntaxToken token, string text)
    {
        if (token.Kind() is SyntaxKind.IdentifierToken)
        {
            return SyntaxFactory.Identifier(
                leading: token.LeadingTrivia,
                contextualKind: SyntaxKind.None,
                text: text,
                valueText: text,
                trailing: token.TrailingTrivia);
        }

        return SyntaxFactory.Token(
            leading: token.LeadingTrivia,
            kind: token.Kind(),
            text: text,
            valueText: text,
            trailing: token.TrailingTrivia);
    }

    /// <summary>
    /// Replaces all descendant tokens, including trivia-descended tokens, that
    /// match the given old text (after trimming), with tokens containing the
    /// new text.
    /// </summary>
    public static SyntaxNode WithReplacedTextTokens(
        this SyntaxNode node,
        string oldText,
        string newText)
    {
        oldText = oldText.Trim();
        var replacedTokens = node
            .DescendantTokens(descendIntoTrivia: true)
            .Where(s => s.Text.AsSpan().Trim().SequenceEqual(oldText));
        return node.ReplaceTokens(replacedTokens, ReplaceToken);

        SyntaxToken ReplaceToken(SyntaxToken oldToken, SyntaxToken newerToken)
        {
            return newerToken.WithText(newText);
        }
    }
}
