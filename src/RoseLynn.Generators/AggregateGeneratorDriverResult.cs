using Garyon.Functions;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.Generators;

/// <summary>Aggregates driver-level execution results with per-generator timing information.</summary>
/// <param name="RunResult">The raw run result returned by the generator driver.</param>
/// <param name="TimingInfo">The timing information captured for the driver execution.</param>
/// <param name="GeneratorResults">The per-generator aggregates matched across run results and timing data.</param>
public sealed record AggregateGeneratorDriverResult(
    GeneratorDriverRunResult RunResult,
    GeneratorDriverTimingInfo TimingInfo,
    IReadOnlyList<AggregateGeneratorResult> GeneratorResults)
{
    /// <summary>Builds an aggregate view of a generator driver's latest run results and timings.</summary>
    /// <param name="driver">The driver whose results will be aggregated.</param>
    /// <returns>An aggregate containing the driver run result, timing info, and matched per-generator data.</returns>
    public static AggregateGeneratorDriverResult ResultsForDriver(
        GeneratorDriver driver)
    {
        var runResult = driver.GetRunResult();
        var timingInfo = driver.GetTimingInfo();

        Asserts.True(runResult.Results.Length == timingInfo.GeneratorTimes.Length);

        var runResultDictionary = runResult.Results.ToDictionary(
            static r => GeneratorFullTypeName(r.Generator));

        var timingInfoDictionary = timingInfo.GeneratorTimes.ToDictionary(
            static t => GeneratorFullTypeName(t.Generator));

        var aggregates = new List<AggregateGeneratorResult>();
        foreach (var runResultKvp in runResultDictionary)
        {
            var generatorTypeName = runResultKvp.Key;
            _ = timingInfoDictionary.TryGetValue(generatorTypeName, out var timingInfoInner);
            Asserts.NotNull(timingInfoInner);
            var runResultGenerator = runResultKvp.Value.Generator;
            var timingInfoGenerator = timingInfoInner.Generator;
            Asserts.True(ReferenceEquals(runResultGenerator, timingInfoGenerator));
            var aggregate = new AggregateGeneratorResult(
                timingInfoGenerator,
                runResultKvp.Value,
                timingInfoInner);
            aggregates.Add(aggregate);
        }

        return new(runResult, timingInfo, aggregates);

        static string GeneratorFullTypeName(ISourceGenerator generator)
        {
            return generator.GetGeneratorType().FullName!;
        }
    }
}
