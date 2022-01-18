using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Provides useful extensions for the <seealso cref="SyntaxNode"/> type.</summary>
public static class SyntaxNodeExtensions
{
    /// <summary>Recursively iterates through the parents of the given node until a parent with the specified type is found.</summary>
    /// <typeparam name="T">The type of the parent to get.</typeparam>
    /// <param name="node">The node whose parents to evaluate.</param>
    /// <returns>The parent that is the closest to the given node, and is of type <typeparamref name="T"/>.</returns>
    public static T GetNearestParentOfType<T>(this SyntaxNode node)
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
}
