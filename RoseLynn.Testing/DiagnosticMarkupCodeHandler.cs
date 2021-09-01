#nullable enable

using RoseLynn.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RoseLynn.Testing
{
    /// <summary>Provides a handler to perform operations on code with special markup syntax for diagnostics.</summary>
    public abstract class DiagnosticMarkupCodeHandler
    {
        /// <summary>Gets the <seealso cref="MicrosoftCodeAnalysisDiagnosticMarkupCodeHandler"/> instance.</summary>
        public static readonly MicrosoftCodeAnalysisDiagnosticMarkupCodeHandler MicrosoftCodeAnalysis = new();
        /// <summary>Gets the <seealso cref="GuRoslynAssertsDiagnosticMarkupCodeHandler"/> instance.</summary>
        public static readonly GuRoslynAssertsDiagnosticMarkupCodeHandler GuRoslynAsserts = new();

        private DiagnosticIndicatorInfo? indicatorInfo;
        /// <summary>Gets the <seealso cref="DiagnosticIndicatorInfo"/> for this markup style.</summary>
        public DiagnosticIndicatorInfo IndicatorInfo => indicatorInfo ??= GetIndicatorInfo();

        /// <summary>Initializes the new instance of the <seealso cref="DiagnosticIndicatorInfo"/> for the style.</summary>
        /// <returns>The initialized instance.</returns>
        /// <remarks>This method is only meant to be called once during initialization of the <seealso cref="IndicatorInfo"/> property.</remarks>
        protected abstract DiagnosticIndicatorInfo GetIndicatorInfo();

        /// <summary>The string indicator that a diagnostic is expected to start at that point.</summary>
        public string DiagnosticIndicatorStart => IndicatorInfo.Start;
        /// <summary>The string indicator that a diagnostic is expected to end at that point.</summary>
        /// <remarks>The returned value is <see langword="null"/> or <seealso cref="string.Empty"/> if the framework does not support ending indicators.</remarks>
        public string? DiagnosticIndicatorEnd => IndicatorInfo.End;
        /// <summary>The string indicator that a bound diagnostic with the specified diagnostic ID is expected to start at that point.</summary>
        /// <remarks>The returned value is <see langword="null"/> or <seealso cref="string.Empty"/> if the framework does not support bound diagnostic indicators.</remarks>
        public string? BoundDiagnosticIndicatorStart => IndicatorInfo.BoundStart;
        /// <summary>The string indicator that a bound diagnostic with the specified diagnostic ID is expected to end at that point.</summary>
        /// <remarks>The returned value is <see langword="null"/> or <seealso cref="string.Empty"/> if the framework does not support bound diagnostic indicators.</remarks>
        public string? BoundDiagnosticIndicatorEnd => IndicatorInfo.BoundEnd;

        /// <summary>Removes the markup syntax for diagnostics from the given code.</summary>
        /// <param name="code">The marked up code to remove all the markup syntax from.</param>
        /// <returns>The resulting code without any markup syntax for diagnostics.</returns>
        public abstract string RemoveMarkup(string code);

        /// <summary>Marks the given node as a raw string with diagnostic markup.</summary>
        /// <param name="rawStringNode">The node to be marked up as a raw string.</param>
        /// <returns>The marked up version of the node. No information regarding the diagnostic ID is provided during marking it up.</returns>
        [ExcludeFromCodeCoverage]
        public virtual string MarkupDiagnostic(string rawStringNode)
        {
            return $"{IndicatorInfo.Start}{rawStringNode}{IndicatorInfo.End}";
        }
        /// <summary>Marks the given node as a raw string with diagnostic markup.</summary>
        /// <param name="rawStringNode">The node to be marked up as a raw string.</param>
        /// <param name="expectedDiagnosticID">The expected diagnostic ID that will be included in the markup, if supported by the framework markup. <see langword="null"/> is not handled.</param>
        /// <returns>The marked up version of the node. Information regarding the expected diagnostic ID may or may not be included.</returns>
        /// <remarks>By default, this calls the <seealso cref="MarkupDiagnostic(string)"/> method. If the implemented framework handles bound diagnostic ID markup, this method must be overridden.</remarks>
        [ExcludeFromCodeCoverage]
        public virtual string MarkupDiagnostic(string rawStringNode, string expectedDiagnosticID) => MarkupDiagnostic(rawStringNode);

        /// <summary>Marks the given node as a raw string with diagnostic markup.</summary>
        /// <param name="rawStringNode">The node to be marked up as a raw string.</param>
        /// <param name="expectedDiagnosticID">The expected diagnostic ID that will be included in the markup, if supported by the framework markup. If <see langword="null"/>, the default markup for unbound expected diagnostics will be used.</param>
        /// <returns>The marked up version of the node. Information regarding the expected diagnostic ID may or may not be included.</returns>
        public string MarkupPotentiallyBoundDiagnostic(string rawStringNode, string? expectedDiagnosticID)
        {
            if (expectedDiagnosticID is null)
                return MarkupDiagnostic(rawStringNode);

            return MarkupDiagnostic(rawStringNode, expectedDiagnosticID);
        }

        /// <summary>Parses a bound diagnostic indicator start.</summary>
        /// <param name="markupCode">The given marked up code.</param>
        /// <param name="startIndex">The index of the first character of the diagnostic indicator start.</param>
        /// <param name="length">The length of the diagnostic indicator's start. If parsing fails, 0 is returned.</param>
        /// <returns>The expected diagnostic ID as stored in the bound diagnostic indicator, if successfully parsed; otherwise <see langword="null"/>. If the diagnostic indicator does not start at <paramref name="startIndex"/>, parsing fails, also returning <see langword="null"/>.</returns>
        protected virtual string? ParseBoundDiagnosticIndicator(string markupCode, int startIndex, out int length)
        {
            length = 0;
            return null;
        }

        /// <summary>Gets the spans of the original code that are marked up with expected diagnostics.</summary>
        /// <param name="markupCode">The diagnostic marked up code whose spans to get.</param>
        /// <returns>A <seealso cref="DiagnosticMarkedUpDocument"/> instance containing the resulting information.</returns>
        public DiagnosticMarkedUpDocument GetDiagnosticMarkedUpSpans(string markupCode)
        {
            var spans = GetDiagnosticMarkedUpSpans(markupCode, out var unmarkedCode);
            return new(unmarkedCode, spans);
        }

        /// <summary>Gets the spans of the original code that are marked up with expected diagnostics.</summary>
        /// <param name="markupCode">The diagnostic marked up code whose spans to get.</param>
        /// <param name="unmarkedCode">The unmarked version of the code.</param>
        /// <returns>An <seealso cref="ImmutableArray{T}"/> of <seealso cref="DiagnosticMarkedUpCodeSpan"/>s reflecting the spans of <paramref name="unmarkedCode"/> that are marked with diagnostics.</returns>
        public ImmutableArray<DiagnosticMarkedUpCodeSpan> GetDiagnosticMarkedUpSpans(string markupCode, out string unmarkedCode)
        {
            var result = ImmutableArray.CreateBuilder<DiagnosticMarkedUpCodeSpan>();

            int unboundStart = 0;
            int boundStart = 0;

            if (string.IsNullOrEmpty(IndicatorInfo.BoundStart))
                boundStart = -1;

            int eatenMarkupCharacters = 0;

            void MoveNextUnbound()
            {
                if (unboundStart >= 0)
                    unboundStart = markupCode.IndexOfAfter(IndicatorInfo.Start, unboundStart);
            }
            void MoveNextBound()
            {
                if (boundStart >= 0)
                    boundStart = markupCode.IndexOfAfter(IndicatorInfo.BoundStart, boundStart);
            }

            MoveNextUnbound();
            MoveNextBound();

            Action moveNextAction = MoveNextBound;

            while (true)
            {
                int length = 0;
                int boundIndicatorLength = 0;
                int endIndicatorLength = 0;
                ref int start = ref unboundStart;
                string? expectedDiagnosticID = null;

                static void GetLength(string markupCode, string? indicatorEnd, int indicatorStartIndex, ref int length)
                {
                    if (string.IsNullOrEmpty(indicatorEnd))
                        return;
                    
                    length = markupCode.IndexOf(indicatorEnd, indicatorStartIndex) - indicatorStartIndex;
                }

                switch ((unboundStart, boundStart))
                {
                    case ( < 0, < 0):
                        goto loopEnd;

                    case ( < 0, _):
                        goto bound;

                    case (_, < 0):
                        goto unbound;

                    default:
                        if (boundStart < unboundStart)
                        {
                            // The indicator is for a bound diagnostic
                            goto bound;
                        }
                        else if (boundStart == unboundStart)
                        {
                            // For the case that the framework markup syntax provides the same indicators
                            // attempt to see which is the used one

                            // This prechecks if the bound diagnostic indicator is present
                            // There could theoretically also be a validation of the given syntax from its ending indicators,
                            // however the implementation is not worth enough
                            AnalyzeBoundDiagnosticIndicator();
                            if (expectedDiagnosticID is not null)
                                goto bound;

                            goto unbound;
                        }
                        else
                        {
                            // The indicator is for an unbound diagnostic
                            goto unbound;
                        }

                    bound:
                        start = ref boundStart;
                        GetLength(markupCode, BoundDiagnosticIndicatorEnd, boundStart, ref length);
                        endIndicatorLength = BoundDiagnosticIndicatorEnd?.Length ?? 0;

                        if (expectedDiagnosticID is null)
                            AnalyzeBoundDiagnosticIndicator();
                        eatenMarkupCharacters += boundIndicatorLength;
                        moveNextAction = MoveNextBound;
                        break;

                        void AnalyzeBoundDiagnosticIndicator()
                        {
                            int indicatorStart = boundStart - BoundDiagnosticIndicatorStart!.Length;
                            expectedDiagnosticID = ParseBoundDiagnosticIndicator(markupCode, indicatorStart, out boundIndicatorLength);
                            int newBoundStart = indicatorStart + boundIndicatorLength;
                            int difference = newBoundStart - boundStart;
                            length -= difference;
                            boundStart = newBoundStart;
                        }

                    unbound:
                        start = ref unboundStart;
                        GetLength(markupCode, DiagnosticIndicatorEnd, unboundStart, ref length);
                        endIndicatorLength = DiagnosticIndicatorEnd?.Length ?? 0;

                        eatenMarkupCharacters += DiagnosticIndicatorStart.Length;
                        moveNextAction = MoveNextUnbound;
                        break;
                }

                var span = new DiagnosticMarkedUpCodeSpan(start - eatenMarkupCharacters, length, expectedDiagnosticID);
                result.Add(span);

                // Forward next start
                eatenMarkupCharacters += endIndicatorLength;
                start += span.TextSpan.Length;
                moveNextAction();
            }

        loopEnd:
            unmarkedCode = RemoveMarkup(markupCode);

            return result.ToImmutable();
        }

        /// <inheritdoc cref="ConvertMarkup(string, DiagnosticMarkupCodeHandler)"/>
        /// <typeparam name="TOther">The type of the other <seealso cref="DiagnosticMarkupCodeHandler"/> according to which to convert the marked up document for.</typeparam>
        public string ConvertMarkup<TOther>(string markupCode)
            where TOther : DiagnosticMarkupCodeHandler, new()
        {
            return ConvertMarkup(markupCode, new TOther());
        }
        /// <summary>Converts the diagnostic markup for marked up code into another diagnostic markup style, handled by the specified handler instance.</summary>
        /// <param name="markupCode">The original marked up code whose diagnostic markup style to convert.</param>
        /// <param name="newHandlerInstance">The handler instance that supports the diagnostic markup style that the original code is being converted to.</param>
        /// <returns>The converted markup code.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the current handler does not support diagnostic indicator ending strings, while the new one does.</exception>
        public string ConvertMarkup(string markupCode, DiagnosticMarkupCodeHandler newHandlerInstance)
        {
            if (string.IsNullOrEmpty(DiagnosticIndicatorEnd))
            {
                if (string.IsNullOrEmpty(newHandlerInstance.DiagnosticIndicatorEnd))
                    return markupCode.Replace(DiagnosticIndicatorStart, newHandlerInstance.DiagnosticIndicatorStart);

                throw new InvalidOperationException("The current handler does not support diagnostic indicator ending strings, while the new handler supports them.");
            }

            var spans = GetDiagnosticMarkedUpSpans(markupCode, out var unmarkedCode);
            return newHandlerInstance.MarkupDiagnostics(unmarkedCode, spans);
        }

        /// <summary>Marks up the given code document with diagnostic indicators at the specified spans.</summary>
        /// <param name="code">The original code docuemnt on which to mark diagnostic indicators on. Already existing indicators will not be affected or accounted.</param>
        /// <param name="spans">The spans of the document to mark up with diagnostic indicators.</param>
        /// <returns>The resulting code document marked up with diagnostic indicators at the specified spans.</returns>
        public string MarkupDiagnostics(string code, IEnumerable<DiagnosticMarkedUpCodeSpan> spans)
        {
            var result = new StringBuilder(code.Length * 2);

            int lastSpanEnd = 0;
            if (!spans.EnumeratePerformAction(null, PerformLast, AppendIndicator))
                return code;
            return result.ToString();

            void PerformLast(DiagnosticMarkedUpCodeSpan lastSpan)
            {
                result.Append(code, lastSpan.TextSpan.End);
            }
            void AppendIndicator(DiagnosticMarkedUpCodeSpan span)
            {
                result.Append(code, lastSpanEnd, span.TextSpan.Start - lastSpanEnd);
                result.Append(MarkupPotentiallyBoundDiagnostic(code.Substring(span.TextSpan), span.ExpectedDiagnosticID));
                lastSpanEnd = span.TextSpan.End;
            }
        }
    }

    /// <summary>Provides an implementation of the <seealso cref="DiagnosticMarkupCodeHandler"/> for the Microsoft.CodeAnalysis.Testing framework.</summary>
    public sealed class MicrosoftCodeAnalysisDiagnosticMarkupCodeHandler : DiagnosticMarkupCodeHandler
    {
        private const string expectedDiagnosticID = nameof(expectedDiagnosticID);
        private static readonly Regex diagnosticMarkupRegex = new($@"{{\|(?'{expectedDiagnosticID}'[\w\d]*):", RegexOptions.Compiled);

        /// <inheritdoc/>
        protected override DiagnosticIndicatorInfo GetIndicatorInfo()
        {
            return DiagnosticIndicatorInfo.AnyDiagnostic("[|", "|]", "{|", "|}");
        }

        /// <inheritdoc/>
        protected override string? ParseBoundDiagnosticIndicator(string markupCode, int startIndex, out int length)
        {
            var match = diagnosticMarkupRegex.Match(markupCode, startIndex);

            length = 0;
            if (!match.Success)
                return null;

            if (match.Index != startIndex)
                return null;

            length = match.Length;
            return match.Groups[expectedDiagnosticID].Value;
        }

        /// <inheritdoc/>
        public override string RemoveMarkup(string code)
        {
            return diagnosticMarkupRegex.Replace(code, "").Remove("|}").Remove("[|").Remove("|]");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override string MarkupDiagnostic(string rawStringNode)
        {
            return $"[|{rawStringNode}|]";
        }
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override string MarkupDiagnostic(string rawStringNode, string expectedDiagnosticID)
        {
            return $"{{|{expectedDiagnosticID}:{rawStringNode}|}}";
        }
    }

    /// <summary>Provides an implementation of the <seealso cref="DiagnosticMarkupCodeHandler"/> for the Gu.Roslyn.Asserts framework.</summary>
    public sealed class GuRoslynAssertsDiagnosticMarkupCodeHandler : DiagnosticMarkupCodeHandler
    {
        /// <inheritdoc/>
        protected override DiagnosticIndicatorInfo GetIndicatorInfo()
        {
            return DiagnosticIndicatorInfo.StartUnboundDiagnostic("↓");
        }

        /// <inheritdoc/>
        public override string RemoveMarkup(string code)
        {
            return code.Remove("↓");
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override string MarkupDiagnostic(string rawStringNode)
        {
            return $"↓{rawStringNode}";
        }
    }
}
