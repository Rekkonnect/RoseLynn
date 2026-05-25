using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;

namespace RoseLynn;

public static class SyntaxQueryingExtensions
{
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

    public static bool ContainsStrict(this TextSpan span, TextSpan other)
    {
        return span.Contains(other.Start)
            && span.Contains(other.End - 1)
            ;
    }

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

    public static SyntaxNode? SyntaxNodeAtPosition(this SyntaxTree tree, int position)
    {
        var root = tree.GetRoot();
        position = Clamp(position, 0, root.FullSpan.End - 1);
        return root.DeepestNodeContainingPosition(position);
    }

    public static SyntaxNode? SyntaxNodeAtPositionIncludingStructuredTrivia(
        this SyntaxTree tree, int position)
    {
        var root = tree.GetRoot();
        position = Clamp(position, 0, root.FullSpan.End - 1);
        return root.DeepestNodeContainingPositionIncludingStructuredTrivia(position);
    }

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

    public static bool HasNoArguments(this AttributeData attribute)
    {
        return attribute.ConstructorArguments is []
            && attribute.NamedArguments is []
            ;
    }

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
