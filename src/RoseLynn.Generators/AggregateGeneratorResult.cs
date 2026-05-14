using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Represents the combined run result and timing information for a single source generator.</summary>
/// <param name="Generator">The generator instance that produced the result.</param>
/// <param name="RunResult">The run result captured for the generator.</param>
/// <param name="TimingInfo">The timing information captured for the generator.</param>
public sealed record AggregateGeneratorResult(
    ISourceGenerator Generator,
    GeneratorRunResult RunResult,
    GeneratorTimingInfo TimingInfo);
