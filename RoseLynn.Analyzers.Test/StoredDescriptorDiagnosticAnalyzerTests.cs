using NUnit.Framework;
using RoseLynn.Analyzers.Test.MockedResources;

namespace RoseLynn.Analyzers.Test;

public class StoredDescriptorDiagnosticAnalyzerTests
{
    [Test]
    public void SupportedDiagnosticsTest()
    {
        var analyzerInstance = new MockAnalyzer();
        Assert.AreEqual(MockStorage.Instance.GetDiagnosticDescriptors(typeof(MockAnalyzer)).Length, analyzerInstance.SupportedDiagnostics.Length);
    }
}
