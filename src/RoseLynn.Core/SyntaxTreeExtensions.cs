using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoseLynn.Core;

/// <summary>Provides extensions for the <seealso cref="SyntaxTree"/> class.</summary>
public static class SyntaxTreeExtensions
{
    /// <summary>Gets all the nodes of the <seealso cref="SyntaxTree"/> of the specified type.</summary>
    /// <typeparam name="T">The type of the nodes to get.</typeparam>
    /// <param name="syntaxTree">The <seealso cref="SyntaxTree"/> whose nodes of the specified type to get.</param>
    /// <returns>A collection of <seealso cref="SyntaxNode"/> of the specified type contained in the specified <seealso cref="SyntaxTree"/>.</returns>
    public static IEnumerable<T> NodesOfType<T>(this SyntaxTree syntaxTree)
        where T : SyntaxNode
    {
        return syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<T>();
    }

    /// <summary>Finds the innermost node in the tree that contains the first occurrence of <paramref name="substring"/>.</summary>
    /// <param name="tree">The syntax tree to search.</param>
    /// <param name="substring">The text to find.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The innermost node containing the substring, or <see langword="null"/> if not found.</returns>
    public static async Task<SyntaxNode?> FindInnermostNodeAsync(
        this SyntaxTree tree,
        string substring,
        CancellationToken cancellationToken)
    {
        var text = await tree.GetTextAsync(cancellationToken);
        int index = text.ToString().IndexOf(substring);
        if (index < 0)
        {
            return null;
        }

        var span = new TextSpan(index, substring.Length);
        return await tree.FindInnermostNodeAsync(span, cancellationToken);
    }

    /// <summary>Finds the first ancestor-or-self of type <typeparamref name="TSyntax"/> enclosing the first occurrence of <paramref name="substring"/>.</summary>
    /// <typeparam name="TSyntax">The syntax node type to find.</typeparam>
    /// <param name="tree">The syntax tree to search.</param>
    /// <param name="substring">The text to locate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The first enclosing node of type <typeparamref name="TSyntax"/>, or <see langword="null"/> if not found.</returns>
    public static async Task<TSyntax?> FindFirstSyntaxWithText<TSyntax>(
        this SyntaxTree tree,
        string substring,
        CancellationToken cancellationToken)
        where TSyntax : SyntaxNode
    {
        var substringNode = await tree.FindInnermostNodeAsync(substring, cancellationToken);
        return substringNode?.FirstAncestorOrSelf<TSyntax>();
    }

    /// <summary>Finds the innermost node in the tree whose span overlaps or contains <paramref name="span"/>.</summary>
    /// <param name="tree">The syntax tree to search.</param>
    /// <param name="span">The text span to find.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The innermost matching node, or <see langword="null"/> if not found.</returns>
    public static async Task<SyntaxNode?> FindInnermostNodeAsync(
        this SyntaxTree tree,
        TextSpan span,
        CancellationToken cancellationToken)
    {
        var root = await tree.GetRootAsync(cancellationToken);
        return root.FindNode(span, getInnermostNodeForTie: true);
    }

    /// <summary>Finds the first ancestor-or-self of type <typeparamref name="TSyntax"/> whose span overlaps <paramref name="span"/>.</summary>
    /// <typeparam name="TSyntax">The syntax node type to find.</typeparam>
    /// <param name="tree">The syntax tree to search.</param>
    /// <param name="span">The text span to overlap.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The first enclosing node of type <typeparamref name="TSyntax"/>, or <see langword="null"/> if not found.</returns>
    public static async Task<TSyntax?> FindSyntaxOverlappingSpan<TSyntax>(
        this SyntaxTree tree,
        TextSpan span,
        CancellationToken cancellationToken)
        where TSyntax : SyntaxNode
    {
        var substringNode = await tree.FindInnermostNodeAsync(span, cancellationToken);
        return substringNode?.FirstAncestorOrSelf<TSyntax>();
    }

    /// <summary>Returns a new syntax tree derived from <paramref name="tree"/> with <paramref name="root"/> as the new root, preserving the original options.</summary>
    /// <param name="tree">The tree to update.</param>
    /// <param name="root">The new syntax root.</param>
    /// <returns>A syntax tree with the given root and the original tree's options.</returns>
    public static SyntaxTree WithRoot(this SyntaxTree tree, SyntaxNode root)
    {
        return tree.WithRootAndOptions(root, tree.Options);
    }

    /// <summary>Returns a new syntax tree with <paramref name="oldNode"/> replaced by <paramref name="newNode"/>.</summary>
    /// <param name="tree">The tree containing the node to replace.</param>
    /// <param name="oldNode">The node to replace.</param>
    /// <param name="newNode">The replacement node.</param>
    /// <returns>A new syntax tree with the replacement applied.</returns>
    public static SyntaxTree ReplaceNode(
        this SyntaxTree tree,
        SyntaxNode oldNode,
        SyntaxNode newNode)
    {
        var root = tree.GetRoot();
        return tree.WithRoot(root.ReplaceNode(oldNode, newNode));
    }
}
