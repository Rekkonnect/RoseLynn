using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoseLynn;

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

    public static async Task<TSyntax?> FindFirstSyntaxWithText<TSyntax>(
        this SyntaxTree tree,
        string substring,
        CancellationToken cancellationToken)
        where TSyntax : SyntaxNode
    {
        var substringNode = await tree.FindInnermostNodeAsync(substring, cancellationToken);
        return substringNode?.FirstAncestorOrSelf<TSyntax>();
    }

    public static async Task<SyntaxNode?> FindInnermostNodeAsync(
        this SyntaxTree tree,
        TextSpan span,
        CancellationToken cancellationToken)
    {
        var root = await tree.GetRootAsync(cancellationToken);
        return root.FindNode(span, getInnermostNodeForTie: true);
    }

    public static async Task<TSyntax?> FindSyntaxOverlappingSpan<TSyntax>(
        this SyntaxTree tree,
        TextSpan span,
        CancellationToken cancellationToken)
        where TSyntax : SyntaxNode
    {
        var substringNode = await tree.FindInnermostNodeAsync(span, cancellationToken);
        return substringNode?.FirstAncestorOrSelf<TSyntax>();
    }

    public static SyntaxTree WithRoot(this SyntaxTree tree, SyntaxNode root)
    {
        return tree.WithRootAndOptions(root, tree.Options);
    }

    public static SyntaxTree ReplaceNode(
        this SyntaxTree tree,
        SyntaxNode oldNode,
        SyntaxNode newNode)
    {
        var root = tree.GetRoot();
        return tree.WithRoot(root.ReplaceNode(oldNode, newNode));
    }
}
