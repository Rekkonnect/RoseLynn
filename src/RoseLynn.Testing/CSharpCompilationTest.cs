using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace RoseLynn.Testing;

/// <summary>
/// Configures a <see cref="AnalyzerTest{TVerifier}"/> to verify interactions
/// with a C# compilation, without forcing the usage of analyzers, code fixes
/// or source generators.
/// </summary>
public class CSharpCompilationTest : BaseCSharpCompilationTest
{
    public CompilationTestStateData CompilationTestState { get; set; } = new();

    protected override async Task RunImplAsync(CancellationToken cancellationToken)
    {
        var compilation = await CreateCompilation(cancellationToken);
        if (compilation is null)
        {
            throw new InvalidOperationException("The compilation was null");
        }

        var verifier = CompilationTestState.CompilationVerifier;
        if (verifier is not null)
        {
            await verifier(compilation, cancellationToken);
        }
    }

    public sealed class CompilationTestStateData
    {
        public CompilationVerifier? CompilationVerifier { get; set; }
    }

    public delegate Task CompilationVerifier(
        Compilation compilation,
        CancellationToken cancellationToken);
}
