using RoseLynn.Analyzers.Test.MockedResources;
using System.Threading.Tasks;

namespace RoseLynn.Analyzers.Test;

/// <summary>Tests analyzers that expose diagnostic descriptors through storage types.</summary>
public class StoredDescriptorDiagnosticAnalyzerTests
{
    [Test]
    public async Task SupportedDiagnosticsTest()
    {
        var analyzerInstance = new MockAnalyzer();
        await Assert.That(analyzerInstance.SupportedDiagnostics.Length).IsEqualTo(MockStorage.Instance.GetDiagnosticDescriptors(typeof(MockAnalyzer)).Length);
    }
}
