using Microsoft.CodeAnalysis;

namespace RoseLynn.Analyzers.Test.MockedResources;

public sealed class MockStorage : DiagnosticDescriptorStorageBase
{
    public static readonly MockStorage Instance = new();

    public const string MockCategory = "Mock";
    public const string MappedCategory = "Mapped";

    public readonly DiagnosticDescriptor
        MOCK0001_Rule,
        MOCK0002_Rule,
        MOCK1001_Rule,
        MOCK1002_Rule;

    private MockStorage()
        : base(typeof(MockAnalyzer))
    {
        MOCK0001_Rule = MockAnalyzer.MOCK0001_Rule;
        MOCK0002_Rule = MockAnalyzer.MOCK0002_Rule;
        MOCK1001_Rule = MockAnalyzer.MOCK1001_Rule;
        MOCK1002_Rule = MockAnalyzer.MOCK1002_Rule;
    }
}
