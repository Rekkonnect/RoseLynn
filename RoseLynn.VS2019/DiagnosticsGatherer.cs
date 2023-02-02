using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace RoseLynn;

/// <summary>
/// A system gathering diagnostics with the purpose of reporting them
/// after the compilation has finished.
/// </summary>
public sealed class DiagnosticsGatherer
{
    private readonly List<Diagnostic> gatheredDiagnostics = new();

    /// <summary>
    /// Gathers a <seealso cref="Diagnostic"/> to be reported after
    /// the compilation.
    /// </summary>
    /// <param name="diagnostic">The diagnotsic to report.</param>
    public void Gather(Diagnostic diagnostic)
    {
        gatheredDiagnostics.Add(diagnostic);
    }
    /// <summary>
    /// Gathers a <seealso cref="Diagnostic"/> to be reported after
    /// the compilation, constructed given a <seealso cref="DiagnosticDescriptor"/>
    /// and a <seealso cref="SyntaxReference"/>.
    /// </summary>
    /// <param name="descriptor">
    /// The <seealso cref="DiagnosticDescriptor"/> for creating the gathered
    /// <seealso cref="Diagnostic"/>.
    /// </param>
    /// <param name="reference">
    /// The <seealso cref="SyntaxReference"/> for creating the gathered
    /// <seealso cref="Diagnostic"/>.
    /// </param>
    public void Gather(DiagnosticDescriptor descriptor, SyntaxReference reference)
    {
        var diagnostic = DiagnosticFactory.Create(descriptor, reference);
        Gather(diagnostic);
    }

    /// <summary>
    /// Registers reporting of the gathered descriptors upon finishing
    /// compilation on a given <seealso cref="AnalysisContext"/>.
    /// </summary>
    /// <param name="context">
    /// The <seealso cref="AnalysisContext"/> on which to register reporting
    /// the gathered diagnostics.
    /// </param>
    public void RegisterReportingOnCompilation(AnalysisContext context)
    {
        context.RegisterCompilationAction(ReportGatheredDiagnostics);
    }

    private void ReportGatheredDiagnostics(CompilationAnalysisContext context)
    {
        context.ReportDiagnostics(gatheredDiagnostics);
    }
}
