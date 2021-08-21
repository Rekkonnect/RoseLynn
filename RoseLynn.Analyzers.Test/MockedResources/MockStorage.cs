using Microsoft.CodeAnalysis;
using System.Resources;

namespace RoseLynn.Analyzers.Test.MockedResources
{
    public sealed class MockStorage : DiagnosticDescriptorStorageBase
    {
        public static readonly MockStorage Instance = new();

        protected override string BaseRuleDocsURI => "https://github.com";
        protected override string DiagnosticIDPrefix => "MOCK";
        protected override ResourceManager ResourceManager => MockDiagnosticResources.ResourceManager;

        public const string MockCategory = "Mock";

        [DiagnosticSupported(typeof(MockAnalyzer))]
        public readonly DiagnosticDescriptor
            MOCK0001_Rule,
            MOCK0002_Rule;

        [DiagnosticSupported(typeof(MockAnalyzer))]
        public DiagnosticDescriptor MOCK1001_Rule { get; }

        private MockStorage()
        {
            MOCK0001_Rule = CreateDiagnosticDescriptor(0001, MockCategory, DiagnosticSeverity.Error);
            MOCK0002_Rule = CreateDiagnosticDescriptor(0002, MockCategory, DiagnosticSeverity.Hidden);
            MOCK1001_Rule = CreateDiagnosticDescriptor(1001, MockCategory, DiagnosticSeverity.Info);
        }
    }
}