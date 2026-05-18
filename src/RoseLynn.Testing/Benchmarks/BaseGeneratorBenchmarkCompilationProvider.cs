using Microsoft.CodeAnalysis;

namespace RoseLynn.Testing.Benchmarks;

/// <summary>Provides the compilation lifecycle hooks used by generator benchmark tests.</summary>
public abstract class BaseGeneratorBenchmarkCompilationProvider
{
    /// <summary>Creates the starting compilation used for the benchmark run.</summary>
    /// <param name="compilation">The baseline compilation produced by the test infrastructure.</param>
    /// <param name="cancellationToken">The token used to cancel the initialization.</param>
    /// <returns>A task that produces the initialized compilation.</returns>
    public abstract ValueTask<Compilation> InitializeCompilation(
        Compilation compilation,
        CancellationToken cancellationToken);

    /// <summary>Applies per-iteration compilation changes before the next benchmark run.</summary>
    /// <param name="iterationIndex">The zero-based iteration index that just completed.</param>
    /// <param name="compilation">The clean compilation state to modify for the next iteration.</param>
    /// <param name="cancellationToken">The token used to cancel the modification.</param>
    /// <returns>A task that produces the compilation for the next iteration.</returns>
    public abstract ValueTask<Compilation> ModifyCompilation(
        int iterationIndex,
        Compilation compilation,
        CancellationToken cancellationToken);
}
