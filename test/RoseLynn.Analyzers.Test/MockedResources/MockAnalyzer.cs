using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace RoseLynn.Analyzers.Test.MockedResources;

#pragma warning disable RS1038 // Compiler extensions should be implemented in assemblies with compiler-provided references
#pragma warning disable RS1041 // Compiler extensions should be implemented in assemblies targeting netstandard2.0
#pragma warning disable RS1036 // Specify analyzer banned API enforcement setting
#pragma warning disable RS2008 // Enable analyzer release tracking

[DiagnosticAnalyzer(LanguageNames.CSharp)]
/// <summary>Provides a test analyzer with a small catalog of mock diagnostic descriptors.</summary>
public sealed class MockAnalyzer : CSharpDiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor MOCK0001_Rule = new(
        id: "MOCK0001",
        title: "Mock diagnostic",
        messageFormat: "Mock diagnostic",
        category: MockStorage.MockCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "This is a mock diagnostic not meant to be ever emitted on any sane application.",
        helpLinkUri: "https://github.com/MOCK0001.md");

    public static readonly DiagnosticDescriptor MOCK0002_Rule = new(
        id: "MOCK0002",
        title: "Mock diagnostic but better?",
        messageFormat: "Mock diagnostic but better?",
        category: MockStorage.MockCategory,
        defaultSeverity: DiagnosticSeverity.Hidden,
        isEnabledByDefault: true,
        description: "This is a much mocker diagnostic whose mockness no other mock diagnostic is ever going to reach.",
        helpLinkUri: "https://github.com/MOCK0002.md");

    public static readonly DiagnosticDescriptor MOCK1001_Rule = new(
        id: "MOCK1001",
        title: "Property mock diagnostic",
        messageFormat: "Property mock diagnostic",
        category: MockStorage.MockCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "This is a property mock diagnostic that must be tested too.",
        helpLinkUri: "https://github.com/MOCK1001.md");

    public static readonly DiagnosticDescriptor MOCK1002_Rule = new(
        id: "MOCK1002",
        title: "Mapped category mock diagnostic",
        messageFormat: "Mapped category mock diagnostic",
        category: MockStorage.MappedCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "This diagnostic exists to verify that severities are declared explicitly.",
        helpLinkUri: "https://github.com/MOCK1002.md");

    protected override ImmutableArray<DiagnosticDescriptor> DeclaredDiagnostics =>
    [
        MOCK0001_Rule,
        MOCK0002_Rule,
        MOCK1001_Rule,
        MOCK1002_Rule,
    ];

    /// <summary>Gets the storage used to resolve the analyzer's diagnostic descriptors.</summary>
    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => MockStorage.Instance;

    /// <summary>Initializes the analyzer context.</summary>
    /// <param name="context">The analysis context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
    }
}
