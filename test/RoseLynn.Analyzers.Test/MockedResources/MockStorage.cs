using Microsoft.CodeAnalysis;

namespace RoseLynn.Analyzers.Test.MockedResources;

/// <summary>Provides a singleton diagnostic-descriptor storage used by analyzer tests.</summary>
public sealed class MockStorage : DiagnosticDescriptorStorageBase
{
    /// <summary>Gets the singleton storage instance.</summary>
    public static readonly MockStorage Instance = new();

    /// <summary>Gets the base diagnostic category used by the mock descriptors.</summary>
    public const string MockCategory = "Mock";
    /// <summary>Gets the alternate diagnostic category used by the mock descriptors.</summary>
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
