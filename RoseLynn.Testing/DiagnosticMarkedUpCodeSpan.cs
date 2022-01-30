#nullable enable

using Microsoft.CodeAnalysis.Text;

namespace RoseLynn.Testing;

/// <summary></summary>
public class DiagnosticMarkedUpCodeSpan
{
    /// <summary>The span of the text at which a diagnostic is expected.</summary>
    public TextSpan TextSpan { get; }
    /// <summary>The diagnostic ID that is expected to be emitted in the given span, or <see langword="null"/> if no specific ID is expected.</summary>
    public string? ExpectedDiagnosticID { get; }

    /// <summary>Initializes a new instance of the <seealso cref="DiagnosticMarkedUpCodeSpan"/> class.</summary>
    /// <param name="start">The starting index of the span in the code document.</param>
    /// <param name="length">The length of the span in the code document.</param>
    /// <param name="expectedDiagnosticID">The diagnostic ID that is expected to be emitted in the given span, or <see langword="null"/> if no specific ID is expected.</param>
    public DiagnosticMarkedUpCodeSpan(int start, int length, string? expectedDiagnosticID)
    {
        TextSpan = new(start, length);
        ExpectedDiagnosticID = expectedDiagnosticID;
    }
}
