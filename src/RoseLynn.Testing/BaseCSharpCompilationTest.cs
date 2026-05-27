using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Model;

namespace RoseLynn.Testing;

/// <summary>Provides a Roslyn analyzer test base that creates C# compilations with nullable diagnostics enabled.</summary>
public abstract class BaseCSharpCompilationTest : AnalyzerTest<DefaultVerifier>
{
    /// <inheritdoc/>
    protected override string DefaultFileExt => "cs";

    /// <inheritdoc/>
    public override string Language => LanguageNames.CSharp;

    /// <inheritdoc/>
    protected override CompilationOptions CreateCompilationOptions()
        => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);

    /// <inheritdoc/>
    protected override ParseOptions CreateParseOptions()
        => new CSharpParseOptions(LanguageVersion.Default, DocumentationMode.Diagnose);

    /// <inheritdoc/>
    protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
    {
        return [];
    }

    /// <inheritdoc/>
    protected override abstract Task RunImplAsync(CancellationToken cancellationToken);

    /// <summary>Creates a <seealso cref="Compilation"/> from the test state's source files.</summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created <seealso cref="Compilation"/>, or <see langword="null"/> if the project could not be created.</returns>
    public async Task<Compilation?> CreateCompilation(
        CancellationToken cancellationToken)
    {
        var evaluatedState = new EvaluatedProjectState(TestState, ReferenceAssemblies);
        var project = await CreateProjectAsync(evaluatedState, [], cancellationToken);
        return await project.GetCompilationAsync(cancellationToken);
    }
}
