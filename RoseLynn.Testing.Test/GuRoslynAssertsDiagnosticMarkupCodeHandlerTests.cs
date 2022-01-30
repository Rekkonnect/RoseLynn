using NUnit.Framework;

namespace RoseLynn.Testing.Test;

public class GuRoslynAssertsDiagnosticMarkupCodeHandlerTests : DiagnosticMarkupCodeHandlerTestsBase
{
    [Test]
    public override void RemoveMarkupTest()
    {
        var markedUp = DiagnosticMarkupCodeHandler.GuRoslynAsserts.RemoveMarkup("int ↓value = ↓5;");
        Assert.AreEqual("int value = 5;", markedUp);
    }
}
