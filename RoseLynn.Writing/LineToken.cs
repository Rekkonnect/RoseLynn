using System;

namespace RoseLynn.Writing;

/// <summary>Represents a tokenized segment of text returned while enumerating lines.</summary>
/// <param name="span">The span covered by the token.</param>
/// <param name="kind">The kind of token represented by <paramref name="span"/>.</param>
public readonly ref struct LineToken(
    ReadOnlySpan<char> span,
    LineToken.TokenKind kind)
{
    /// <summary>Gets the span covered by the token.</summary>
    public ReadOnlySpan<char> Span { get; } = span;
    /// <summary>Gets the classification of the token.</summary>
    public TokenKind Kind { get; } = kind;

    /// <summary>Identifies the kind of token produced while splitting text into lines.</summary>
    public enum TokenKind
    {
        /// <summary>No token has been assigned.</summary>
        None,
        /// <summary>The token represents a newline sequence.</summary>
        NewLine,
        /// <summary>The token represents non-newline text content.</summary>
        String,
    }
}

