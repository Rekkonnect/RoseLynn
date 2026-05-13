using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

[NeedsDocs]
public sealed record AggregateGeneratorResult(
    ISourceGenerator Generator,
    GeneratorRunResult RunResult,
    GeneratorTimingInfo TimingInfo);
