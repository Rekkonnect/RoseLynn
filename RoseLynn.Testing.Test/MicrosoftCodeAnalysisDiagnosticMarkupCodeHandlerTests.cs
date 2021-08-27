using NUnit.Framework;
using RoseLynn.Utilities;

namespace RoseLynn.Testing.Test
{
    public class MicrosoftCodeAnalysisDiagnosticMarkupCodeHandlerTests : DiagnosticMarkupCodeHandlerTestsBase
    {
        [Test]
        public override void RemoveMarkupTest()
        {
            var markedUp = DiagnosticMarkupCodeHandler.MicrosoftCodeAnalysis.RemoveMarkup("int {|CS0101:value|} = {|CS0202:5|};");
            Assert.AreEqual("int value = 5;", markedUp);
        }

        [Test]
        public void GetDiagnosticMarkedUpSpansTest()
        {
            var spans = DiagnosticMarkupCodeHandler.MicrosoftCodeAnalysis.GetDiagnosticMarkedUpSpans("int {|CS0101:value|} = {|CS0202:5|};", out var unmarkedCode);

            AssertMarkupSpan(spans[0], "value", "CS0101");
            AssertMarkupSpan(spans[1], "5", "CS0202");

            void AssertMarkupSpan(DiagnosticMarkedUpCodeSpan span, string node, string expectedDiagnosticID)
            {
                Assert.AreEqual(node, unmarkedCode!.Substring(span.TextSpan));
                Assert.AreEqual(expectedDiagnosticID, span.ExpectedDiagnosticID);
            }
        }
    }
}