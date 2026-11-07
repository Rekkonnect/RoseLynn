using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Provides useful extensions for the <seealso cref="SyntaxNode"/> type.</summary>
public static class SyntaxNodeExtensions
{
    extension(SyntaxNode node)
    {
        /// <summary>Recursively iterates through the parents of the given node until a parent with the specified type is found.</summary>
        /// <typeparam name="T">The type of the parent to get.</typeparam>
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

        /// <summary>Returns the node with <paramref name="append"/> added to the end of its leading trivia.</summary>
        /// <param name="append">The trivia to append.</param>
        /// <returns>The node with updated leading trivia.</returns>
        public SyntaxNode AppendLeadingTrivia(
            SyntaxTriviaList append)
        {
            return node.WithLeadingTrivia(
                node.GetLeadingTrivia().AddRange(append));
        }

        /// <summary>Returns the node with <paramref name="prepend"/> inserted at the start of its trailing trivia.</summary>
        /// <param name="prepend">The trivia to prepend.</param>
        /// <returns>The node with updated trailing trivia.</returns>
        public SyntaxNode PrependTrailingTrivia(
            SyntaxTriviaList prepend)
        {
            return node.WithTrailingTrivia(
                node.GetTrailingTrivia().Prepend(prepend));
        }
    }

    extension<TNode>(TNode node) where TNode : SyntaxNode
    {
        /// <summary>Replaces each node in <paramref name="nodes"/> using <paramref name="computeReplacementNode"/>, receiving only the tracked replacement node.</summary>
        /// <param name="nodes">The nodes to replace.</param>
        /// <param name="computeReplacementNode">A function mapping a tracked replacement node to its final replacement.</param>
        /// <returns>The node with all replacements applied.</returns>
        public TNode ReplaceNodes(
            IEnumerable<TNode> nodes,
            Func<TNode, SyntaxNode> computeReplacementNode)
        {
            return node.ReplaceNodes(nodes, (a, b) => computeReplacementNode(b));
        }
    }

    /// <summary>Enumerates all descendant nodes and self of <paramref name="root"/> that contain at least one annotation.</summary>
    /// <param name="root">The root node to search.</param>
    /// <param name="descendIntoTrivia">Whether to descend into trivia.</param>
    /// <returns>All annotated descendant nodes and the root itself.</returns>
    public static IEnumerable<SyntaxNode> AnnotatedDescendantNodesAndSelf(
        this SyntaxNode root, bool descendIntoTrivia = true)
    {
        return root.DescendantNodesAndSelf(n => n.ContainsAnnotations, descendIntoTrivia);
    }

    /// <summary>Returns all nodes annotated with the given annotation <paramref name="kind"/>.</summary>
    /// <param name="root">The root node to search.</param>
    /// <param name="kind">The annotation kind to match.</param>
    /// <returns>All nodes that carry an annotation of the specified kind.</returns>
    public static IEnumerable<SyntaxNode> NodesWithAnnotationKind(
        this SyntaxNode root, string kind)
    {
        return root.GetAnnotatedNodes(kind);
    }

    /// <summary>Returns all nodes annotated with an annotation of the given <paramref name="kind"/> and <paramref name="data"/>.</summary>
    /// <param name="root">The root node to search.</param>
    /// <param name="kind">The annotation kind to match.</param>
    /// <param name="data">The annotation data to match.</param>
    /// <returns>All nodes that carry a matching annotation.</returns>
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

    /// <summary>Returns all nodes annotated with <paramref name="annotation"/>.</summary>
    /// <param name="root">The root node to search.</param>
    /// <param name="annotation">The annotation identifier to match.</param>
    /// <returns>All nodes that carry the specified annotation.</returns>
    public static IEnumerable<SyntaxNode> NodesWithAnnotation(
        this SyntaxNode root, SyntaxAnnotationIdentifier annotation)
    {
        return root.NodesWithAnnotation(annotation.Kind, annotation.Data);
    }

    /// <summary>Returns the first node that carries <paramref name="annotation"/> but not <paramref name="excludingAnnotation"/>.</summary>
    /// <param name="root">The root node to search.</param>
    /// <param name="annotation">The annotation the node must have.</param>
    /// <param name="excludingAnnotation">The annotation the node must not have.</param>
    /// <returns>The first matching node, or <see langword="null"/> if none found.</returns>
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

    /// <summary>Returns all nodes that carry <paramref name="annotation"/> but not <paramref name="excludingAnnotation"/>.</summary>
    /// <param name="root">The root node to search.</param>
    /// <param name="annotation">The annotation the nodes must have.</param>
    /// <param name="excludingAnnotation">The annotation the nodes must not have.</param>
    /// <returns>All matching nodes.</returns>
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

    /// <summary>Returns <paramref name="node"/> with a unique dummy annotation added, useful for later retrieval by annotation.</summary>
    /// <typeparam name="TNode">The node type.</typeparam>
    /// <param name="node">The node to annotate.</param>
    /// <returns>The annotated node.</returns>
    public static TNode WithDummyAnnotation<TNode>(
        this TNode node)
        where TNode : SyntaxNode
    {
        return node.WithAdditionalAnnotations(
            CommonSyntaxAnnotations.CreateDummyAnnotation());
    }
}

/// <summary>Provides factory methods for common <seealso cref="SyntaxAnnotation"/> instances.</summary>
public static class CommonSyntaxAnnotations
{
    /// <summary>Creates a unique dummy annotation using a timestamp-based value.</summary>
    /// <returns>A <seealso cref="SyntaxAnnotation"/> with a unique data value.</returns>
    public static SyntaxAnnotation CreateDummyAnnotation()
    {
        const string annotationName = "Common.Dummy";
        var value = DateTime.Now.Ticks.ToString();
        return new(annotationName, value);
    }
}
