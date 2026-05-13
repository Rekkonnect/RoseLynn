using Microsoft.CodeAnalysis;
using RoseLynn.Generators;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.Testing.Benchmarks;

[NeedsDocs]
public abstract class BaseGeneratorBenchmarkTest<TSourceGenerator>
    : CSharpSourceGeneratorTestEx<TSourceGenerator>, IGeneratorBenchmarkTest
    where TSourceGenerator : IIncrementalGenerator, new()
{
    public async Task<GeneratorBenchmarkRunResults> RunBenchmark(
        GeneratorBenchmarkParameters parameters,
        CancellationToken cancellationToken)
    {
        var assembly = typeof(TSourceGenerator).Assembly;
        var commitSha = TestEffortContextHelpers.GetCommitSha(assembly) ?? "MISSING";
        var runTime = DateTime.Now;
        var results = await RunIterations(parameters, cancellationToken);
        return new(
            CommitSha: commitSha,
            RunTime: runTime,
            Parameters: parameters,
            IterationResults: results);
    }

    private async Task<ImmutableArray<GeneratorBenchmarkIterationResult>> RunIterations(
        GeneratorBenchmarkParameters parameters,
        CancellationToken cancellationToken)
    {
        CreateDriverGenerator(out var generator, out var driver);
        var compilationProvider = CreateCompilationProvider();
        var initialCompilation = await CreateCompilation([], cancellationToken);
        initialCompilation = await compilationProvider.InitializeCompilation(
            initialCompilation, cancellationToken);

        var iterations = ImmutableArray.CreateBuilder<GeneratorBenchmarkIterationResult>(
            parameters.MinIterations);

        var currentCompilation = initialCompilation;
        var currentDriver = driver;
        var totalDriverTime = TimeSpan.Zero;

        currentDriver.ColdStart(currentCompilation, 6, cancellationToken);

        // Collect all garbage to be more objective in the results
        GC.Collect();

        for (var iterationIndex = 0; true; iterationIndex++)
        {
            if (iterationIndex >= parameters.MinIterations
                && totalDriverTime >= parameters.MinDriverTime)
            {
                if (iterationIndex >= parameters.MaxIterations)
                {
                    break;
                }

                if (totalDriverTime >= parameters.MaxDriverTime)
                {
                    break;
                }
            }

            var nextDriver = currentDriver.RunGeneratorsAndUpdateCompilation(
                currentCompilation,
                out var nextCompilation,
                out var diagnostics,
                cancellationToken);

            var timingInfo = nextDriver.GetTimingInfo();
            var elapsedTime = timingInfo.GeneratorTimes
                .Where(s => s.Generator.GetGeneratorType() == typeof(TSourceGenerator))
                .Single()
                .ElapsedTime;
            totalDriverTime += elapsedTime;
            var totalMemory = CollectionMemorySnapshot.Collect(
                parameters.ForceGarbageCollection);

            iterations.Add(new(iterationIndex, elapsedTime, totalMemory));

            if (totalMemory.ForcedCollectionMemory >= parameters.MaxMemory)
            {
                break;
            }

            currentCompilation = nextCompilation;
            currentDriver = nextDriver;

            var cleanCompilation = currentCompilation.RemoveGeneratedTrees(nextDriver);

            nextCompilation = await compilationProvider.ModifyCompilation(
                iterationIndex,
                cleanCompilation,
                cancellationToken);

            currentCompilation = nextCompilation;
        }

        return iterations.ToImmutable();
    }

    protected abstract BaseGeneratorBenchmarkCompilationProvider CreateCompilationProvider();
}
