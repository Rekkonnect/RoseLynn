using Microsoft.CodeAnalysis;

namespace RoseLynn;

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
        return Location.Create(location.SourceTree, offsetSpan);
    }
}
