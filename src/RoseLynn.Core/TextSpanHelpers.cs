using Garyon.Functions;
using Microsoft.CodeAnalysis.Text;

namespace RoseLynn.Core;

/// <summary>Provides helpers for constructing <seealso cref="TextSpan"/> values.</summary>
public static class TextSpanHelpers
{
    /// <summary>Creates a <seealso cref="TextSpan"/> spanning from the start of <paramref name="left"/> to the end of <paramref name="right"/>.</summary>
    /// <param name="left">The left span; its start must be at or before <paramref name="right"/>'s start.</param>
    /// <param name="right">The right span.</param>
    /// <returns>A span from <paramref name="left"/>.Start to <paramref name="right"/>.End.</returns>
    public static TextSpan FromBounds(TextSpan left, TextSpan right)
    {
        Asserts.True(left.Start <= right.Start);

        var start = left.Start;
        var end = right.End;
        return TextSpan.FromBounds(start, end);
    }
}
