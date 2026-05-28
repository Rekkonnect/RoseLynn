using Garyon.Extensions;
using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Provides extension methods for <seealso cref="Solution"/>.</summary>
public static class SolutionExtensions
{
    /// <summary>Returns the <seealso cref="Document"/> instances for the given set of <paramref name="documents"/> IDs.</summary>
    /// <param name="solution">The solution to query.</param>
    /// <param name="documents">The document IDs to resolve.</param>
    /// <returns>A read-only collection of resolved documents.</returns>
    public static IReadOnlyCollection<Document> GetDocuments(
        this Solution solution, IReadOnlyCollection<DocumentId> documents)
    {
        return documents.Select(solution.GetDocument).ToList()!;
    }

    /// <summary>Groups <paramref name="syntaxNodes"/> by their containing <seealso cref="DocumentId"/> within <paramref name="solution"/>.</summary>
    /// <param name="solution">The solution used to resolve document IDs.</param>
    /// <param name="syntaxNodes">The nodes to group.</param>
    /// <returns>A dictionary mapping each document ID to its associated syntax nodes.</returns>
    public static IReadOnlyDictionary<DocumentId, IReadOnlyList<SyntaxNode>> GetDocumentIdsByNode(
        this Solution solution, IEnumerable<SyntaxNode> syntaxNodes)
    {
        return syntaxNodes
            .GroupBy(node => solution.GetDocumentId(node.SyntaxTree))
            .Where(s => s.Key is not null)
            .ToDictionary(
                keySelector: static s => s.Key!,
                elementSelector: nodes => nodes.ToReadOnlyListOrExisting())
            ;
    }

    /// <summary>Returns the distinct set of <seealso cref="DocumentId"/>s containing any of the given <paramref name="syntaxNodes"/>.</summary>
    /// <param name="solution">The solution used to resolve document IDs.</param>
    /// <param name="syntaxNodes">The nodes whose document IDs to collect.</param>
    /// <returns>A distinct, non-null collection of document IDs.</returns>
    public static IReadOnlyCollection<DocumentId> GetDistinctDocumentIds(
        this Solution solution, IEnumerable<SyntaxNode> syntaxNodes)
    {
        return syntaxNodes
            .Select(node => solution.GetDocumentId(node.SyntaxTree))
            .Distinct()
            .WhereNotNull()
            .ToList()!
            ;
    }

    /// <summary>Groups <paramref name="syntaxNodes"/> by their containing <seealso cref="Document"/> within <paramref name="solution"/>.</summary>
    /// <param name="solution">The solution used to resolve documents.</param>
    /// <param name="syntaxNodes">The nodes to group.</param>
    /// <returns>A dictionary mapping each document to its associated syntax nodes.</returns>
    public static IReadOnlyDictionary<Document, IReadOnlyList<SyntaxNode>> GetDocumentsToNodes(
        this Solution solution, IEnumerable<SyntaxNode> syntaxNodes)
    {
        return GetDocumentIdsByNode(solution, syntaxNodes)
            .TransformKeys(s => solution.GetDocument(s)!)!
            ;
    }

    /// <summary>Gets the syntax roots of all documents identified by <paramref name="documentIds"/> within <paramref name="solution"/>.</summary>
    /// <param name="solution">The solution to query.</param>
    /// <param name="documentIds">The document IDs whose roots to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of non-null syntax roots.</returns>
    public static async Task<IReadOnlyCollection<SyntaxNode>> GetRootsForDocuments(
        this Solution solution,
        IReadOnlyCollection<DocumentId> documentIds,
        CancellationToken cancellationToken)
    {
        var documents = solution.GetDocuments(documentIds);
        var roots = await documents
            .GetAllNotNullNodes(cancellationToken);
        return roots;
    }

    /// <summary>Returns a new solution with the updated name, file path, and syntax root from <paramref name="document"/>.</summary>
    /// <param name="solution">The solution to update.</param>
    /// <param name="document">The document whose state to apply to the solution.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated solution.</returns>
    public static async Task<Solution> ReplaceDocumentAsync(
        this Solution solution, Document document, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        return solution
            .WithDocumentName(document.Id, document.Name)
            .WithDocumentFilePath(document.Id, document.FilePath)
            .WithDocumentSyntaxRootIfExists(document.Id, root)
            ;
    }

    /// <summary>Returns a solution with the syntax root of <paramref name="documentId"/> set to <paramref name="root"/>, or the unchanged solution if <paramref name="root"/> is <see langword="null"/>.</summary>
    /// <param name="solution">The solution to update.</param>
    /// <param name="documentId">The document whose root to replace.</param>
    /// <param name="root">The new syntax root, or <see langword="null"/> to leave the solution unchanged.</param>
    /// <returns>The updated solution, or the original if <paramref name="root"/> is <see langword="null"/>.</returns>
    public static Solution WithDocumentSyntaxRootIfExists(
        this Solution solution, DocumentId documentId, SyntaxNode? root)
    {
        if (root is null)
        {
            return solution;
        }

        return solution.WithDocumentSyntaxRoot(documentId, root);
    }

    /// <summary>Replaces <paramref name="node"/> with <paramref name="replacedNode"/> in <paramref name="document"/> and returns the updated solution.</summary>
    /// <param name="solution">The solution containing the document.</param>
    /// <param name="document">The document in which to perform the replacement.</param>
    /// <param name="node">The node to replace.</param>
    /// <param name="replacedNode">The replacement node.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated solution.</returns>
    public static async Task<Solution> ReplaceNodeAsync(
        this Solution solution, Document document,
        SyntaxNode node, SyntaxNode replacedNode,
        CancellationToken cancellationToken)
    {
        var replacedDocument = await document.WithSyntaxRootAsync(
            root => root.ReplaceNode(node, replacedNode), cancellationToken);
        return await solution.ReplaceDocumentAsync(replacedDocument, cancellationToken);
    }

    /// <summary>Applies <paramref name="nodeReplacer"/> to each of <paramref name="nodes"/> across their respective documents, returning the updated solution.</summary>
    /// <param name="solution">The solution containing the nodes' documents.</param>
    /// <param name="nodes">The nodes to replace.</param>
    /// <param name="nodeReplacer">A function that maps each node to its replacement.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated solution with all replacements applied.</returns>
    public static async Task<Solution> ReplaceNodesAsync(
        this Solution solution,
        IEnumerable<SyntaxNode> nodes,
        Func<SyntaxNode, SyntaxNode> nodeReplacer,
        CancellationToken cancellationToken)
    {
        var documentKvps = GetDocumentsToNodes(solution, nodes.ToReadOnlyListOrExisting());
        foreach (var kvp in documentKvps)
        {
            var document = kvp.Key;
            var documentNodes = kvp.Value;
            var replacedDocument = await document.WithSyntaxRootAsync(
                root => root.ReplaceNodes(documentNodes, nodeReplacer), cancellationToken);
            solution = await solution.ReplaceDocumentAsync(
                replacedDocument, cancellationToken);
        }
        return solution;
    }
}
