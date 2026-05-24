namespace RoseLynn;

public static class SpanExtensions
{
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
