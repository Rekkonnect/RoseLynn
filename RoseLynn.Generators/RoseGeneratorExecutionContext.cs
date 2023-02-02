using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace RoseLynn.Generators;

/// <summary>
/// Implements the <seealso cref="IGeneratorExecutionContext{TContext}"/> for the
/// <seealso cref="SourceProductionContext"/> type.
/// </summary>
public record RoseSourceProductionContext(SourceProductionContext Context) : IGeneratorExecutionContext<SourceProductionContext>
{
    /// <inheritdoc/>
    public CancellationToken CancellationToken => Context.CancellationToken;

    /// <inheritdoc cref="SourceProductionContext.AddSource(string, string)"/>
    public void AddSource(string hintName, string source)
    {
        Context.AddSource(hintName, source);
    }

    /// <inheritdoc cref="SourceProductionContext.AddSource(string, SourceText)"/>
    public void AddSource(string hintName, SourceText sourceText)
    {
        Context.AddSource(hintName, sourceText);
    }

    /// <inheritdoc cref="SourceProductionContext.ReportDiagnostic(Diagnostic)"/>
    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        Context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Always returns <seealso cref="NETLanguage.Unknown"/>, as the
    /// <seealso cref="SourceProductionContext"/> does not provide information
    /// about the compilation.
    /// </summary>
    public NETLanguage CompilationLanguage => NETLanguage.Unknown;
}

