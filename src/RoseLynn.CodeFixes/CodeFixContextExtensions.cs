using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CodeFixes;

/// <summary>Provides useful extensions for the <seealso cref="CodeFixContext"/> type.</summary>
public static class CodeFixContextExtensions
{
    public static async Task<Document> RemoveAttributeCleanAsync
    (
        this CodeFixContext context,
        AttributeSyntax attributeSyntax,
        SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
        CancellationToken cancellationToken = default
    )
        => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeClean(attributeSyntax, options), cancellationToken);

    public static async Task<Document> RemoveAttributeArgumentCleanAsync
    (
        this CodeFixContext context,
        AttributeArgumentSyntax attributeArgumentSyntax,
        SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
        CancellationToken cancellationToken = default
    )
        => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeArgumentClean(attributeArgumentSyntax, options), cancellationToken);

    public static async Task<Document> RemoveAttributeArgumentsCleanAsync
    (
        this CodeFixContext context,
        IEnumerable<AttributeArgumentSyntax> attributeArgumentNodes,
        SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
        CancellationToken cancellationToken = default
    )
        => await context.Document.WithSyntaxRootAsync(root => root.RemoveAttributeArgumentsClean(attributeArgumentNodes, options), cancellationToken);

    public static async Task<Document> RemoveSyntaxNodeAsync
    (
        this CodeFixContext context,
        SyntaxNode removedNode,
        SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
        CancellationToken cancellationToken = default
    )
        => await context.Document.WithSyntaxRootAsync(root => root.RemoveSyntaxNode(removedNode, options), cancellationToken);

    public static async Task<Document> RemoveSyntaxNodesAsync
    (
        this CodeFixContext context,
        IEnumerable<SyntaxNode> removedNodes,
        SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia,
        CancellationToken cancellationToken = default
    )
        => await context.Document.WithSyntaxRootAsync(root => root.RemoveSyntaxNodes(removedNodes, options), cancellationToken);

    public static async Task<Document> InsertSyntaxNodesAfterAsync
    (
        this CodeFixContext context,
        SyntaxNode referenceNode,
        IEnumerable<SyntaxNode> insertedNodes,
        CancellationToken cancellationToken = default
    )
        => await context.Document.WithSyntaxRootAsync(root => root.InsertSyntaxNodesAfter(referenceNode, insertedNodes), cancellationToken);

    public static async Task<Document> InsertSyntaxNodesBeforeAsync
    (
        this CodeFixContext context,
        SyntaxNode referenceNode,
        IEnumerable<SyntaxNode> insertedNodes,
        CancellationToken cancellationToken = default
    )
        => await context.Document.WithSyntaxRootAsync(root => root.InsertSyntaxNodesBefore(referenceNode, insertedNodes), cancellationToken);

    public static async Task<SyntaxNode?> GetSyntaxRootAsync(this CodeFixContext context) => await context.Document.GetSyntaxRootAsync(context.CancellationToken).NoContext;
}
