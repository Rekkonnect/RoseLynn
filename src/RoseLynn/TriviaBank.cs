using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Holds leading and trailing trivia to be applied to a syntax node in one operation.</summary>
/// <param name="Leading">The leading trivia to prepend.</param>
/// <param name="Trailing">The trailing trivia to append.</param>
public readonly record struct TriviaBank(
    SyntaxTriviaList Leading,
    SyntaxTriviaList Trailing)
{
    /// <summary>Gets a value indicating whether this bank has any leading trivia.</summary>
    public bool HasLeading => Leading.Count > 0;
    /// <summary>Gets a value indicating whether this bank has any trailing trivia.</summary>
    public bool HasTrailing => Trailing.Count > 0;

    /// <summary>Returns <paramref name="node"/> with the bank's leading trivia appended and trailing trivia prepended.</summary>
    /// <typeparam name="TNode">The node type.</typeparam>
    /// <param name="node">The node to modify.</param>
    /// <returns>The node with the bank's trivia applied.</returns>
    public TNode AppendOnNode<TNode>(TNode node)
        where TNode : SyntaxNode
    {
        return (TNode)node
            .AppendLeadingTrivia(Leading)
            .PrependTrailingTrivia(Trailing)
            ;
    }

    /// <summary>Returns <paramref name="node"/> with the bank's leading trivia appended only.</summary>
    /// <typeparam name="TNode">The node type.</typeparam>
    /// <param name="node">The node to modify.</param>
    /// <returns>The node with leading trivia appended.</returns>
    public TNode AppendLeadingOnNode<TNode>(TNode node)
        where TNode : SyntaxNode
    {
        return (TNode)node
            .AppendLeadingTrivia(Leading)
            ;
    }
}
