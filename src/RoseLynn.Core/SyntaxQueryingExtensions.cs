using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;

namespace RoseLynn.Core;

/// <summary>Provides advanced querying helpers for navigating syntax trees by position and span.</summary>
public static class SyntaxQueryingExtensions
{
    /// <summary>Returns the deepest <seealso cref="SyntaxToken"/> in <paramref name="parent"/> whose full span contains <paramref name="position"/>.</summary>
    /// <param name="parent">The root node to search.</param>
    /// <param name="position">The character position to locate.</param>
    /// <returns>The deepest token containing the position, or <see langword="default"/> if not found.</returns>
    public static SyntaxToken DeepestTokenContainingPosition(this SyntaxNode parent, int position)
    {
        if (!parent.FullSpan.Contains(position))
            return default;

        var current = parent;

        while (true)
        {
            var child = current.ChildThatContainsPosition(position);
            if (child == default)
                return default;

            if (child.IsToken)
                return child.AsToken();

            var node = child.AsNode();
            Asserts.NotNull(node);
            current = node;
        }
    }

    /// <summary>Returns the deepest <seealso cref="SyntaxToken"/> in <paramref name="parent"/> whose full span strictly contains <paramref name="span"/>.</summary>
    /// <param name="parent">The root node to search.</param>
    /// <param name="span">The text span to locate.</param>
    /// <returns>The deepest token containing the span, or <see langword="default"/> if not found.</returns>
    public static SyntaxToken DeepestTokenContainingSpan(this SyntaxNode parent, TextSpan span)
    {
        if (!parent.FullSpan.ContainsStrict(span))
            return default;

        var current = parent;

        while (true)
        {
            var child = current.ChildThatContainsSpan(span);
            if (child == default)
                return default;

            if (child.IsToken)
                return child.AsToken();

            var node = child.AsNode();
            Asserts.NotNull(node);
            current = node;
        }
    }

    /// <summary>Returns the deepest <seealso cref="SyntaxTrivia"/> in <paramref name="parent"/> whose full span strictly contains <paramref name="span"/>.</summary>
    /// <param name="parent">The root node to search.</param>
    /// <param name="span">The text span to locate.</param>
    /// <returns>The deepest trivia containing the span, or <see langword="default"/> if not found.</returns>
    public static SyntaxTrivia DeepestTriviaContainingSpan(this SyntaxNode parent, TextSpan span)
    {
        if (!parent.FullSpan.ContainsStrict(span))
            return default;

        var current = parent;

        while (true)
        {
            var child = current.ChildThatContainsSpan(span);
            if (child == default)
                return default;

            var trivia = child.FindTrivia(span);
            if (trivia != default)
            {
                return trivia;
            }

            if (child.IsToken)
                return default;

            var node = child.AsNode();
            Asserts.NotNull(node);
            current = node;
        }
    }

    /// <summary>Returns the trivia in <paramref name="child"/> that covers <paramref name="span"/>, or <see langword="default"/> if no single trivia spans the whole range.</summary>
    /// <param name="child">The node or token whose trivia to search.</param>
    /// <param name="span">The text span to match.</param>
    /// <returns>The matching trivia, or <see langword="default"/>.</returns>
    public static SyntaxTrivia FindTrivia(this SyntaxNodeOrToken child, TextSpan span)
    {
        var startingTrivia = FindTrivia(child, span.Start);
        if (startingTrivia == default)
            return default;

        var endingTrivia = FindTrivia(child, span.End - 1);
        if (startingTrivia != endingTrivia)
            return default;

        return startingTrivia;
    }

    /// <summary>Returns the trivia in <paramref name="child"/>'s leading or trailing trivia that contains <paramref name="position"/>.</summary>
    /// <param name="child">The node or token whose trivia to search.</param>
    /// <param name="position">The character position.</param>
    /// <returns>The matching trivia, or <see langword="default"/>.</returns>
    public static SyntaxTrivia FindTrivia(this SyntaxNodeOrToken child, int position)
    {
        var leading = child.GetLeadingTrivia();
        if (leading.Span.Contains(position))
        {
            return AtPosition(leading, position);
        }

        var trailing = child.GetTrailingTrivia();
        return AtPosition(trailing, position);
    }

    private static SyntaxTrivia AtPosition(this SyntaxTriviaList list, int position)
    {
        foreach (var trivia in list)
        {
            if (trivia.Span.Contains(position))
                return trivia;
        }

        return default;
    }

    /// <summary>Returns the deepest syntax node in <paramref name="parent"/> whose full span contains <paramref name="position"/>, stopping before tokens.</summary>
    /// <param name="parent">The root node to search.</param>
    /// <param name="position">The character position to locate.</param>
    /// <returns>The deepest node at the position, or <see langword="null"/> if outside <paramref name="parent"/>.</returns>
    public static SyntaxNode? DeepestNodeContainingPosition(this SyntaxNode parent, int position)
    {
        if (!parent.FullSpan.Contains(position))
            return null;

        var current = parent;

        while (true)
        {
            var child = current.ChildThatContainsPosition(position);
            if (child == default)
                return current;

            if (child.IsToken)
                return current;

            var node = child.AsNode();
            Asserts.NotNull(node);
            current = node;
        }
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="span"/> strictly contains every position in <paramref name="other"/> (both start and end-1).</summary>
    /// <param name="span">The outer span.</param>
    /// <param name="other">The inner span to test.</param>
    /// <returns><see langword="true"/> when <paramref name="other"/> fits entirely within <paramref name="span"/>.</returns>
    public static bool ContainsStrict(this TextSpan span, TextSpan other)
    {
        return span.Contains(other.Start)
            && span.Contains(other.End - 1)
            ;
    }

    /// <summary>Returns the single child of <paramref name="node"/> whose full span strictly contains <paramref name="span"/>, or <see langword="default"/> if none or multiple.</summary>
    /// <param name="node">The parent node to search.</param>
    /// <param name="span">The text span to locate.</param>
    /// <returns>The single containing child, or <see langword="default"/>.</returns>
    public static SyntaxNodeOrToken ChildThatContainsSpan(this SyntaxNode node, TextSpan span)
    {
        var fullSpan = node.FullSpan;
        if (!fullSpan.ContainsStrict(span))
            return null;

        var start = span.Start;
        var end = span.End;

        var startingChild = node.ChildThatContainsPosition(start);
        if (startingChild == default)
            return default;

        var endingChild = node.ChildThatContainsPosition(end - 1);
        if (startingChild != endingChild)
            return default;

        return startingChild;
    }

    /// <summary>Returns the deepest node in <paramref name="parent"/> at <paramref name="position"/>, descending into structured trivia when encountered.</summary>
    /// <param name="parent">The root node to search.</param>
    /// <param name="position">The character position to locate.</param>
    /// <returns>The deepest node, or <see langword="null"/> if outside <paramref name="parent"/>.</returns>
    public static SyntaxNode? DeepestNodeContainingPositionIncludingStructuredTrivia(
        this SyntaxNode parent, int position)
    {
        if (!parent.FullSpan.Contains(position))
            return null;

        var current = parent;

        while (true)
        {
            var child = current.ChildThatContainsPosition(position);
            if (child == default)
                return current;

            if (child.IsToken)
            {
                if (!current.HasStructuredTrivia)
                    return current;

                var trivia = current.FindTrivia(position);
                var structure = trivia.GetStructure();
                if (structure is null)
                    return current;

                current = structure;
                continue;
            }

            var node = child.AsNode();
            Asserts.NotNull(node);
            current = node;
        }
    }

    /// <summary>Returns the deepest syntax node in <paramref name="tree"/> at <paramref name="position"/>, clamped to the tree's valid range.</summary>
    /// <param name="tree">The syntax tree to search.</param>
    /// <param name="position">The character position.</param>
    /// <returns>The deepest node at the position, or <see langword="null"/>.</returns>
    public static SyntaxNode? SyntaxNodeAtPosition(this SyntaxTree tree, int position)
    {
        var root = tree.GetRoot();
        position = Clamp(position, 0, root.FullSpan.End - 1);
        return root.DeepestNodeContainingPosition(position);
    }

    /// <summary>Returns the deepest syntax node in <paramref name="tree"/> at <paramref name="position"/>, descending into structured trivia, clamped to the tree's valid range.</summary>
    /// <param name="tree">The syntax tree to search.</param>
    /// <param name="position">The character position.</param>
    /// <returns>The deepest node at the position including structured trivia, or <see langword="null"/>.</returns>
    public static SyntaxNode? SyntaxNodeAtPositionIncludingStructuredTrivia(
        this SyntaxTree tree, int position)
    {
        var root = tree.GetRoot();
        position = Clamp(position, 0, root.FullSpan.End - 1);
        return root.DeepestNodeContainingPositionIncludingStructuredTrivia(position);
    }

    /// <summary>Returns the deepest syntax node in <paramref name="parent"/> whose full span contains <paramref name="span"/>, descending into structured trivia when encountered.</summary>
    /// <param name="parent">The root node to search.</param>
    /// <param name="span">The text span to locate.</param>
    /// <returns>The deepest node containing the span including structured trivia, or <see langword="null"/> if outside <paramref name="parent"/>.</returns>
    public static SyntaxNode? DeepestNodeContainingSpanIncludingStructuredTrivia(
        this SyntaxNode parent, TextSpan span)
    {
        if (!parent.FullSpan.ContainsStrict(span))
            return null;

        var current = parent;

        while (true)
        {
            var child = current.ChildThatContainsSpan(span);
            if (child == default)
                return current;

            if (child.IsToken)
            {
                if (!current.HasStructuredTrivia)
                    return current;

                var startTrivia = current.FindTrivia(span.Start);
                var endTrivia = current.FindTrivia(span.End - 1);

                if (startTrivia != endTrivia)
                    return current;

                var trivia = startTrivia;
                var structure = trivia.GetStructure();
                if (structure is null)
                    return current;

                current = structure;
                continue;
            }

            var node = child.AsNode();
            Asserts.NotNull(node);
            current = node;
        }
    }

    /// <summary>Returns the deepest syntax node in <paramref name="tree"/> covering <paramref name="span"/>, descending into structured trivia, with span clamped to the tree's valid range.</summary>
    /// <param name="tree">The syntax tree to search.</param>
    /// <param name="span">The text span to locate.</param>
    /// <returns>The deepest node containing the span including structured trivia, or <see langword="null"/>.</returns>
    public static SyntaxNode? SyntaxNodeAtSpanIncludingStructuredTrivia(
        this SyntaxTree tree, TextSpan span)
    {
        var root = tree.GetRoot();
        int lastIndex = Math.Max(0, root.FullSpan.End - 1);
        var start = Clamp(span.Start, 0, lastIndex);
        var end = Clamp(span.End, 0, root.FullSpan.End);
        var clampedSpan = TextSpan.FromBounds(start, end);
        return root.DeepestNodeContainingSpanIncludingStructuredTrivia(clampedSpan);
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="attribute"/> has no constructor arguments and no named arguments.</summary>
    /// <param name="attribute">The attribute data to inspect.</param>
    /// <returns><see langword="true"/> when the attribute was applied with no arguments.</returns>
    public static bool HasNoArguments(this AttributeData attribute)
    {
        return attribute.ConstructorArguments is []
            && attribute.NamedArguments is []
            ;
    }

    /// <summary>Returns the outermost ancestor of <paramref name="node"/> that shares the same <see cref="SyntaxNode.Span"/>.</summary>
    /// <param name="node">The node to walk up from.</param>
    /// <returns>The outermost ancestor with the same span, or <paramref name="node"/> itself if no such ancestor exists.</returns>
    public static SyntaxNode OutermostSameSpanParent(this SyntaxNode node)
    {
        var span = node.Span;

        var current = node;
        while (true)
        {
            var parent = current.Parent;
            if (parent is null)
                return current;

            if (parent.Span != span)
                return current;

            current = parent;
        }
    }

    /// <summary>Returns the <see cref="LinePosition"/> in <paramref name="tree"/> for the given character <paramref name="position"/>.</summary>
    /// <param name="tree">The syntax tree whose line map to query.</param>
    /// <param name="position">The character offset within the tree.</param>
    /// <returns>The line and character position corresponding to <paramref name="position"/>.</returns>
    public static LinePosition GetLinePosition(
        this SyntaxTree tree, int position)
    {
        var span = new TextSpan(position, 0);
        return tree.GetLineSpan(span).Span.Start;
    }

    private static int Clamp(int value, int min, int max)
    {
        return Math.Max(Math.Min(value, max), min);
    }
}
