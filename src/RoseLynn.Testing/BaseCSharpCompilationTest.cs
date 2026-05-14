using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.Testing;

/// <summary>Provides a Roslyn analyzer test base that creates C# compilations with nullable diagnostics enabled.</summary>
public abstract class BaseCSharpCompilationTest : AnalyzerTest<DefaultVerifier>
{
    protected override string DefaultFileExt => "cs";

    public override string Language => LanguageNames.CSharp;

    protected override CompilationOptions CreateCompilationOptions()
        => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);

    protected override ParseOptions CreateParseOptions()
        => new CSharpParseOptions(LanguageVersion.Default, DocumentationMode.Diagnose);

    protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
    {
        return [];
    }

    protected override abstract Task RunImplAsync(CancellationToken cancellationToken);

    public async Task<Compilation?> CreateCompilation(
        CancellationToken cancellationToken)
    {
        var evaluatedState = new EvaluatedProjectState(TestState, ReferenceAssemblies);
        var project = await CreateProjectAsync(evaluatedState, [], cancellationToken);
        return await project.GetCompilationAsync(cancellationToken);
    }
}
