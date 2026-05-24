using Microsoft.CodeAnalysis;

namespace RoseLynn.Utilities;

public sealed class SyntaxTokenCounter : SyntaxWalker
{
    private long _counter = 0;

    public long CountedTokens => _counter;

    private SyntaxTokenCounter()
        : base(SyntaxWalkerDepth.StructuredTrivia)
    {
    }

    protected override void VisitToken(SyntaxToken token)
    {
        _counter++;
        base.VisitToken(token);
    }

    public static long CountTokens(SyntaxNode compilationUnit)
    {
        var counter = new SyntaxTokenCounter();
        counter.Visit(compilationUnit);
        return counter.CountedTokens;
    }
}
