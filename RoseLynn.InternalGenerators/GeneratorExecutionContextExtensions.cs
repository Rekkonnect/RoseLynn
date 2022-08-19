using Microsoft.CodeAnalysis;

namespace RoseLynn.InternalGenerators;

public static class GeneratorExecutionContextExtensions
{
    public static void ReportDiagnostic(this GeneratorExecutionContext context, DiagnosticDescriptor descriptor, SyntaxReference reference)
    {
        context.ReportDiagnostic(DiagnosticFactoryEx.Create(descriptor, reference));
    }
}
