using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Polyfills;
using System.Text;

namespace RoseLynn.Core;

/// <summary>Provides extensions for <seealso cref="Document"/>.</summary>
public static class DocumentExtensions
{
    /// <summary>Gets the length difference between the new document and the original one.</summary>
    /// <param name="newDocument">The new document.</param>
    /// <param name="originalDocument">The original document.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A positive value if the new document is larger than the original, a negative if the original is larger than the new one, or 0 if both have the same length. The documents' full span is taken into account when calculating the difference.</returns>
    public static async Task<int> LengthDifferenceFrom(this Document newDocument, Document originalDocument, CancellationToken cancellationToken = default)
    {
        var originalRootTask = originalDocument.GetSyntaxRootAsync(cancellationToken);
        var newRootTask = newDocument.GetSyntaxRootAsync(cancellationToken);

        var originalRoot = await originalRootTask;
        var newRoot = await newRootTask;

        ArgumentNullException.ThrowIfNull(originalRoot);
        ArgumentNullException.ThrowIfNull(newRoot);

        return newRoot.FullSpan.Length - originalRoot.FullSpan.Length;
    }

    /// <summary>Applies a transformation to the syntax root of <paramref name="document"/> and returns the updated document.</summary>
    /// <param name="document">The document whose root to transform.</param>
    /// <param name="selectNode">A function that receives the current root and returns the new root.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document with the transformed syntax root.</returns>
    public static async ValueTask<Document> WithSyntaxRootAsync
    (
        this Document document,
        Func<SyntaxNode, SyntaxNode> selectNode,
        CancellationToken cancellationToken = default
    )
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        ArgumentNullException.ThrowIfNull(root);
        return document.WithSyntaxRoot(selectNode(root));
    }

    /// <summary>Removes the text at the specified location in the <seealso cref="Document"/>.</summary>
    /// <param name="document">The <seealso cref="Document"/> whose text to remove at the specified location.</param>
    /// <param name="location">The location of the text to remove.</param>
    /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> for the operation.</param>
    /// <returns>A <seealso cref="Task{TResult}"/> wrapping the resulting <seealso cref="Document"/> without the removed text at the specified location.</returns>
    public static async Task<Document> RemoveText(this Document document, Location location, CancellationToken cancellationToken = default)
    {
        return await RemoveText(document, location.SourceSpan, cancellationToken);
    }

    /// <summary>Removes the text at the specified location in the <seealso cref="Document"/>.</summary>
    /// <param name="document">The <seealso cref="Document"/> whose text to remove at the specified location.</param>
    /// <param name="location">The location of the text to remove.</param>
    /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> for the operation.</param>
    /// <returns>A <seealso cref="Task{TResult}"/> wrapping the resulting <seealso cref="Document"/> without the removed text at the specified location.</returns>
    public static async Task<Document> RemoveText(this Document document, TextSpan location, CancellationToken cancellationToken = default)
    {
        var originalText = await document.GetTextAsync(cancellationToken);
        var newText = originalText.Replace(location, "");
        return document.WithText(newText);
    }

    /// <summary>
    /// Changes the text content of the <seealso cref="Document"/> to the specified content,
    /// while preserving the original encoding and checksum algorithm if possible.
    /// If the text content of the given <seealso cref="Document"/> cannot be retrieved,
    /// the new text content will be created with <see cref="Encoding.UTF8"/> encoding
    /// and <see cref="SourceHashAlgorithm.Sha256"/> as the checksum algorithm by default.
    /// </summary>
    /// <param name="document">The <seealso cref="Document"/> whose text content to change.</param>
    /// <param name="content">The new text content.</param>
    /// <returns>The <seealso cref="Document"/> with the updated text content.</returns>
    public static Document WithTextContent(this Document document, string content)
    {
        _ = document.TryGetText(out var text);
        var resultText = SourceText.From(
            content,
            text?.Encoding ?? Encoding.UTF8,
            text?.ChecksumAlgorithm ?? SourceHashAlgorithm.Sha256);
        return document.WithText(resultText);
    }

    extension(Document document)
    {
        /// <summary>Gets the <seealso cref="Microsoft.CodeAnalysis.Solution"/> that contains this document.</summary>
        public Solution Solution => document.Project.Solution;

        /// <summary>
        /// Finds a node in the expected position after the replacement of a node
        /// with another one. The expected position is calculated by the old node's
        /// position in the original tree and the new node's length.
        /// </summary>
        /// <param name="document">
        /// The new document that contains the replaced node.
        /// </param>
        /// <param name="oldNode">
        /// The old node that was replaced. The new document should not contain this
        /// node.
        /// <br/>
        /// The old node must belong to a syntax tree in order to calculate the
        /// replacement's position in the new syntax tree.
        /// </param>
        /// <param name="newNode">
        /// The new node that replaced the old node. It is expected that this method
        /// is invoked only because <paramref name="newNode"/> is not present in the
        /// provided document instance.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token to cancel the querying operation.
        /// </param>
        /// <remarks>
        /// Using this method is necessary after replacement if we want to track a
        /// node without traversing the entire tree to discover it via annotations.
        /// <br/>
        /// This method forces building the syntax tree of the entire document and
        /// can therefore be somewhat costly.
        /// </remarks>
        public async Task<SyntaxNode> FindReplacedNode(
            SyntaxNode oldNode,
            SyntaxNode newNode,
            CancellationToken cancellationToken)
        {
            if (oldNode.SyntaxTree is null)
            {
                throw new InvalidOperationException(
                    """
                    The old node must belong to a syntax tree in order to calculate
                    the replacement's position in the new syntax tree.
                    """);
            }

            var start = oldNode.FullSpan.Start;
            var length = newNode.FullSpan.Length;
            var span = new TextSpan(start, length);
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            return root!.FindNode(span);
        }

        /// <summary>
        /// Finds a node in the expected position after a mutation in the previous
        /// tree that held the requested node. If the code before the node's span
        /// has not been textually resized, this method will correctly find the node
        /// within the new tree, by querying the span in which it belonged in the
        /// previous tree.
        /// </summary>
        /// <param name="document">
        /// The new document that contains the replaced node.
        /// </param>
        /// <param name="previousTreeNode">
        /// The node in the previous tree to find in the new document. The previous
        /// tree node must have a non-null <see cref="SyntaxNode.SyntaxTree"/> to
        /// ensure that the node comes from a previous version of the syntax tree.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token to cancel the querying operation.
        /// </param>
        /// <remarks>
        /// This method forces building the syntax tree of the entire document and
        /// can therefore be somewhat costly.
        /// </remarks>
        public async Task<SyntaxNode> FindNode(
            SyntaxNode previousTreeNode,
            CancellationToken cancellationToken)
        {
            if (previousTreeNode.SyntaxTree is null)
            {
                throw new InvalidOperationException(
                    """
                    The previous tree node must belong to a syntax tree to properly
                    evaluate the new node's position in the new tree.
                    """);
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            return root!.FindNode(previousTreeNode.Span);
        }

        /// <summary>Replaces <paramref name="old"/> with <paramref name="new"/> in the document, throwing if <paramref name="old"/> is not found in the document's syntax tree.</summary>
        /// <param name="old">The node to replace; must belong to this document's syntax tree.</param>
        /// <param name="new">The replacement node.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated document.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="old"/> does not belong to this document's syntax tree.</exception>
        public async Task<Document> ReplaceRequiredNodeAsync(
            SyntaxNode? old,
            SyntaxNode @new,
            CancellationToken cancellationToken)
        {
            var tree = await document.GetSyntaxTreeAsync(cancellationToken);
            if (old?.SyntaxTree != tree)
            {
                throw new InvalidOperationException(
                    "The syntax tree does not contain the old node");
            }

            return await document.ReplaceRequiredNodeAsync(old, @new, cancellationToken);
        }
    }

    /// <summary>Asynchronously enumerates the non-null syntax roots of all documents in <paramref name="documents"/>.</summary>
    /// <param name="documents">The documents whose roots to enumerate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The non-null syntax roots.</returns>
    public static async Task<IEnumerable<SyntaxNode>> EnumerateNotNullNodes(
        this IEnumerable<Document> documents,
        CancellationToken cancellationToken)
    {
        var tasks = documents
            .Select(async document => await document.GetSyntaxRootAsync(cancellationToken))
            ;
        await Task.WhenAll(tasks);
        var nodes = tasks.Select(task => task.Result).ToList();
        return nodes.WhereNotNull();
    }

    /// <summary>Asynchronously collects the non-null syntax roots of all documents in <paramref name="documents"/> into a read-only collection.</summary>
    /// <param name="documents">The documents whose roots to collect.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only collection of non-null syntax roots.</returns>
    public static async Task<IReadOnlyCollection<SyntaxNode>> GetAllNotNullNodes(
        this IEnumerable<Document> documents,
        CancellationToken cancellationToken)
    {
        var nodes = await EnumerateNotNullNodes(documents, cancellationToken);
        return nodes.ToList();
    }
}
