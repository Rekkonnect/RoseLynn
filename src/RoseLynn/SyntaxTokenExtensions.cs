using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn;

public static class SyntaxTokenExtensions
{
    public static bool IsFirstIn(this SyntaxToken token, SyntaxTokenList list)
    {
        if (list.Count is 0)
        {
            return false;
        }

        return list.First().SpanStart == token.SpanStart;
    }

    public static bool IsLastIn(this SyntaxToken token, SyntaxTokenList list)
    {
        if (list.Count is 0)
        {
            return false;
        }

        return list.Last().SpanStart == token.SpanStart;
    }

    public static bool HasKind(this SyntaxTokenList list, SyntaxKind kind)
    {
        foreach (var token in list)
        {
            if (token.IsKind(kind))
            {
                return true;
            }
        }
        return false;
    }

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

    public static SyntaxToken AppendLeadingTrivia(
        this SyntaxToken token,
        SyntaxTriviaList append)
    {
        return token.WithLeadingTrivia(
            token.LeadingTrivia.AddRange(append));
    }

    public static SyntaxToken PrependTrailingTrivia(
        this SyntaxToken token,
        SyntaxTriviaList prepend)
    {
        return token.WithTrailingTrivia(
            token.TrailingTrivia.Prepend(prepend));
    }
}
