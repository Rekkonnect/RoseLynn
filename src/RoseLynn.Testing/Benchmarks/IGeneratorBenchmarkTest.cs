namespace RoseLynn.Testing.Benchmarks;

/// <summary>Defines the contract for running a source generator benchmark.</summary>
public interface IGeneratorBenchmarkTest
{
    /// <summary>Runs the benchmark using the provided parameters.</summary>
    /// <param name="parameters">The limits and options that control the benchmark run.</param>
    /// <param name="cancellationToken">The token used to cancel the benchmark.</param>
    /// <returns>A task that produces the benchmark run results.</returns>
    public Task<GeneratorBenchmarkRunResults> RunBenchmark(
        GeneratorBenchmarkParameters parameters,
        CancellationToken cancellationToken);
}
