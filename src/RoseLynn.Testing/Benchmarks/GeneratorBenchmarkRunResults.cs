using System.Collections.Immutable;

namespace RoseLynn.Testing.Benchmarks;

/// <summary>Represents the complete output of a generator benchmark run.</summary>
/// <param name="CommitSha">The source commit identifier associated with the benchmarked assembly.</param>
/// <param name="RunTime">The local time when the benchmark run was started.</param>
/// <param name="Parameters">The parameters used to execute the benchmark.</param>
/// <param name="IterationResults">The per-iteration measurements collected during the run.</param>
public sealed record GeneratorBenchmarkRunResults(
    string CommitSha,
    DateTime RunTime,
    GeneratorBenchmarkParameters Parameters,
    ImmutableArray<GeneratorBenchmarkIterationResult> IterationResults);
