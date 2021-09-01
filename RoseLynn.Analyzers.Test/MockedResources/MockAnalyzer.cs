using Microsoft.CodeAnalysis.Diagnostics;

namespace RoseLynn.Analyzers.Test.MockedResources
{
    public sealed class MockAnalyzer : CSharpDiagnosticAnalyzer
    {
        protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => MockStorage.Instance;

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        }
    }
}