using System;

namespace RoseLynn.Writing;

public readonly ref struct LineToken(
    ReadOnlySpan<char> span,
    LineToken.TokenKind kind)
{
    public ReadOnlySpan<char> Span { get; } = span;
    public TokenKind Kind { get; } = kind;

    public enum TokenKind
    {
        None,
        NewLine,
        String,
    }
}

