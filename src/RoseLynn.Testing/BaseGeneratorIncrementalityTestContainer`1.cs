using Microsoft.CodeAnalysis;
using RoseLynn.CSharp;
using RoseLynn.Generators;

namespace RoseLynn.Testing;

/// <summary>Provides a base container for testing incrementality of a single <typeparamref name="TSourceGenerator"/>,
/// using the default <see cref="IncrementalityTest"/> inner test class.</summary>
/// <typeparam name="TSourceGenerator">The incremental source generator type under test.</typeparam>
public abstract class BaseGeneratorIncrementalityTestContainer<TSourceGenerator>
    : BaseGeneratorIncrementalityTestContainer<
        TSourceGenerator,
        BaseGeneratorIncrementalityTestContainer<TSourceGenerator>.IncrementalityTest>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    /// <summary>Provides the test harness for running incrementality verification of <typeparamref name="TSourceGenerator"/>.</summary>
    public class IncrementalityTest : CSharpSingleGeneratorDriverTest<TSourceGenerator>
    {
        /// <summary>Gets or sets the test state data controlling how the compilation is modified between runs.</summary>
        public IncrementalityTestStateData IncrementalityTestState { get; set; } = new();

        /// <inheritdoc/>
        protected override async Task RunImplAsync(CancellationToken cancellationToken)
        {
            var (compilation, driver) = await CreateCompilationDriver(cancellationToken);

            var executionStep = driver.RunExecutionStep(compilation, cancellationToken);
            var newDriver = executionStep.ResultingDriver;
            var cleanCompilation = executionStep.CleanGeneratedFiles();

            var newCompilation = await ModifyCompilation(cleanCompilation, cancellationToken);

            var verifiedExecutionStep = newDriver.RunExecutionStep(newCompilation, cancellationToken);
            var verifier = GeneratorDriverTestState.GeneratorDriverVerifier;
            if (verifier is not null)
            {
                await verifier(verifiedExecutionStep, cancellationToken);
            }
        }

        protected override CompilationOptions CreateCompilationOptions()
        {
            return base.CreateCompilationOptions()
                .WithNullableContextOptions(NullableContextOptions.Enable);
        }

        private async Task<Compilation> ModifyCompilation(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var modifier = IncrementalityTestState.CompilationModifier;
            if (modifier is null)
            {
                return compilation;
            }

            return await modifier(compilation, cancellationToken);
        }

        /// <summary>Holds the configurable data driving the compilation modification step between incrementality runs.</summary>
        public sealed class IncrementalityTestStateData
        {
            /// <summary>Gets or sets the delegate that modifies the compilation between the first and second generator runs.</summary>
            public CompilationModifier? CompilationModifier { get; set; }
        }

        /// <summary>Represents a delegate that modifies a <seealso cref="Compilation"/> between incremental generation runs.</summary>
        /// <param name="sourceCompilation">The compilation to modify.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The modified compilation.</returns>
        public delegate ValueTask<Compilation> CompilationModifier(
            Compilation sourceCompilation,
            CancellationToken cancellationToken);
    }
}
