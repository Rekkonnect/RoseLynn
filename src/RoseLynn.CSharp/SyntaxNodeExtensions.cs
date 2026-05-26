using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn.CSharp;

/// <summary>Provides C#-specific extensions for <seealso cref="SyntaxNode"/>.</summary>
public static class SyntaxNodeExtensions
{
    extension<TNode>(TNode node) where TNode : SyntaxNode
    {
        /// <summary>Returns the node with a single elastic space as its leading trivia.</summary>
        public TNode WithElasticLeadingTrivia()
        {
            return node.WithLeadingTrivia(SyntaxFactory.ElasticSpace);
        }

        /// <summary>Returns the node with a single elastic space as its trailing trivia.</summary>
        public TNode WithElasticTrailingTrivia()
        {
            return node.WithTrailingTrivia(SyntaxFactory.ElasticSpace);
        }
    }
}
