using Microsoft.CodeAnalysis;
using System;

namespace RoseLynn.Writing;

public ref struct StringLineTokenEnumerator(ReadOnlySpan<char> source)
{
    private ReadOnlySpan<char> _remaining = source;
    private LineToken _current = default;

    public readonly LineToken Current => _current;

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

    public void Reset()
    {
        _current = default;
    }

    public StringLineTokenEnumerator GetEnumerator()
    {
        return this;
    }
}

