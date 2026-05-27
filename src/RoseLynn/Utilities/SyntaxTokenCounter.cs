using Microsoft.CodeAnalysis;

namespace RoseLynn.Utilities;

/// <summary>Counts the total number of <see cref="SyntaxToken"/> instances within a syntax tree, including structured trivia.</summary>
public sealed class SyntaxTokenCounter : SyntaxWalker
{
    private long _counter = 0;

    /// <summary>Gets the total number of tokens visited so far.</summary>
    public long CountedTokens => _counter;

    private SyntaxTokenCounter()
        : base(SyntaxWalkerDepth.StructuredTrivia)
    {
    }

    /// <inheritdoc/>
    protected override void VisitToken(SyntaxToken token)
    {
        _counter++;
        base.VisitToken(token);
    }

    /// <summary>Counts all syntax tokens in <paramref name="compilationUnit"/>, including structured trivia descendants.</summary>
    /// <param name="compilationUnit">The root node to walk.</param>
    /// <returns>The total number of <see cref="SyntaxToken"/> instances in the tree.</returns>
    public static long CountTokens(SyntaxNode compilationUnit)
    {
        var counter = new SyntaxTokenCounter();
        counter.Visit(compilationUnit);
        return counter.CountedTokens;
    }
}
