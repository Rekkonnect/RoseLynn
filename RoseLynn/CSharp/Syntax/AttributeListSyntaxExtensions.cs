#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides useful extensions for the <seealso cref="AttributeListSyntax"/> type.</summary>
public static class AttributeListSyntaxExtensions
{
    /// <summary>Gets the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeListSyntax"/>.</summary>
    /// <param name="attributeList">The <seealso cref="AttributeListSyntax"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attributeList"/>.</returns>
    public static AttributeListTarget GetTarget(this AttributeListSyntax? attributeList)
    {
        var targetNode = attributeList?.Target;
        if (targetNode is null)
            return AttributeListTarget.Default;

        return targetNode.Identifier.Kind().GetAttributeListTarget();
    }
    /// <summary>Determines whether the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeListSyntax"/> is a provided one.</summary>
    /// <param name="attributeList">The <seealso cref="AttributeListSyntax"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <param name="target">The desired <seealso cref="AttributeListTarget"/> to check.</param>
    /// <returns><see langword="true"/> if the <seealso cref="AttributeListTarget"/> of <paramref name="attributeList"/> matches the given one.</returns>
    public static bool HasTarget(this AttributeListSyntax? attributeList, AttributeListTarget target)
    {
        var targetNode = attributeList?.Target;
        if (targetNode is null)
            return target is AttributeListTarget.Default;

        return targetNode.Identifier.IsKind(target.GetSyntaxKind());
    }

    /// <summary>Resolves the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeListSyntax"/>, if none is explicitly specified.</summary>
    /// <param name="attributeList">The <seealso cref="AttributeListSyntax"/> whose <seealso cref="AttributeListTarget"/> to resolve.</param>
    /// <returns>The resolved <seealso cref="AttributeListTarget"/> of <paramref name="attributeList"/>.</returns>
    public static AttributeListTarget GetResolvedTarget(this AttributeListSyntax? attributeList)
    {
        if (attributeList is null)
            return AttributeListTarget.Default;

        var target = attributeList!.GetTarget();
        if (target is not AttributeListTarget.Default)
            return target;

        return ResolveAttributeListTarget(attributeList);
    }
    /// <summary>Determines whether the resolved <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeListSyntax"/> is a provided one.</summary>
    /// <param name="attributeList">The <seealso cref="AttributeListSyntax"/> whose <seealso cref="AttributeListTarget"/> to resolve.</param>
    /// <param name="target">The desired <seealso cref="AttributeListTarget"/> to check.</param>
    /// <returns><see langword="true"/> if the resolved <seealso cref="AttributeListTarget"/> of <paramref name="attributeList"/> matches the given one.</returns>
    public static bool HasResolvedTarget(this AttributeListSyntax? attributeList, AttributeListTarget target)
    {
        if (attributeList.HasTarget(target))
            return true;

        if (attributeList is null)
            return false;

        return ResolveAttributeListTarget(attributeList) == target;
    }

    private static AttributeListTarget ResolveAttributeListTarget(AttributeListSyntax attributeList)
    {
        return (attributeList.Parent as CSharpSyntaxNode)!.ResolveDefaultAttributeListTarget();
    }

    // No idea how this might or might not work
    // Privated for the time being until it is proven useful and valid.
    private static IEnumerable<SyntaxNode> GetAttributedNodes(this AttributeListSyntax? attributeList)
    {
        var parent = attributeList?.Parent;
        var nodes = parent?.ChildNodes();

        if (nodes is null)
            yield break;

        bool foundAttributeList = false;
        foreach (var n in nodes)
        {
            if (!foundAttributeList)
            {
                if (n.Equals(attributeList))
                    foundAttributeList = true;
            }
            else
            {
                // This needs a bit of work
                // The main concept is that multiple fields that are declared in the same line can be attributed
                // However, this does not apply to multiple arguments or other forms of declarations with the same parent
                if (n is not AttributeListSyntax)
                    yield return n;
            }
        }
    }
}
