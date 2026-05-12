using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn;

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

        return newRoot.FullSpan.Length - originalRoot.FullSpan.Length;
    }

    public static async ValueTask<Document> WithSyntaxRootAsync
    (
        this Document document,
        Func<SyntaxNode, SyntaxNode> selectNode,
        CancellationToken cancellationToken = default
    )
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
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
}
