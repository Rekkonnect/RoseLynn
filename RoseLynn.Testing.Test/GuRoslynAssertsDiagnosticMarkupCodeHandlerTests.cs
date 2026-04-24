using System.Threading.Tasks;

namespace RoseLynn.Testing.Test;

public class GuRoslynAssertsDiagnosticMarkupCodeHandlerTests : DiagnosticMarkupCodeHandlerTestsBase
{
    [Test]
    public override async Task RemoveMarkupTest()
    {
        var markedUp = DiagnosticMarkupCodeHandler.GuRoslynAsserts.RemoveMarkup("int ↓value = ↓5;");
        await Assert.That(markedUp).IsEqualTo("int value = 5;");
    }
}
