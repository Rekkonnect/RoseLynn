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
    public GeneratorDriverTestStateData GeneratorDriverTestState { get; set; } = new();

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

    protected virtual GeneratorDriver CreateDriver()
    {
        var driverOptions = CreateDriverOptions();
        return CSharpGeneratorDriver.Create(
            generators: CreateGenerators().AsSourceGenerators(),
            driverOptions: driverOptions);
    }

    protected virtual GeneratorDriverOptions CreateDriverOptions()
    {
        return new(trackIncrementalGeneratorSteps: true);
    }

    protected abstract ImmutableArray<IIncrementalGenerator> CreateGenerators();

    public sealed class GeneratorDriverTestStateData
    {
        public GeneratorDriverVerifier? GeneratorDriverVerifier { get; set; }
    }

    public delegate ValueTask GeneratorDriverVerifier(
        GeneratorDriverCompilationExecutionStep executionStep,
        CancellationToken cancellationToken);
}
