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
        public Solution Solution => context.Document.Solution;

        public async Task<Document> RemoveAttributeCleanAsync
        (
            AttributeSyntax attributeSyntax,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeClean(attributeSyntax, options), cancellationToken);

        public async Task<Document> RemoveAttributeArgumentCleanAsync
        (
            AttributeArgumentSyntax attributeArgumentSyntax,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeArgumentClean(attributeArgumentSyntax, options), cancellationToken);

        public async Task<Document> RemoveAttributeArgumentsCleanAsync
        (
            IEnumerable<AttributeArgumentSyntax> attributeArgumentNodes,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeArgumentsClean(attributeArgumentNodes, options), cancellationToken);

        public async Task<Document> RemoveSyntaxNodeAsync
        (
            SyntaxNode removedNode,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveSyntaxNode(removedNode, options), cancellationToken);

        public async Task<Document> RemoveSyntaxNodesAsync
        (
            IEnumerable<SyntaxNode> removedNodes,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.RemoveSyntaxNodes(removedNodes, options), cancellationToken);

        public async Task<Document> InsertSyntaxNodesAfterAsync
        (
            SyntaxNode referenceNode,
            IEnumerable<SyntaxNode> insertedNodes,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.InsertSyntaxNodesAfter(referenceNode, insertedNodes), cancellationToken);

        public async Task<Document> InsertSyntaxNodesBeforeAsync
        (
            SyntaxNode referenceNode,
            IEnumerable<SyntaxNode> insertedNodes,
            CancellationToken cancellationToken = default
        )
            => await context.Document.WithSyntaxRootAsync(root => root.InsertSyntaxNodesBefore(referenceNode, insertedNodes), cancellationToken);

        public async Task<SyntaxNode?> GetSyntaxRootAsync() => await context.Document.GetSyntaxRootAsync(context.CancellationToken).NoContext;
    }
}
