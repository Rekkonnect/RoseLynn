using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace RoseLynn.Analyzers.Test
{
    using static MockedResources.MockStorage;

    public class DiagnosticDescriptorStorageTests
    {
        [Test]
        public void GetDiagnosticDescriptor()
        {
            Assert.AreEqual(Instance.MOCK0001_Rule, Instance[0001]);
            Assert.AreEqual(Instance.MOCK0002_Rule, Instance[0002]);
            Assert.AreEqual(Instance.MOCK1001_Rule, Instance[1001]);

            Assert.Null(Instance[0003]);
            Assert.Null(Instance[9999]);
            Assert.Null(Instance[5555]);
            Assert.Null(Instance[1111]);
        }

        [Test]
        public void DefaultDiagnosticSeverity()
        {
            var mappedSeverity = Instance[1002];
            Assert.AreEqual(DiagnosticSeverity.Warning, mappedSeverity.DefaultSeverity);
        }

        [Test]
        public void GetInvalidDiagnosticID()
        {
            Assert.Null(Instance[-1]);
            Assert.Null(Instance[15023]);
        }
    }
}