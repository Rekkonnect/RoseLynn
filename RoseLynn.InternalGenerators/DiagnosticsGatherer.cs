using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace RoseLynn.InternalGenerators;

public sealed class DiagnosticsGatherer
{
    private readonly List<Diagnostic> gatheredDiagnostics = new();

    public void Gather(Diagnostic diagnostic)
    {
        gatheredDiagnostics.Add(diagnostic);
    }
    public void Gather(DiagnosticDescriptor descriptor, SyntaxReference reference)
    {
        var diagnostic = DiagnosticFactoryEx.Create(descriptor, reference);
        Gather(diagnostic);
    }

    public void RegisterReportingOnCompilation(AnalysisContext context)
    {
        context.RegisterCompilationAction(ReportGatheredDiagnostics);
    }

    private void ReportGatheredDiagnostics(CompilationAnalysisContext context)
    {
        foreach (var diagnostic in gatheredDiagnostics)
            context.ReportDiagnostic(diagnostic);
    }
}
