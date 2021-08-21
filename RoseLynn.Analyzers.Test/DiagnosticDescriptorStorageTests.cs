using NUnit.Framework;

namespace RoseLynn.Analyzers.Test
{
    using static MockedResources.MockStorage;

    public class DiagnosticDescriptorStorageTests
    {
        [Test]
        public void Test1()
        {
            var descriptor0001 = Instance.GetDiagnosticDescriptor(1);
            Assert.AreEqual(Instance.MOCK0001_Rule, descriptor0001);

            var descriptor0002 = Instance.GetDiagnosticDescriptor(2);
            Assert.AreEqual(Instance.MOCK0002_Rule, descriptor0002);

            var descriptor0003 = Instance.GetDiagnosticDescriptor(3);
            Assert.Null(descriptor0003);
        }
    }
}