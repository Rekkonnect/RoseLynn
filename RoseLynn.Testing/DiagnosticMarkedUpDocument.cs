#nullable enable

using System.Collections.Immutable;

namespace RoseLynn.Testing;

/// <summary>Contains information regarding a testing code document containing expected diagnostic information at specified spans of the document.</summary>
public class DiagnosticMarkedUpDocument
{
    /// <summary>The actual code document without any expected diagnostic markup indicators.</summary>
    public string UnmarkedCode { get; }
    /// <summary>The spans of the document that are marked up with expected diagnostics.</summary>
    public ImmutableArray<DiagnosticMarkedUpCodeSpan> MarkedUpSpans { get; }

    /// <summary>Initializes a new instance of the <seealso cref="DiagnosticMarkedUpDocument"/> class.</summary>
    /// <param name="unmarkedCode">The unmarked code document.</param>
    /// <param name="markedUpSpans">The marked up spans of the document.</param>
    public DiagnosticMarkedUpDocument(string unmarkedCode, ImmutableArray<DiagnosticMarkedUpCodeSpan> markedUpSpans)
    {
        UnmarkedCode = unmarkedCode;
        MarkedUpSpans = markedUpSpans;
    }
}
