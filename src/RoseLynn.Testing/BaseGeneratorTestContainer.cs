using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Polyfills;
using RoseLynn.Generators;
using RoseLynn.Writing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.Testing;

/// <summary>Represents a source generator test container, providing helpful features for building tests for source generators.</summary>
/// <typeparam name="TSourceGenerator">The type of the source generator.</typeparam>
public abstract partial class BaseGeneratorTestContainer<TSourceGenerator, TTest>
    : BaseDrivenCSharpIncrementalGeneratorTestContainer<
        TSourceGenerator,
        TTest>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TTest : BaseGeneratorTestContainer<TSourceGenerator, TTest>.SimpleGeneratorTest, new()
{
    public static SourceText Utf8CSharpSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string source)
    {
        return GeneralGeneratorTestHelpers.Utf8CSharpSource(source);
    }

    public abstract class SimpleGeneratorTest
        : CSharpSourceGeneratorTestEx<TSourceGenerator>
    {
        protected override CompilationOptions CreateCompilationOptions()
        {
            return base.CreateCompilationOptions()
                .WithNullableContextOptions(NullableContextOptions.Enable);
        }

        protected override async Task RunImplAsync(CancellationToken cancellationToken)
        {
            try
            {
                await base.RunImplAsync(cancellationToken);
            }
            catch
            {
                await HandleGeneratorAssertionFailure(cancellationToken);
                throw;
            }
        }

        // TODO: Abstract the test context provider/adapter so that we can be agnostic to
        // TUnit and instead change this into a workflow-based model that 
        private async Task HandleGeneratorAssertionFailure(
            CancellationToken cancellationToken)
        {
            var context = TestContext.Current;
            if (context is null)
            {
                throw new InvalidCastException(
                    "Cannot retrieve the current test context");
            }

            var testName = $"{context.ClassContext.ClassType.Name}.{context.Metadata.TestName}";
            var artifacts = await RunTestWriteHelpArtifacts(
                testName, cancellationToken);

            foreach (var artifact in artifacts)
            {
                context.Output.AttachArtifact(artifact);
            }
        }

        private async Task<IReadOnlyList<Artifact>> RunTestWriteHelpArtifacts(
            string testName,
            CancellationToken cancellationToken = default)
        {
            var result = await CreateCompilationRunGenerator(
                TestState.Sources.Select(s => s.content.ToString()),
                cancellationToken);

            return await WriteTestGeneratorOutput(
                testName,
                result.ResultingDriver,
                cancellationToken);
        }

        private static async Task<IReadOnlyList<Artifact>> WriteTestGeneratorOutput(
            string testName,
            GeneratorDriver resultingGeneratorDriver,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var fileNameBase = $"{testName}_{now:yyyyMMdd_HHmmss}";
            const string directoryBase = "generator_outputs";

            var fileSystem = new FileSystem();
            var targetDirectory = fileSystem.DirectoryInfo.New(directoryBase);
            targetDirectory.CreateSafe();

            var runResult = resultingGeneratorDriver.GetRunResult();

            var generatorOutputFile = targetDirectory.File($"{fileNameBase}.cs");
            var outputMappings = GenerateMappingsFromRunResult(runResult);
            var code = GeneratorOutputWriter.GenerateSourceMappingsCode(outputMappings);
            await generatorOutputFile.WriteAllTextAsync(code, cancellationToken);

            var generatorDriverInfoFile = targetDirectory
                .File($"{fileNameBase}_driverinfo.txt");
            var aggregateDriverResult = AggregateGeneratorDriverResult
                .ResultsForDriver(resultingGeneratorDriver);
            var driverInfoText = GenerateDriverInfoText(aggregateDriverResult);
            await generatorDriverInfoFile.WriteAllTextAsync(
                driverInfoText, cancellationToken);

            return [
                new()
                {
                    File = generatorOutputFile.Concrete,
                    DisplayName = "Expected generator sources output",
                    Description = "A RoseLynn-friendly representation of the actual outputs to be copied as expected",
                },
                new()
                {
                    File = generatorDriverInfoFile.Concrete,
                    DisplayName = "Generator driver run result information",
                    Description = "Extra information aggregated from the driver's run result, helping with debug",
                },
            ];
        }

        private static GeneratedSourceMappings GenerateMappingsFromRunResult(
            GeneratorDriverRunResult runResult)
        {
            var results = runResult.Results;
            var outputMappings = new GeneratedSourceMappings();

            foreach (var result in results)
            {
                foreach (var source in result.GeneratedSources)
                {
                    var fileName = source.HintName;
                    var text = source.SourceText;

                    outputMappings.Add(fileName, text);
                }
            }

            return outputMappings;
        }

        private static string GenerateDriverInfoText(
            AggregateGeneratorDriverResult driverResult)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"""
            Total results: {driverResult.GeneratorResults.Count}
            Total run time: {driverResult.TimingInfo.ElapsedTime}

            ====

            """);

            var blockList = new StringBuilderSeparatableBlockList(builder);
            foreach (var sub in driverResult.GeneratorResults)
            {
                using var blockScope = blockList.BeginBlockScope();

                var generatorTypeName = sub.Generator.GetGeneratorType().FullName;

                builder.AppendLine($"""
                Generator type: {generatorTypeName}
                Elapsed time: {sub.TimingInfo.ElapsedTime}
                """);

                if (sub.RunResult.Diagnostics is { Length: > 0 }
                    and var diagnostics)
                {
                    builder.AppendLine();
                    builder.AppendLine("Diagnostics:");

                    foreach (var diagnostic in diagnostics)
                    {
                        builder.AppendLine($"""
                          {diagnostic.Severity} - {diagnostic.Id}
                          Message: {diagnostic.GetMessage()}
                        """);
                    }

                    builder.AppendLine();
                }

                if (sub.RunResult.TrackedSteps is { Count: > 0 }
                    and var trackedSteps)
                {
                    builder.AppendLine();
                    builder.AppendLine("Tracked steps:");

                    foreach (var trackedStepGroup in trackedSteps)
                    {
                        builder.AppendLine($"""
                        - Step key: {trackedStepGroup.Key}
                          Steps:
                        """);

                        foreach (var step in trackedStepGroup.Value)
                        {
                            builder.AppendLine($"""
                              - Elapsed time: {step.ElapsedTime}
                                Input count: {step.Inputs.Length}
                                Output count: {step.Outputs.Length}
                            """);
                        }
                    }

                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates a <seealso cref="Compilation"/> and for the provided collection of sources and initializes
        /// instances around running the generator on the provided sources.
        /// </summary>
        /// <param name="sources">The collection of sources the generator will act upon.</param>
        /// <param name="generator">The generator instance that was created.</param>
        /// <param name="resultingGeneratorDriver">The resulting generator driver.</param>
        /// <param name="initialCompilation">The initial compilation before the generator runs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="GeneratorDriverCompilationExecutionStep"/> after the generator is run.</returns>
        private async Task<GeneratorDriverCompilationExecutionStep> CreateCompilationRunGenerator(
            IEnumerable<string> sources,
            CancellationToken cancellationToken = default)
        {
            CreateDriverGenerator(out _, out var driver);

            return await CreateCompilationRunGenerator(
                sources,
                driver,
                cancellationToken);
        }

        private async Task<GeneratorDriverCompilationExecutionStep> CreateCompilationRunGenerator(
            IEnumerable<string> sources,
            GeneratorDriver driver,
            CancellationToken cancellationToken = default)
        {
            var initialCompilation = await CreateCompilation(sources, cancellationToken);
            return driver.RunExecutionStep(initialCompilation, cancellationToken);
        }
    }
}

