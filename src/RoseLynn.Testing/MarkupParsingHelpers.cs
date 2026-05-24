using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using RoseLynn.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace RoseLynn.Testing;

public static class MarkupParsingHelpers
{
    public static string ExtractActualSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)] string source)
    {
        TestFileMarkupParser.GetPositionsAndSpans(source, out var output, out _, out _);
        return output;
    }

    // Multiple $$ carets are not parsed correctly for refactoring tests, so we have
    // to roll our own
    public static IEnumerable<string> SplitMultiCaretMarkupToSingleCarets(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)] string source)
    {
        TestFileMarkupParser.GetPositionsAndSpans(source, out var actualCode, out var positions, out var spans);

        foreach (var position in positions)
        {
            yield return actualCode.Insert(position, "$$");
        }
    }

    public static TextSpan GetSinglePositionOrSpan(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)] string source,
        out string actualCode)
    {
        var found = TryGetSinglePositionOrSpan(source, out actualCode, out var span);
        if (!found)
        {
            throw new ArgumentException("The input did not contain a single position or span");
        }

        return span;
    }

    public static bool TryGetSinglePositionOrSpan(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)] string source,
        out string actualCode,
        out TextSpan span)
    {
        span = default;
        TestFileMarkupParser.GetPositionsAndSpans(
            source, out actualCode, out var positions, out var spans);
        if (positions.Length > 0 && spans.Count > 0)
        {
            return false;
        }

        if (positions is [var single])
        {
            span = new(single, 0);
            return true;
        }

        if (spans.IsSingle(out var singleKvp)
            && singleKvp.Value is [var singleSpan])
        {
            span = singleSpan;
            return true;
        }

        return false;
    }
}
