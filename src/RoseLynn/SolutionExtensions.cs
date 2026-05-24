using Garyon.Extensions;
using Microsoft.CodeAnalysis;

namespace RoseLynn;

public static class SolutionExtensions
{
    public static IReadOnlyCollection<Document> GetDocuments(
        this Solution solution, IReadOnlyCollection<DocumentId> documents)
    {
        return documents.Select(solution.GetDocument).ToList()!;
    }

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

    public static IReadOnlyDictionary<Document, IReadOnlyList<SyntaxNode>> GetDocumentsToNodes(
        this Solution solution, IEnumerable<SyntaxNode> syntaxNodes)
    {
        return GetDocumentIdsByNode(solution, syntaxNodes)
            .TransformKeys(s => solution.GetDocument(s)!)!
            ;
    }

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

    public static Solution WithDocumentSyntaxRootIfExists(
        this Solution solution, DocumentId documentId, SyntaxNode? root)
    {
        if (root is null)
        {
            return solution;
        }

        return solution.WithDocumentSyntaxRoot(documentId, root);
    }

    public static async Task<Solution> ReplaceNodeAsync(
        this Solution solution, Document document,
        SyntaxNode node, SyntaxNode replacedNode,
        CancellationToken cancellationToken)
    {
        var replacedDocument = await document.WithSyntaxRootAsync(
            root => root.ReplaceNode(node, replacedNode), cancellationToken);
        return await solution.ReplaceDocumentAsync(replacedDocument, cancellationToken);
    }

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
