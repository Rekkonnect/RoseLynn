using Microsoft.CodeAnalysis;
using System;

namespace RoseLynn.Writing;

/// <summary>Enumerates a string span as alternating text and newline tokens.</summary>
/// <param name="source">The source text to enumerate.</param>
public ref struct StringLineTokenEnumerator(ReadOnlySpan<char> source)
{
    private ReadOnlySpan<char> _remaining = source;
    private LineToken _current = default;

    /// <summary>Gets the current token produced by the enumerator.</summary>
    public readonly LineToken Current => _current;

    /// <summary>Advances to the next token.</summary>
    /// <returns><see langword="true"/> if a token was produced; otherwise, <see langword="false"/>.</returns>
    public bool MoveNext()
    {
        if (_remaining is [])
        {
            return false;
        }

        Consume();
        return true;
    }

    private void Consume()
    {
        if (TryConsumeNewLine(out var newLineToken))
        {
            _current = newLineToken;
            return;
        }

        int nextNewLine = _remaining.IndexOfAny('\r', '\n');
        if (nextNewLine < 0)
        {
            _current = new(_remaining, LineToken.TokenKind.String);
            _remaining = [];
            return;
        }

        var @string = _remaining[..nextNewLine];
        _current = new(@string, LineToken.TokenKind.String);
        var nextRemaining = _remaining[nextNewLine..];
        _remaining = nextRemaining;
    }

    private bool TryConsumeNewLine(out LineToken token)
    {
        token = default;
        var firstRemaining = _remaining[0];
        var startsWithNewLine = firstRemaining is '\r' or '\n';
        if (startsWithNewLine)
        {
            var newLineLength = 1;
            if (firstRemaining is '\r'
                && _remaining is [_, '\n', ..])
            {
                newLineLength = 2;
            }

            token = new(
                _remaining[..newLineLength],
                LineToken.TokenKind.NewLine);
            _remaining = _remaining[newLineLength..];
        }

        return startsWithNewLine;
    }

    /// <summary>Resets the current token state.</summary>
    public void Reset()
    {
        _current = default;
    }

    /// <summary>Returns the enumerator instance for foreach-style iteration.</summary>
    /// <returns>The current enumerator.</returns>
    public StringLineTokenEnumerator GetEnumerator()
    {
        return this;
    }
}

