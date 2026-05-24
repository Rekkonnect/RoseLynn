using Garyon.Functions;
using Microsoft.CodeAnalysis.Text;

namespace RoseLynn;

public static class TextSpanHelpers
{
    public static TextSpan FromBounds(TextSpan left, TextSpan right)
    {
        Asserts.True(left.Start <= right.Start);

        var start = left.Start;
        var end = right.End;
        return TextSpan.FromBounds(start, end);
    }
}
