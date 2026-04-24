using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace RoseLynn.Generators;

/// <summary>
/// Implements the <seealso cref="IGeneratorExecutionContext{TContext}"/> for the
/// <seealso cref="GeneratorExecutionContext"/> type.
/// </summary>
public record RoseGeneratorExecutionContext(GeneratorExecutionContext Context)
    : IGeneratorExecutionContext<GeneratorExecutionContext>
{
    /// <inheritdoc/>
    public CancellationToken CancellationToken => Context.CancellationToken;

    /// <inheritdoc/>
    public void AddSource(string hintName, string source)
    {
        Context.AddSource(hintName, source);
    }

    /// <inheritdoc/>
    public void AddSource(string hintName, SourceText sourceText)
    {
        Context.AddSource(hintName, sourceText);
    }

    /// <inheritdoc/>
    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        Context.ReportDiagnostic(diagnostic);
    }

    /// <inheritdoc/>
    public NETLanguage CompilationLanguage => Context.GetCompilationLanguage();
}

