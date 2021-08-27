#nullable enable

using System.Collections.Immutable;

namespace RoseLynn.Testing
{
    public class DiagnosticMarkedUpDocument
    {
        public string UnmarkedCode { get; }
        public ImmutableArray<DiagnosticMarkedUpCodeSpan> MarkedUpSpans { get; }

        public DiagnosticMarkedUpDocument(string unmarkedCode, ImmutableArray<DiagnosticMarkedUpCodeSpan> markedUpSpans)
        {
            UnmarkedCode = unmarkedCode;
            MarkedUpSpans = markedUpSpans;
        }
    }
}
