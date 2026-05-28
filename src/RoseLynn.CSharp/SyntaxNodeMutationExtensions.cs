using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp.Syntax;
using RoseLynn.Core.Utilities;

namespace RoseLynn.CSharp;

/// <summary>Provides mutation-oriented extensions for <see cref="SyntaxNode"/> and related syntax nodes.</summary>
public static class SyntaxNodeMutationExtensions
{
    /// <summary>Removes the attribute that was specified. If the attribute's parent attribute list remains without any attributes, it is removed.</summary>
    /// <param name="attributeNode">The attribute node to remove.</param>
    /// <param name="options">The options when performing the removal.</param>
    /// <returns>The resulting syntax root that will not contain the removed attribute.</returns>
    public static SyntaxNode RemoveAttributeClean(this SyntaxNode root, AttributeSyntax attributeNode, SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia)
    {
        SyntaxNode removedNode = attributeNode;
        var attributeList = attributeNode.GetParentAttributeList();

        if (attributeList.Attributes.Count == 1)
            removedNode = attributeList;

        return root.RemoveNode(removedNode, options) ?? root;
    }

    /// <summary>Removes the attribute argument that was specified. If the specified argument's parent attribute remains without any arguments, it is removed. If the attribute's parent attribute list remains without any attributes, it is removed.</summary>
    /// <param name="root">The syntax root whose attribute argument to remove.</param>
    /// <param name="attributeArgumentNode">The attribute argument node to remove.</param>
    /// <param name="options">The options when performing the removal.</param>
    /// <returns>The resulting syntax root that will not contain the removed attribute argument.</returns>
    public static SyntaxNode RemoveAttributeArgumentClean(this SyntaxNode root, AttributeArgumentSyntax attributeArgumentNode, SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia)
    {
        SyntaxNode removedNode = attributeArgumentNode;
        attributeArgumentNode.GetAttributeRelatedParents(out var argumentList, out var attribute, out var attributeList);

        if (argumentList.Arguments.Count == 1)
        {
            removedNode = attribute;
            if (attributeList.Attributes.Count == 1)
                removedNode = attributeList;
        }

        return root.RemoveNode(removedNode, options) ?? root;
    }

    /// <summary>Removes all attribute arguments that were specified. Any attribute that remains without any arguments is also removed. Any attribute list that remains without any attributes is also removed.</summary>
    /// <param name="root">The syntax root whose attribute arguments to remove.</param>
    /// <param name="attributeArgumentNodes">The attribute argument nodes to remove.</param>
    /// <param name="options">The options when performing the removal.</param>
    /// <returns>The resulting syntax root that will not contain the removed arguments.</returns>
    /// <remarks>The operation has a complexity of O(n).</remarks>
    public static SyntaxNode RemoveAttributeArgumentsClean(this SyntaxNode root, IEnumerable<AttributeArgumentSyntax> attributeArgumentNodes, SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia)
    {
        var unprocessedArguments = new HashSet<AttributeArgumentSyntax>(attributeArgumentNodes);

        var removedArguments = new HashSet<AttributeArgumentSyntax>();
        var removedAttributes = new HashSet<AttributeSyntax>();
        var removedAttributeLists = new HashSet<AttributeListSyntax>();

        var locallyProcessedArguments = new HashSet<AttributeArgumentSyntax>();

        while (unprocessedArguments.Any())
        {
            var argument = unprocessedArguments.First();
            argument.GetAttributeRelatedParents(out var argumentList, out var attribute, out var attributeList);

            foreach (var arg in argumentList.Arguments)
            {
                if (!unprocessedArguments.Remove(arg))
                    continue;

                locallyProcessedArguments.Add(arg);
            }

            if (locallyProcessedArguments.Count == argumentList.Arguments.Count)
            {
                removedAttributes.Add(attribute);

                bool containsAll = removedAttributes.ContainsAll(attributeList.Attributes);
                if (containsAll)
                {
                    removedAttributes.RemoveRange(attributeList.Attributes);
                    removedAttributeLists.Add(attributeList);
                }
            }
            else
            {
                removedArguments.AddRange(locallyProcessedArguments);
            }

            locallyProcessedArguments.Clear();
        }

        var removedNodes = removedArguments.AsEnumerable<SyntaxNode>().Concat(removedAttributes).Concat(removedAttributeLists);

        return root.RemoveNodes(removedNodes, options) ?? root;
    }

    /// <summary>Removes the specified node from the syntax tree, returning the updated root. Returns <paramref name="root"/> unchanged if removal produces <see langword="null"/>.</summary>
    /// <param name="root">The syntax root to operate on.</param>
    /// <param name="removedNode">The node to remove.</param>
    /// <param name="options">Controls how surrounding trivia is handled after removal.</param>
    public static SyntaxNode RemoveSyntaxNode(this SyntaxNode root, SyntaxNode removedNode, SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia)
    {
        return root.RemoveNode(removedNode, options) ?? root;
    }

    /// <summary>Removes the specified nodes from the syntax tree, returning the updated root. Returns <paramref name="root"/> unchanged if removal produces <see langword="null"/>.</summary>
    /// <param name="root">The syntax root to operate on.</param>
    /// <param name="removedNodes">The nodes to remove.</param>
    /// <param name="options">Controls how surrounding trivia is handled after removal.</param>
    public static SyntaxNode RemoveSyntaxNodes(this SyntaxNode root, IEnumerable<SyntaxNode> removedNodes, SyntaxRemoveOptions options = SyntaxRemoveOptions.KeepExteriorTrivia)
    {
        return root.RemoveNodes(removedNodes, options) ?? root;
    }

    /// <summary>Inserts the given nodes immediately after the specified reference node in the syntax tree.</summary>
    /// <param name="root">The syntax root to operate on.</param>
    /// <param name="referenceNode">The existing node after which the new nodes are inserted.</param>
    /// <param name="insertedNodes">The nodes to insert.</param>
    public static SyntaxNode InsertSyntaxNodesAfter(this SyntaxNode root, SyntaxNode referenceNode, IEnumerable<SyntaxNode> insertedNodes)
    {
        return root.InsertNodesAfter(referenceNode, insertedNodes);
    }

    /// <summary>Inserts the given nodes immediately before the specified reference node in the syntax tree.</summary>
    /// <param name="root">The syntax root to operate on.</param>
    /// <param name="referenceNode">The existing node before which the new nodes are inserted.</param>
    /// <param name="insertedNodes">The nodes to insert.</param>
    public static SyntaxNode InsertSyntaxNodesBefore(this SyntaxNode root, SyntaxNode referenceNode, IEnumerable<SyntaxNode> insertedNodes)
    {
        return root.InsertNodesBefore(referenceNode, insertedNodes);
    }

    /// <summary>Replaces the specified node in the syntax tree with multiple new nodes.</summary>
    /// <param name="root">The syntax root to operate on.</param>
    /// <param name="oldNode">The node to replace.</param>
    /// <param name="insertedNodes">The nodes to substitute in place of the removed node.</param>
    public static SyntaxNode ReplaceSyntaxNode(this SyntaxNode root, SyntaxNode oldNode, IEnumerable<SyntaxNode> insertedNodes)
    {
        return root.ReplaceNode(oldNode, insertedNodes);
    }

    /// <summary>Replaces the specified node in the syntax tree with a single new node.</summary>
    /// <param name="root">The syntax root to operate on.</param>
    /// <param name="oldNode">The node to replace.</param>
    /// <param name="insertedNode">The node to substitute.</param>
    public static SyntaxNode ReplaceSyntaxNode(this SyntaxNode root, SyntaxNode oldNode, SyntaxNode insertedNode)
    {
        return root.ReplaceNode(oldNode, insertedNode);
    }

    /// <summary>Replaces the specified nodes based on a replacement function.</summary>
    /// <typeparam name="TNode">The type of the nodes that will be replaced.</typeparam>
    /// <param name="root">The syntax root whose nodes will be replaced. The instance remains unaffected.</param>
    /// <param name="oldNodes">The original nodes that will be replaced.</param>
    /// <param name="computeReplacementNode">The function that calculates the replacement node for each of the given nodes. The first argument accepts the original node, and the second argument accepts the visited version of the original node, which could be adjusted.</param>
    /// <returns>The resulting syntax root containing the replaced nodes. The original instance remains unaffected.</returns>
    public static SyntaxNode ReplaceSyntaxNodes<TNode>(this SyntaxNode root, IEnumerable<TNode> oldNodes, Func<TNode, TNode, SyntaxNode> computeReplacementNode)
        where TNode : SyntaxNode
    {
        return root.ReplaceNodes(oldNodes, computeReplacementNode);
    }

    /// <summary>Replaces the specified nodes based on a replacement function.</summary>
    /// <typeparam name="TNode">The type of the nodes that will be replaced.</typeparam>
    /// <param name="root">The syntax root whose nodes will be replaced. The instance remains unaffected.</param>
    /// <param name="oldNodes">The original nodes that will be replaced.</param>
    /// <param name="computeReplacementNode">The function that calculates the replacement node for each of the given nodes.</param>
    /// <returns>The resulting syntax root containing the replaced nodes. The original instance remains unaffected.</returns>
    public static SyntaxNode ReplaceSyntaxNodes<TNode>(this SyntaxNode root, IEnumerable<TNode> oldNodes, Func<TNode, SyntaxNode> computeReplacementNode)
        where TNode : SyntaxNode
    {
        return root.ReplaceSyntaxNodes(oldNodes, ComputeReplacementNode);

        SyntaxNode ComputeReplacementNode(TNode old, TNode rewritten) => computeReplacementNode(rewritten);
    }

    /// <summary>Removes the <see cref="TypeParameterListSyntax"/> of the specified <see cref="MemberDeclarationSyntax"/>.</summary>
    /// <param name="root">The syntax root containing the member declaration.</param>
    /// <param name="typeDeclarationNode">The <see cref="MemberDeclarationSyntax"/> whose <see cref="TypeParameterListSyntax"/> to remove.</param>
    /// <returns>The resulting syntax root after having removed the <see cref="TypeParameterListSyntax"/> from the given <see cref="MemberDeclarationSyntax"/>.</returns>
    public static SyntaxNode RemoveTypeParameterList(this SyntaxNode root, MemberDeclarationSyntax typeDeclarationNode)
    {
        return root.ReplaceSyntaxNode(typeDeclarationNode, SyntaxNodeMutationExtensions.WithoutTypeParameterList(typeDeclarationNode));
    }

    /// <summary>Removes the <see cref="TypeParameterListSyntax"/> nodes of the specified <see cref="MemberDeclarationSyntax"/> nodes.</summary>
    /// <param name="root">The syntax root containing the member declarations.</param>
    /// <param name="typeDeclarationNodes">The <see cref="MemberDeclarationSyntax"/> nodes whose <see cref="TypeParameterListSyntax"/> nodes to remove.</param>
    /// <returns>The resulting syntax root after having removed the <see cref="TypeParameterListSyntax"/> nodes from the given <see cref="MemberDeclarationSyntax"/> nodes.</returns>
    public static SyntaxNode RemoveTypeParameterLists(this SyntaxNode root, IEnumerable<MemberDeclarationSyntax> typeDeclarationNodes)
    {
        return root.ReplaceSyntaxNodes(typeDeclarationNodes, SyntaxNodeMutationExtensions.WithoutTypeParameterList);
    }

    /// <summary>Removes the type parameter list syntax node from the given <see cref="MemberDeclarationSyntax"/> while also maintaining exterior trivia associated with the type parameter list node.</summary>
    /// <param name="memberDeclarationNode">The <see cref="MemberDeclarationSyntax"/> whose type parameter list to remove. The original node remains unaffected.</param>
    /// <returns>If the given <see cref="MemberDeclarationSyntax"/> is generic, returns the resulting <see cref="MemberDeclarationSyntax"/> without a type parameter list and with its exterior trivia being merged with that of the removed type parameter list's. Otherwise, it returns the original node without any adjustments.</returns>
    public static MemberDeclarationSyntax WithoutTypeParameterList(this MemberDeclarationSyntax memberDeclarationNode)
    {
        if (!memberDeclarationNode.IsGeneric())
            return memberDeclarationNode;

        var declarationIdentifier = memberDeclarationNode.GetIdentifier();
        var triviaAfterTypeParameterList = memberDeclarationNode.GetTypeParameterList()?.GetTrailingTrivia();
        var identifierWithTrivia = declarationIdentifier.WithTrailingTrivia(declarationIdentifier.TrailingTrivia.AddRange(triviaAfterTypeParameterList ?? []));
        var appendedTriviaResultingNode = memberDeclarationNode.WithIdentifier(identifierWithTrivia);
        return appendedTriviaResultingNode.WithTypeParameterList(null);
    }
}
