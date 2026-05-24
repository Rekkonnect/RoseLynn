using Microsoft.CodeAnalysis;

namespace RoseLynn.Workspaces;

public static class DocumentTransformationHelpers
{
    public static async Task<Document> AnnotateNodes(
        this Document document,
        IReadOnlyCollection<SyntaxNode> nodes,
        string annotationKind,
        string annotationValue,
        CancellationToken cancellationToken)
    {
        return await document.WithSyntaxRootAsync(
            root => root.ReplaceNodes(
                nodes,
                node => AnnotateNode(node, annotationKind, annotationValue)),
                cancellationToken);
    }

    public static SyntaxNode AnnotateNode(
        this SyntaxNode node,
        string kind,
        string value)
    {
        var annotation = new SyntaxAnnotation(kind, value);
        return node.WithAdditionalAnnotations(annotation);
    }
}
