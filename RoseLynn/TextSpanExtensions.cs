using Microsoft.CodeAnalysis.Text;

namespace RoseLynn;

/// <summary>Contains extensions for the <seealso cref="TextSpan"/> struct.</summary>
public static class TextSpanExtensions
{
    /// <summary>Creates a new <seealso cref="TextSpan"/> instance with the start offset by the specified amount.</summary>
    /// <param name="textSpan">The original <seealso cref="TextSpan"/>.</param>
    /// <param name="startOffset">The offset to apply to the start.</param>
    /// <returns>A new <seealso cref="TextSpan"/> instance with the start offset by <paramref name="startOffset"/>.</returns>
    public static TextSpan WithOffsetStart(this in TextSpan textSpan, int startOffset)
    {
        return textSpan.WithOffset(startOffset, 0);
    }
    /// <summary>Creates a new <seealso cref="TextSpan"/> instance with the end offset by the specified amount.</summary>
    /// <param name="textSpan">The original <seealso cref="TextSpan"/>.</param>
    /// <param name="endOffset">The offset to apply to the end.</param>
    /// <returns>A new <seealso cref="TextSpan"/> instance with the end offset by <paramref name="endOffset"/>.</returns>
    public static TextSpan WithOffsetEnd(this in TextSpan textSpan, int endOffset)
    {
        return textSpan.WithOffset(0, endOffset);
    }
    /// <summary>Creates a new <seealso cref="TextSpan"/> instance with the start and end offset by the specified amount.</summary>
    /// <param name="textSpan">The original <seealso cref="TextSpan"/>.</param>
    /// <param name="startOffset">The offset to apply to the start.</param>
    /// <param name="endOffset">The offset to apply to the end.</param>
    /// <returns>A new <seealso cref="TextSpan"/> instance with the start offset by <paramref name="startOffset"/> and the end offset by <paramref name="endOffset"/>.</returns>
    public static TextSpan WithOffset(this in TextSpan textSpan, int startOffset, int endOffset)
    {
        return TextSpan.FromBounds(textSpan.Start + startOffset, textSpan.End + endOffset);
    }
}
