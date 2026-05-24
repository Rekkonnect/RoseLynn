using Microsoft.CodeAnalysis;

namespace RoseLynn.Utilities;

public sealed class SyntaxNodeCounter : SyntaxWalker
{
    private long _counter = 0;

    public long CountedNodes => _counter;

    private SyntaxNodeCounter()
        : base(SyntaxWalkerDepth.StructuredTrivia)
    {
    }

    public override void Visit(SyntaxNode node)
    {
        _counter++;
        base.Visit(node);
    }

    public static long CountNodes(SyntaxNode compilationUnit)
    {
        var counter = new SyntaxNodeCounter();
        counter.Visit(compilationUnit);
        return counter.CountedNodes;
    }
}
