using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.CodeFixes
{
    public static class CodeFixContextExtensions
    {
        public static async Task<Document> RemoveAttributeCleanAsync
        (
            this CodeFixContext context,
            AttributeSyntax attributeSyntax,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.RemoveAttributeCleanAsync(attributeSyntax, options, cancellationToken);

        public static async Task<Document> RemoveAttributeArgumentCleanAsync
        (
            this CodeFixContext context,
            AttributeArgumentSyntax attributeArgumentSyntax,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.RemoveAttributeArgumentCleanAsync(attributeArgumentSyntax, options, cancellationToken);

        public static async Task<Document> RemoveAttributeArgumentsCleanAsync
        (
            this CodeFixContext context,
            IEnumerable<AttributeArgumentSyntax> attributeArgumentNodes,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.RemoveAttributeArgumentsCleanAsync(attributeArgumentNodes, options, cancellationToken);

        public static async Task<Document> RemoveSyntaxNodeAsync
        (
            this CodeFixContext context,
            SyntaxNode removedNode,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.RemoveSyntaxNodeAsync(removedNode, options, cancellationToken);

        public static async Task<Document> RemoveSyntaxNodesAsync
        (
            this CodeFixContext context,
            IEnumerable<SyntaxNode> removedNodes,
            SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
            CancellationToken cancellationToken = default
        )
            => await context.Document.RemoveSyntaxNodesAsync(removedNodes, options, cancellationToken);

        public static async Task<Document> InsertSyntaxNodesAfterAsync
        (
            this CodeFixContext context,
            SyntaxNode referenceNode,
            IEnumerable<SyntaxNode> insertedNodes,
            CancellationToken cancellationToken = default
        )
            => await context.Document.InsertSyntaxNodesAfterAsync(referenceNode, insertedNodes, cancellationToken);

        public static async Task<Document> InsertSyntaxNodesBeforeAsync
        (
            this CodeFixContext context,
            SyntaxNode referenceNode,
            IEnumerable<SyntaxNode> insertedNodes,
            CancellationToken cancellationToken = default
        )
            => await context.Document.InsertSyntaxNodesBeforeAsync(referenceNode, insertedNodes, cancellationToken);

        public static async Task<SyntaxNode> GetSyntaxRootAsync(this CodeFixContext context) => await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
    }
}
