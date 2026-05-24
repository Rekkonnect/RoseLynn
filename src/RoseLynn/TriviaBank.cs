using Microsoft.CodeAnalysis;

namespace RoseLynn;

public readonly record struct TriviaBank(
    SyntaxTriviaList Leading,
    SyntaxTriviaList Trailing)
{
    public bool HasLeading => Leading.Count > 0;
    public bool HasTrailing => Trailing.Count > 0;

    public TNode AppendOnNode<TNode>(TNode node)
        where TNode : SyntaxNode
    {
        return (TNode)node
            .AppendLeadingTrivia(Leading)
            .PrependTrailingTrivia(Trailing)
            ;
    }

    public TNode AppendLeadingOnNode<TNode>(TNode node)
        where TNode : SyntaxNode
    {
        return (TNode)node
            .AppendLeadingTrivia(Leading)
            ;
    }
}
