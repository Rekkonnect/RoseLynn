using RoseLynn.Analyzers.Test.MockedResources;
using System.Threading.Tasks;

namespace RoseLynn.Analyzers.Test;

public class StoredDescriptorDiagnosticAnalyzerTests
{
    [Test]
    public async Task SupportedDiagnosticsTest()
    {
        var analyzerInstance = new MockAnalyzer();
        await Assert.That(analyzerInstance.SupportedDiagnostics.Length).IsEqualTo(MockStorage.Instance.GetDiagnosticDescriptors(typeof(MockAnalyzer)).Length);
    }
}
