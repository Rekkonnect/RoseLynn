using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Provides helper extensions for the <seealso cref="GeneratorExecutionContext"/> type.</summary>
public static class GeneratorExecutionContextExtensions
{
    /// <summary>Gets the <seealso cref="NETLanguage"/> value representing the source language of the compilation the generator is executed on.</summary>
    /// <param name="context">The <seealso cref="GeneratorExecutionContext"/> whose source language to parse into a <seealso cref="NETLanguage"/> value.</param>
    /// <returns>The <seealso cref="NETLanguage"/> of the compilation that the generator is executed on.</returns>
    public static NETLanguage GetCompilationLanguage(this GeneratorExecutionContext context)
    {
        return context.Compilation.GetNETLanguage();
    }

    /// <summary>
    /// Reports a diagnostic on the <seealso cref="GeneratorExecutionContext"/>
    /// that is created from a <seealso cref="DiagnosticDescriptor"/> and a
    /// <seealso cref="SyntaxReference"/>.
    /// </summary>
    /// <param name="context">
    /// The <seealso cref="GeneratorExecutionContext"/> on which to report the created diagnostic.
    /// </param>
    /// <param name="descriptor">
    /// The <seealso cref="DiagnosticDescriptor"/> for creating the reported
    /// <seealso cref="Diagnostic"/>.
    /// </param>
    /// <param name="reference">
    /// The <seealso cref="SyntaxReference"/> for creating the reported
    /// <seealso cref="Diagnostic"/>.
    /// </param>
    public static void ReportDiagnostic(this GeneratorExecutionContext context, DiagnosticDescriptor descriptor, SyntaxReference reference)
    {
        context.ReportDiagnostic(DiagnosticFactory.Create(descriptor, reference));
    }
}
