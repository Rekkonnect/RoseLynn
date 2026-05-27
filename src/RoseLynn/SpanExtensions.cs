namespace RoseLynn;

/// <summary>Provides extension methods for <see cref="ReadOnlySpan{T}"/>.</summary>
public static class SpanExtensions
{
    /// <summary>Splits <paramref name="span"/> on the last occurrence of <paramref name="delimiter"/>.</summary>
    /// <param name="span">The span to split.</param>
    /// <param name="delimiter">The character to split on.</param>
    /// <param name="left">The portion before the last delimiter; default if not found.</param>
    /// <param name="right">The portion after the last delimiter, or the full span if not found.</param>
    /// <returns><see langword="true"/> if <paramref name="delimiter"/> was found; otherwise <see langword="false"/>.</returns>
    public static bool SplitOnceRight(
        this ReadOnlySpan<char> span,
        char delimiter,
        out ReadOnlySpan<char> left,
        out ReadOnlySpan<char> right)
    {
        var index = span.LastIndexOf(delimiter);
        if (index < 0)
        {
            right = span;
            left = default;
            return false;
        }

        left = span.Slice(0, index);
        right = span.Slice(index + 1);
        return true;
    }
}
