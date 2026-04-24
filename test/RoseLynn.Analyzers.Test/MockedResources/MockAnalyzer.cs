using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoseLynn.Analyzers.Test.MockedResources;

#pragma warning disable RS1038 // Compiler extensions should be implemented in assemblies with compiler-provided references
#pragma warning disable RS1041 // Compiler extensions should be implemented in assemblies targeting netstandard2.0
#pragma warning disable RS1036 // Specify analyzer banned API enforcement setting

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MockAnalyzer : CSharpDiagnosticAnalyzer
{
    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => MockStorage.Instance;

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
    }
}
