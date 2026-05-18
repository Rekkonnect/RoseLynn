using Microsoft.CodeAnalysis;
using RoseLynn.Generators;

namespace RoseLynn.Testing;

public abstract class BaseGeneratorIncrementalityTestContainer<TSourceGenerator>
    : BaseGeneratorIncrementalityTestContainer<
        TSourceGenerator,
        BaseGeneratorIncrementalityTestContainer<TSourceGenerator>.IncrementalityTest>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    public class IncrementalityTest : CSharpSingleGeneratorDriverTest<TSourceGenerator>
    {
        public IncrementalityTestStateData IncrementalityTestState { get; set; } = new();

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

        public sealed class IncrementalityTestStateData
        {
            public CompilationModifier? CompilationModifier { get; set; }
        }

        public delegate ValueTask<Compilation> CompilationModifier(
            Compilation sourceCompilation,
            CancellationToken cancellationToken);
    }
}
