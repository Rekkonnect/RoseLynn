using System.Threading.Tasks;

namespace RoseLynn.Testing.Test;

public class DiagnosticMarkupCodeHandlerConversionTests
{
    [Test]
    public async Task MicrosoftToGuTest()
    {
        var microsoft = DiagnosticMarkupCodeHandler.MicrosoftCodeAnalysis;
        var gu = DiagnosticMarkupCodeHandler.GuRoslynAsserts;

        var original = GenerateMarkedCode(microsoft);
        var expectedConverted = GenerateMarkedCode(gu);
        var converted = microsoft.ConvertMarkup(original, gu);

        await Assert.That(converted).IsEqualTo(expectedConverted);

        static string GenerateMarkedCode(DiagnosticMarkupCodeHandler handler)
        {
            return
$@"
int a = 3;
{handler.MarkupDiagnostic("Int32")} {handler.MarkupDiagnostic("var", "CS0101")} = 3;
";
        }
    }
}
