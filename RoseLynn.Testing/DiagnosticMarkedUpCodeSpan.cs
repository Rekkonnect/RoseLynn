#nullable enable

using Microsoft.CodeAnalysis.Text;

namespace RoseLynn.Testing
{
    public class DiagnosticMarkedUpCodeSpan
    {
        public TextSpan TextSpan { get; }
        public string? ExpectedDiagnosticID { get; }

        public DiagnosticMarkedUpCodeSpan(int start, int length, string? expectedDiagnosticID)
        {
            TextSpan = new(start, length);
            ExpectedDiagnosticID = expectedDiagnosticID;
        }
    }
}
