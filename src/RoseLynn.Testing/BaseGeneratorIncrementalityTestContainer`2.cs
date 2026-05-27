using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn.Generators;

namespace RoseLynn.Testing;

/// <summary>Provides a base container for testing incrementality of a <typeparamref name="TSourceGenerator"/>
/// using a custom <typeparamref name="TTest"/> incrementality test class.</summary>
/// <typeparam name="TSourceGenerator">The incremental source generator type under test.</typeparam>
/// <typeparam name="TTest">The incrementality test class to use for running driver verification.</typeparam>
public abstract class BaseGeneratorIncrementalityTestContainer<TSourceGenerator, TTest>
    : CSharpSourceGeneratorVerifier<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TTest : BaseGeneratorIncrementalityTestContainer<TSourceGenerator>.IncrementalityTest, new()
{
    /// <summary>Verifies that transitioning from <paramref name="startSource"/> to <paramref name="nextSource"/>
    /// produces only cached generation steps.</summary>
    /// <param name="startSource">The initial source to compile.</param>
    /// <param name="nextSource">The modified source to transition to.</param>
    /// <param name="assertTrue">A delegate to assert the boolean result.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
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

    /// <summary>Verifies symmetrically that transitioning between <paramref name="source"/> and <paramref name="otherSource"/>
    /// in both directions produces only cached generation steps.</summary>
    /// <param name="source">The first source.</param>
    /// <param name="otherSource">The second source.</param>
    /// <param name="assertTrue">A delegate to assert the boolean result.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task VerifyHasOnlyCachedGenerationStepsSymmetrically(
        string source,
        string otherSource,
        Func<bool, ValueTask> assertTrue,
        CancellationToken cancellationToken)
    {
        await VerifyHasOnlyCachedGenerationSteps(source, otherSource, assertTrue, cancellationToken);
        await VerifyHasOnlyCachedGenerationSteps(otherSource, source, assertTrue, cancellationToken);
    }

    /// <summary>Verifies that transitioning from <paramref name="startSource"/> to <paramref name="nextSource"/>
    /// produces at least one non-cached generation step.</summary>
    /// <param name="startSource">The initial source to compile.</param>
    /// <param name="nextSource">The modified source to transition to.</param>
    /// <param name="assertTrue">A delegate to assert the boolean result.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
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

    /// <summary>Creates a new <typeparamref name="TTest"/> instance for running an incrementality test.</summary>
    /// <returns>A new <typeparamref name="TTest"/> instance.</returns>
    public virtual TTest CreateTest()
    {
        return new();
    }

    /// <summary>Returns a new <seealso cref="Compilation"/> with the single source document replaced by <paramref name="newSource"/>.</summary>
    /// <param name="sourceCompilation">The compilation whose source to replace.</param>
    /// <param name="newSource">The new source text to substitute.</param>
    /// <returns>A compilation with the replaced source.</returns>
    protected static Compilation ModifySingleSource(
        Compilation sourceCompilation,
        string newSource)
    {
        return GeneratorTestHelpers.ModifySingleSource(sourceCompilation, newSource);
    }

    /// <summary>Returns <see langword="true"/> if all tracked output steps in <paramref name="step"/> have <see cref="IncrementalStepRunReason.Cached"/> reason.</summary>
    /// <param name="step">The execution step to inspect.</param>
    /// <returns><see langword="true"/> if outputs are non-empty and all cached; otherwise <see langword="false"/>.</returns>
    protected static bool HasOnlyCachedGenerationSteps(
        GeneratorDriverCompilationExecutionStep step)
    {
        var allOutputs = GetAllOutputs(step);
        return allOutputs is not []
            && allOutputs.All(output => output.Reason == IncrementalStepRunReason.Cached);
    }

    /// <summary>Returns <see langword="true"/> if any tracked output step in <paramref name="step"/> has a reason other than <see cref="IncrementalStepRunReason.Cached"/>.</summary>
    /// <param name="step">The execution step to inspect.</param>
    /// <returns><see langword="true"/> if outputs are non-empty and any is non-cached; otherwise <see langword="false"/>.</returns>
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
