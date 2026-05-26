using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Provides useful extensions for the <seealso cref="SyntaxNode"/> type.</summary>
public static class SyntaxNodeExtensions
{
    extension(SyntaxNode node)
    {
        /// <summary>Recursively iterates through the parents of the given node until a parent with the specified type is found.</summary>
        /// <typeparam name="T">The type of the parent to get.</typeparam>
        /// <param name="node">The node whose parents to evaluate.</param>
        /// <returns>The parent that is the closest to the given node, and is of type <typeparamref name="T"/>.</returns>
        public T? GetNearestParentOfType<T>()
            where T : SyntaxNode
        {
            var parent = node;
            while (true)
            {
                parent = parent.Parent;
                switch (parent)
                {
                    case T matchingParent:
                        return matchingParent;

                    case null:
                        return null;
                }
            }
        }

        public SyntaxNode AppendLeadingTrivia(
            SyntaxTriviaList append)
        {
            return node.WithLeadingTrivia(
                node.GetLeadingTrivia().AddRange(append));
        }

        public SyntaxNode PrependTrailingTrivia(
            SyntaxTriviaList prepend)
        {
            return node.WithTrailingTrivia(
                node.GetTrailingTrivia().Prepend(prepend));
        }
    }

    extension<TNode>(TNode node) where TNode : SyntaxNode
    {
        public TNode ReplaceNodes(
            IEnumerable<TNode> nodes,
            Func<TNode, SyntaxNode> computeReplacementNode)
        {
            return node.ReplaceNodes(nodes, (a, b) => computeReplacementNode(b));
        }
    }

    public static IEnumerable<SyntaxNode> AnnotatedDescendantNodesAndSelf(
        this SyntaxNode root, bool descendIntoTrivia = true)
    {
        return root.DescendantNodesAndSelf(n => n.ContainsAnnotations, descendIntoTrivia);
    }

    public static IEnumerable<SyntaxNode> NodesWithAnnotationKind(
        this SyntaxNode root, string kind)
    {
        return root.GetAnnotatedNodes(kind);
    }

    public static IEnumerable<SyntaxNode> NodesWithAnnotation(
        this SyntaxNode root, string kind, string data)
    {
        return root.GetAnnotatedNodes(kind)
            .Where(node => node
                .GetAnnotations(kind)
                .Any(s => s.Data == data)
                )
            ;
    }

    public static IEnumerable<SyntaxNode> NodesWithAnnotation(
        this SyntaxNode root, SyntaxAnnotationIdentifier annotation)
    {
        return root.NodesWithAnnotation(annotation.Kind, annotation.Data);
    }

    public static SyntaxNode? FirstNodeWithAnnotationExcept(
        this SyntaxNode root,
        SyntaxAnnotationIdentifier annotation,
        SyntaxAnnotationIdentifier excludingAnnotation)
    {
        return root
            .NodesWithAnnotationExcept(
                annotation,
                excludingAnnotation)
            .FirstOrDefault()
            ;
    }

    public static IEnumerable<SyntaxNode> NodesWithAnnotationExcept(
        this SyntaxNode root,
        SyntaxAnnotationIdentifier annotation,
        SyntaxAnnotationIdentifier excludingAnnotation)
    {
        return root.GetAnnotatedNodes(annotation.Kind)
            .Where(node => node
                .GetAnnotations(annotation.Kind)
                .Any(s => s.Data == annotation.Data)
                )
            .Where(node => node
                .GetAnnotations(excludingAnnotation.Kind)
                .All(s => s.Data != excludingAnnotation.Data)
                )
            ;
    }

    public static TNode WithDummyAnnotation<TNode>(
        this TNode node)
        where TNode : SyntaxNode
    {
        return node.WithAdditionalAnnotations(
            CommonSyntaxAnnotations.CreateDummyAnnotation());
    }
}

public static class CommonSyntaxAnnotations
{
    public static SyntaxAnnotation CreateDummyAnnotation()
    {
        const string annotationName = "Common.Dummy";
        var value = DateTime.Now.Ticks.ToString();
        return new(annotationName, value);
    }
}
