using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Contains extensions for the <seealso cref="Location"/> class.</summary>
public static class LocationExtensions
{
    /// <summary>Offsets a given <seealso cref="Location"/>'s start and end by a given offset.</summary>
    /// <param name="location">The <seealso cref="Location"/> instance to offset.</param>
    /// <param name="startOffset">The offset to apply to the start of the location.</param>
    /// <param name="endOffset">The offset to apply to the end of the location.</param>
    /// <returns>The offset location, with its start and end locations adjusted by the specified amount.</returns>
    public static Location WithOffset(this Location location, int startOffset, int endOffset)
    {
        var offsetSpan = location.SourceSpan.WithOffset(startOffset, endOffset);
        // TODO: This will probably explode in our faces if the location is not a source tree one,
        // but a file path one. Ensure that we properly test against this, and properly handle
        // the type of location to create
        return Location.Create(location.SourceTree!, offsetSpan);
    }

    /// <summary>Gets the innermost syntax node at this location's source span, or <see langword="null"/> if the location has no source tree.</summary>
    /// <param name="location">The location to resolve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The innermost node at the location's span, or <see langword="null"/>.</returns>
    public static async Task<SyntaxNode?> GetNodeAsync(
        this Location location, CancellationToken cancellationToken)
    {
        var tree = location.SourceTree;
        if (tree is null)
        {
            return null;
        }
        var root = await tree.GetRootAsync(cancellationToken);
        return root?.FindNode(location.SourceSpan);
    }
}
