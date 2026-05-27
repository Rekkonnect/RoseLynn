using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using RoseLynn.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace RoseLynn.Testing;

/// <summary>Provides helpers for parsing Roslyn test-markup annotations from source strings.</summary>
public static class MarkupParsingHelpers
{
    /// <summary>Strips all test-markup annotations from <paramref name="source"/> and returns the plain source text.</summary>
    /// <param name="source">The markup-annotated C# source.</param>
    /// <returns>The source with all position and span markers removed.</returns>
    public static string ExtractActualSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)] string source)
    {
        TestFileMarkupParser.GetPositionsAndSpans(source, out var output, out _, out _);
        return output;
    }

    // Multiple $$ carets are not parsed correctly for refactoring tests, so we have
    // to roll our own
    /// <summary>Splits a multi-caret markup source into separate single-caret markup strings, one per caret position.</summary>
    /// <param name="source">The markup-annotated C# source containing multiple <c>$$</c> carets.</param>
    /// <returns>One markup string per caret, each containing only that caret.</returns>
    public static IEnumerable<string> SplitMultiCaretMarkupToSingleCarets(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)] string source)
    {
        TestFileMarkupParser.GetPositionsAndSpans(source, out var actualCode, out var positions, out var spans);

        foreach (var position in positions)
        {
            yield return actualCode.Insert(position, "$$");
        }
    }

    /// <summary>Extracts the single position or span from <paramref name="source"/>, throwing if none or ambiguous.</summary>
    /// <param name="source">The markup-annotated C# source.</param>
    /// <param name="actualCode">The plain source text with annotations stripped.</param>
    /// <returns>The extracted <see cref="TextSpan"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the source does not contain exactly one position or span.</exception>
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

    /// <summary>Attempts to extract a single position or span from <paramref name="source"/>.</summary>
    /// <param name="source">The markup-annotated C# source.</param>
    /// <param name="actualCode">The plain source text with annotations stripped.</param>
    /// <param name="span">The extracted <see cref="TextSpan"/> if found; otherwise default.</param>
    /// <returns><see langword="true"/> if exactly one position or span was found; otherwise <see langword="false"/>.</returns>
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
