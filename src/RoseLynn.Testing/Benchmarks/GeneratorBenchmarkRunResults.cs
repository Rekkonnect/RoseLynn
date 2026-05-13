using System;
using System.Collections.Immutable;

namespace RoseLynn.Testing.Benchmarks;

[NeedsDocs]
public sealed record GeneratorBenchmarkRunResults(
    string CommitSha,
    DateTime RunTime,
    GeneratorBenchmarkParameters Parameters,
    ImmutableArray<GeneratorBenchmarkIterationResult> IterationResults);
