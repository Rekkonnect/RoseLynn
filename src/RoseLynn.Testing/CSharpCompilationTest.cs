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
    /// <summary>Gets or sets the test state data controlling compilation verification behavior.</summary>
    public CompilationTestStateData CompilationTestState { get; set; } = new();

    /// <inheritdoc/>
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

    /// <summary>Holds the configurable data driving the compilation verification step.</summary>
    public sealed class CompilationTestStateData
    {
        /// <summary>Gets or sets the verifier delegate to invoke against the compilation result.</summary>
        public CompilationVerifier? CompilationVerifier { get; set; }
    }

    /// <summary>Represents a delegate that verifies the state of a <seealso cref="Compilation"/> after it is created.</summary>
    /// <param name="compilation">The compilation to verify.</param>
    /// <param name="cancellationToken">A token to cancel the verification.</param>
    public delegate Task CompilationVerifier(
        Compilation compilation,
        CancellationToken cancellationToken);
}
