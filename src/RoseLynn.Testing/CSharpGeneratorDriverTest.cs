using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn.Generators;
using System.Collections.Immutable;

namespace RoseLynn.Testing;

/// <summary>
/// Provides a <see cref="AnalyzerTest{TVerifier}"/> with improved functionality
/// around testing <see cref="GeneratorDriver"/>s and their incrementality.
/// </summary>
public abstract class CSharpGeneratorDriverTest : BaseCSharpCompilationTest
{
    /// <summary>Gets or sets the test state data controlling generator driver verification behavior.</summary>
    public GeneratorDriverTestStateData GeneratorDriverTestState { get; set; } = new();

    /// <inheritdoc/>
    protected override async Task RunImplAsync(CancellationToken cancellationToken)
    {
        var (compilation, driver) = await CreateCompilationDriver(cancellationToken);
        var executionStep = driver.RunExecutionStep(compilation, cancellationToken);
        var verifier = GeneratorDriverTestState.GeneratorDriverVerifier;
        if (verifier is not null)
        {
            await verifier(executionStep, cancellationToken);
        }
    }

    /// <summary>Creates a <seealso cref="CompilationDriver"/> pairing the compilation with the generator driver.</summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <seealso cref="CompilationDriver"/> ready for generator execution.</returns>
    protected async Task<CompilationDriver> CreateCompilationDriver(CancellationToken cancellationToken)
    {
        var compilation = await CreateCompilation(cancellationToken);
        if (compilation is null)
        {
            throw new InvalidOperationException("The compilation was null");
        }

        var driver = CreateDriver();
        return new CompilationDriver(compilation, driver);
    }

    /// <summary>Creates the <seealso cref="GeneratorDriver"/> to run against the test compilation.</summary>
    /// <returns>A configured <seealso cref="GeneratorDriver"/>.</returns>
    protected virtual GeneratorDriver CreateDriver()
    {
        var driverOptions = CreateDriverOptions();
        return CSharpGeneratorDriver.Create(
            generators: CreateGenerators().AsSourceGenerators(),
            driverOptions: driverOptions);
    }

    /// <summary>Creates the <seealso cref="GeneratorDriverOptions"/> for the driver.</summary>
    /// <returns>The driver options with incremental step tracking enabled.</returns>
    protected virtual GeneratorDriverOptions CreateDriverOptions()
    {
        return new(trackIncrementalGeneratorSteps: true);
    }

    /// <summary>Creates the incremental generators to include in the driver.</summary>
    /// <returns>The generators to run.</returns>
    protected abstract ImmutableArray<IIncrementalGenerator> CreateGenerators();

    /// <summary>Holds the configurable data driving the generator driver verification step.</summary>
    public sealed class GeneratorDriverTestStateData
    {
        /// <summary>Gets or sets the verifier delegate to invoke against the execution step result.</summary>
        public GeneratorDriverVerifier? GeneratorDriverVerifier { get; set; }
    }

    /// <summary>Represents a delegate that verifies the result of a generator driver execution step.</summary>
    /// <param name="executionStep">The execution step snapshot to verify.</param>
    /// <param name="cancellationToken">A token to cancel the verification.</param>
    public delegate ValueTask GeneratorDriverVerifier(
        GeneratorDriverCompilationExecutionStep executionStep,
        CancellationToken cancellationToken);
}
