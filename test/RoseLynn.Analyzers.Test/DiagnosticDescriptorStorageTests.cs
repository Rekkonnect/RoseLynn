using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace RoseLynn.Analyzers.Test;

using static MockedResources.MockStorage;

/// <summary>Tests lookup behavior for stored diagnostic descriptors.</summary>
public class DiagnosticDescriptorStorageTests
{
    [Test]
    public async Task GetDiagnosticDescriptor()
    {
        await Assert.That(Instance[0001]).IsEqualTo(Instance.MOCK0001_Rule);
        await Assert.That(Instance[0002]).IsEqualTo(Instance.MOCK0002_Rule);
        await Assert.That(Instance[1001]).IsEqualTo(Instance.MOCK1001_Rule);
        await Assert.That(Instance["MOCK0001"]).IsEqualTo(Instance.MOCK0001_Rule);
        await Assert.That(Instance["MOCK1002"]).IsEqualTo(Instance.MOCK1002_Rule);

        await Assert.That(Instance[0003]).IsNull();
        await Assert.That(Instance[9999]).IsNull();
        await Assert.That(Instance[5555]).IsNull();
        await Assert.That(Instance[1111]).IsNull();
        await Assert.That(Instance["MOCK0003"]).IsNull();
        await Assert.That(Instance["OTHER0001"]).IsNull();
    }

    [Test]
    public async Task DiagnosticSeverityIsDeclaredExplicitly()
    {
        var mappedSeverity = Instance[1002];
        await Assert.That(mappedSeverity?.DefaultSeverity).IsEqualTo(DiagnosticSeverity.Warning);
    }

    [Test]
    public async Task GetInvalidDiagnosticID()
    {
        await Assert.That(Instance[-1]).IsNull();
        await Assert.That(Instance[15023]).IsNull();
    }
}
