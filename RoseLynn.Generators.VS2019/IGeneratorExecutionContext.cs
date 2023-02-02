using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace RoseLynn.Generators;

/// <summary>
/// Abstracts the idea of a <seealso cref="GeneratorExecutionContext"/>. Always prefer
/// using this in RoseLynn APIs, crafted with flexibility between both normal and
/// incremental source generators.
/// </summary>
public interface IGeneratorExecutionContext
{
    /// <inheritdoc cref="GeneratorExecutionContext.CancellationToken"/>
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc cref="GeneratorExecutionContext.AddSource(string, string)"/>
    public void AddSource(string hintName, string source);

    /// <inheritdoc cref="GeneratorExecutionContext.AddSource(string, SourceText)"/>
    public void AddSource(string hintName, SourceText sourceText);

    /// <inheritdoc cref="GeneratorExecutionContext.ReportDiagnostic(Diagnostic)"/>
    public void ReportDiagnostic(Diagnostic diagnostic);

    /// <summary>
    /// Gets the compilation language of the compilation that the generator
    /// acts upon. If the context does not provide that information,
    /// <seealso cref="NETLanguage.Unknown"/> is returned.
    /// </summary>
    public NETLanguage CompilationLanguage { get; }
}

/// <summary>
/// Abstracts the idea of a <seealso cref="GeneratorExecutionContext"/>. Always prefer
/// using this in RoseLynn APIs, crafted with flexibility between both normal and
/// incremental source generators.
/// </summary>
/// <typeparam name="TContext">The type of the generator execution context.</typeparam>
public interface IGeneratorExecutionContext<TContext> : IGeneratorExecutionContext
{
    /// <summary>
    /// Gets the wrapped context that this abstraction encapsulates.
    /// </summary>
    public TContext Context { get; }
}

/// <summary>
/// Provides handy extensions for the <see cref="IGeneratorExecutionContext"/> type.
/// </summary>
public static class IGeneratorExecutionContextExtensions
{
    /// <summary>
    /// Creates a <seealso cref="RoseGeneratorExecutionContext"/> wrapping the given
    /// <seealso cref="GeneratorExecutionContext"/>.
    /// </summary>
    /// <param name="context">
    /// The <seealso cref="GeneratorExecutionContext"/> to wrap into the resulting
    /// <seealso cref="RoseGeneratorExecutionContext"/>.
    /// </param>
    /// <returns>
    /// A <seealso cref="RoseGeneratorExecutionContext"/> wrapping the given
    /// <seealso cref="GeneratorExecutionContext"/>.
    /// </returns>
    public static RoseGeneratorExecutionContext RoseContext(this GeneratorExecutionContext context)
    {
        return new(context);
    }
}
