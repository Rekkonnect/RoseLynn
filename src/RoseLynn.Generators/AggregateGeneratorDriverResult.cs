using Garyon.Functions;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.Generators;

[NeedsDocs]
public sealed record AggregateGeneratorDriverResult(
    GeneratorDriverRunResult RunResult,
    GeneratorDriverTimingInfo TimingInfo,
    IReadOnlyList<AggregateGeneratorResult> GeneratorResults)
{
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
