using RoseLynn.Utilities;
using System.Threading.Tasks;

namespace RoseLynn.Testing.Test;

/// <summary>Tests the Microsoft.CodeAnalysis-style diagnostic markup handler.</summary>
public class MicrosoftCodeAnalysisDiagnosticMarkupCodeHandlerTests : DiagnosticMarkupCodeHandlerTestsBase
{
    [Test]
    public override async Task RemoveMarkupTest()
    {
        var markedUp = DiagnosticMarkupCodeHandler.MicrosoftCodeAnalysis.RemoveMarkup("int {|CS0101:value|} = {|CS0202:5|};");
        await Assert.That(markedUp).IsEqualTo("int value = 5;");
    }

    [Test]
    public async Task GetDiagnosticMarkedUpSpansTest()
    {
        var spans = DiagnosticMarkupCodeHandler.MicrosoftCodeAnalysis.GetDiagnosticMarkedUpSpans("int {|CS0101:value|} = {|CS0202:5|};", out var unmarkedCode);

        await AssertMarkupSpan(spans[0], "value", "CS0101");
        await AssertMarkupSpan(spans[1], "5", "CS0202");

        async Task AssertMarkupSpan(DiagnosticMarkedUpCodeSpan span, string node, string expectedDiagnosticID)
        {
            await Assert.That(unmarkedCode!.Substring(span.TextSpan)).IsEqualTo(node);
            await Assert.That(span.ExpectedDiagnosticID).IsEqualTo(expectedDiagnosticID);
        }
    }
}
