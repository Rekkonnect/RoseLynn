using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn.Generators;

namespace RoseLynn.Testing;

public abstract class BaseGeneratorIncrementalityTestContainer<TSourceGenerator, TTest>
    : CSharpSourceGeneratorVerifier<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TTest : BaseGeneratorIncrementalityTestContainer<TSourceGenerator>.IncrementalityTest, new()
{
    public async Task VerifyHasOnlyCachedGenerationSteps(
        string startSource,
        string nextSource,
        Func<bool, ValueTask> assertTrue,
        CancellationToken cancellationToken)
    {
        var test = CreateTest();
        test.TestState.Sources.Add(startSource);
        test.IncrementalityTestState.CompilationModifier = ModifyCompilation;
        test.GeneratorDriverTestState.GeneratorDriverVerifier = VerifyExecution;

        await test.RunAsync(cancellationToken);

        ValueTask<Compilation> ModifyCompilation(
            Compilation compilation, CancellationToken cancellationToken)
        {
            return new(ModifySingleSource(compilation, nextSource));
        }

        async ValueTask VerifyExecution(
            GeneratorDriverCompilationExecutionStep step,
            CancellationToken cancellationToken)
        {
            var hasOnlyCachedSteps = HasOnlyCachedGenerationSteps(step);
            await assertTrue(hasOnlyCachedSteps);
        }
    }

    public async Task VerifyHasOnlyCachedGenerationStepsSymmetrically(
        string source,
        string otherSource,
        Func<bool, ValueTask> assertTrue,
        CancellationToken cancellationToken)
    {
        await VerifyHasOnlyCachedGenerationSteps(source, otherSource, assertTrue, cancellationToken);
        await VerifyHasOnlyCachedGenerationSteps(otherSource, source, assertTrue, cancellationToken);
    }

    public async Task VerifyHasNonCachedGenerationSteps(
        string startSource,
        string nextSource,
        Func<bool, ValueTask> assertTrue,
        CancellationToken cancellationToken)
    {
        var test = CreateTest();
        test.TestState.Sources.Add(startSource);
        test.IncrementalityTestState.CompilationModifier = ModifyCompilation;
        test.GeneratorDriverTestState.GeneratorDriverVerifier = VerifyExecution;

        await test.RunAsync(cancellationToken);

        ValueTask<Compilation> ModifyCompilation(
            Compilation compilation, CancellationToken cancellationToken)
        {
            return new(ModifySingleSource(compilation, nextSource));
        }

        async ValueTask VerifyExecution(
            GeneratorDriverCompilationExecutionStep step,
            CancellationToken cancellationToken)
        {
            var hasNonCachedSteps = HasNonCachedGenerationSteps(step);
            await assertTrue(hasNonCachedSteps);
        }
    }

    public virtual TTest CreateTest()
    {
        return new();
    }

    protected static Compilation ModifySingleSource(
        Compilation sourceCompilation,
        string newSource)
    {
        return GeneratorTestHelpers.ModifySingleSource(sourceCompilation, newSource);
    }

    protected static bool HasOnlyCachedGenerationSteps(
        GeneratorDriverCompilationExecutionStep step)
    {
        var allOutputs = GetAllOutputs(step);
        return allOutputs is not []
            && allOutputs.All(output => output.Reason == IncrementalStepRunReason.Cached);
    }

    protected static bool HasNonCachedGenerationSteps(
        GeneratorDriverCompilationExecutionStep step)
    {
        var allOutputs = GetAllOutputs(step);
        return allOutputs is not []
            && allOutputs.Any(output => output.Reason != IncrementalStepRunReason.Cached);
    }

    private static List<(object Value, IncrementalStepRunReason Reason)> GetAllOutputs(
        GeneratorDriverCompilationExecutionStep step)
    {
        var driver = step.ResultingDriver;
        var results = driver.GetRunResult().Results;
        var allOutputs = results
            .SelectMany(s => s.TrackedOutputSteps)
            .SelectMany(outputStep => outputStep.Value)
            .SelectMany(output => output.Outputs)
            .ToList();
        return allOutputs;
    }
}
