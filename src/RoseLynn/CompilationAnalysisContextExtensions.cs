using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace RoseLynn;

/// <summary>Provides helper extensions for the <seealso cref="CompilationAnalysisContext"/> type.</summary>
public static class CompilationAnalysisContextExtensions
{
    /// <summary>
    /// Reports a range of diagnostics to the compilation the
    /// <seealso cref="CompilationAnalysisContext"/> reflects.
    /// </summary>
    /// <param name="context">The <seealso cref="CompilationAnalysisContext"/>.</param>
    /// <param name="diagnostics">The <seealso cref="Diagnostic"/>s to report.</param>
    public static void ReportDiagnostics(this CompilationAnalysisContext context, IEnumerable<Diagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
            context.ReportDiagnostic(diagnostic);
    }
}
