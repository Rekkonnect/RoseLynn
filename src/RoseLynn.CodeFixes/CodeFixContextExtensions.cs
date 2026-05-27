using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp;

namespace RoseLynn.CodeFixes;

/// <summary>Provides useful extensions for the <seealso cref="CodeFixContext"/> type.</summary>
public static class CodeFixContextExtensions
{
    extension(CodeFixContext context)
    {
        /// <summary>Gets the <see cref="Solution"/> that contains the context's document.</summary>
        public Solution Solution => context.Document.Solution;

        /// <summary>Removes <paramref name="attributeSyntax"/> from the document, cleaning up any leftover trivia.</summary>
        /// <param name="attributeSyntax">The attribute syntax node to remove.</param>
        /// <param name="options">Controls how surrounding trivia is preserved after removal.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated <see cref="Document"/> with the attribute removed.</returns>
        public async Task<Document> RemoveAttributeCleanAsync
        (
            AttributeSyntax attributeSyntax,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeClean(attributeSyntax, options), cancellationToken);

        /// <summary>Removes <paramref name="attributeArgumentSyntax"/> from the document, cleaning up any leftover trivia.</summary>
        /// <param name="attributeArgumentSyntax">The attribute argument syntax node to remove.</param>
        /// <param name="options">Controls how surrounding trivia is preserved after removal.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated <see cref="Document"/> with the attribute argument removed.</returns>
        public async Task<Document> RemoveAttributeArgumentCleanAsync
        (
            AttributeArgumentSyntax attributeArgumentSyntax,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeArgumentClean(attributeArgumentSyntax, options), cancellationToken);

        /// <summary>Removes <paramref name="attributeArgumentNodes"/> from the document, cleaning up any leftover trivia.</summary>
        /// <param name="attributeArgumentNodes">The attribute argument syntax nodes to remove.</param>
        /// <param name="options">Controls how surrounding trivia is preserved after removal.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated <see cref="Document"/> with the attribute arguments removed.</returns>
        public async Task<Document> RemoveAttributeArgumentsCleanAsync
        (
            IEnumerable<AttributeArgumentSyntax> attributeArgumentNodes,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeArgumentsClean(attributeArgumentNodes, options), cancellationToken);

        /// <summary>Removes <paramref name="removedNode"/> from the document.</summary>
        /// <param name="removedNode">The syntax node to remove.</param>
        /// <param name="options">Controls how surrounding trivia is preserved after removal.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated <see cref="Document"/> with the node removed.</returns>
        public async Task<Document> RemoveSyntaxNodeAsync
        (
            SyntaxNode removedNode,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveSyntaxNode(removedNode, options), cancellationToken);

        /// <summary>Removes <paramref name="removedNodes"/> from the document.</summary>
        /// <param name="removedNodes">The syntax nodes to remove.</param>
        /// <param name="options">Controls how surrounding trivia is preserved after removal.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated <see cref="Document"/> with the nodes removed.</returns>
        public async Task<Document> RemoveSyntaxNodesAsync
        (
            IEnumerable<SyntaxNode> removedNodes,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveSyntaxNodes(removedNodes, options), cancellationToken);

        /// <summary>Inserts <paramref name="insertedNodes"/> immediately after <paramref name="referenceNode"/> in the document.</summary>
        /// <param name="referenceNode">The node after which to insert.</param>
        /// <param name="insertedNodes">The nodes to insert.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated <see cref="Document"/> with the nodes inserted.</returns>
        public async Task<Document> InsertSyntaxNodesAfterAsync
        (
            SyntaxNode referenceNode,
            IEnumerable<SyntaxNode> insertedNodes,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.InsertSyntaxNodesAfter(referenceNode, insertedNodes), cancellationToken);

        /// <summary>Inserts <paramref name="insertedNodes"/> immediately before <paramref name="referenceNode"/> in the document.</summary>
        /// <param name="referenceNode">The node before which to insert.</param>
        /// <param name="insertedNodes">The nodes to insert.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated <see cref="Document"/> with the nodes inserted.</returns>
        public async Task<Document> InsertSyntaxNodesBeforeAsync
        (
            SyntaxNode referenceNode,
            IEnumerable<SyntaxNode> insertedNodes,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.InsertSyntaxNodesBefore(referenceNode, insertedNodes), cancellationToken);

        /// <summary>Returns the syntax root of the context's document.</summary>
        /// <returns>The root <see cref="SyntaxNode"/> of the document, or <see langword="null"/>.</returns>
        public async Task<SyntaxNode?> GetSyntaxRootAsync() => await context.Document.GetSyntaxRootAsync(context.CancellationToken).NoContext;
    }
}
