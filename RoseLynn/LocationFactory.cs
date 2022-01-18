using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoseLynn;

/// <summary>Creates factory methods for constructing <seealso cref="Location"/> instances.</summary>
public static class LocationFactory
{
    /// <summary>Creates a new <seealso cref="Location"/> instance from the given node's <seealso cref="SyntaxTree"/>, and the specified bounds.</summary>
    /// <param name="node">The node whose <seealso cref="SyntaxTree"/> to use in the <seealso cref="Location"/>.</param>
    /// <param name="start">The start of the <seealso cref="Location.SourceSpan"/> property.</param>
    /// <param name="end">The end of the <seealso cref="Location.SourceSpan"/> property</param>
    /// <returns>A new <seealso cref="Location"/> instance with the <seealso cref="Location.SourceTree"/> set to the <seealso cref="SyntaxTree"/> of the given node, and its bounds set to the given start and end.</returns>
    public static Location CreateFromNodeTreeSpanBounds(SyntaxNode node, int start, int end)
    {
        return CreateFromNodeTree(node, TextSpan.FromBounds(start, end));
    }
    /// <summary>Creates a new <seealso cref="Location"/> instance from the given node's <seealso cref="SyntaxTree"/>, and the specified bounds.</summary>
    /// <param name="node">The node whose <seealso cref="SyntaxTree"/> to use in the <seealso cref="Location"/>.</param>
    /// <param name="span">The value for the <seealso cref="Location.SourceSpan"/> property.</param>
    /// <returns>A new <seealso cref="Location"/> instance with the <seealso cref="Location.SourceTree"/> set to the <seealso cref="SyntaxTree"/> of the given node, and its bounds set to the given <seealso cref="TextSpan"/>.</returns>
    public static Location CreateFromNodeTree(SyntaxNode node, TextSpan span)
    {
        return Location.Create(node.SyntaxTree, span);
    }
}
