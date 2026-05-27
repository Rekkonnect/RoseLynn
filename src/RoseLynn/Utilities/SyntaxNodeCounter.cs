using Microsoft.CodeAnalysis;

namespace RoseLynn.Utilities;

/// <summary>Counts the total number of <see cref="SyntaxNode"/> instances within a syntax tree, including structured trivia.</summary>
public sealed class SyntaxNodeCounter : SyntaxWalker
{
    private long _counter = 0;

    /// <summary>Gets the total number of nodes visited so far.</summary>
    public long CountedNodes => _counter;

    private SyntaxNodeCounter()
        : base(SyntaxWalkerDepth.StructuredTrivia)
    {
    }

    /// <inheritdoc/>
    public override void Visit(SyntaxNode node)
    {
        _counter++;
        base.Visit(node);
    }

    /// <summary>Counts all syntax nodes in <paramref name="compilationUnit"/>, including structured trivia descendants.</summary>
    /// <param name="compilationUnit">The root node to walk.</param>
    /// <returns>The total number of <see cref="SyntaxNode"/> instances in the tree.</returns>
    public static long CountNodes(SyntaxNode compilationUnit)
    {
        var counter = new SyntaxNodeCounter();
        counter.Visit(compilationUnit);
        return counter.CountedNodes;
    }
}
