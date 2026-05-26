using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Polyfills;
using RoseLynn.CSharp;
using RoseLynn.Generators;
using RoseLynn.Writing;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoseLynn.Testing;

/// <summary>Represents a source generator test container, providing helpful features for building tests for source generators.</summary>
/// <typeparam name="TSourceGenerator">The type of the source generator.</typeparam>
public abstract partial class BaseGeneratorTestContainer<TSourceGenerator, TTest>
    : BaseDrivenCSharpIncrementalGeneratorTestContainer<
        TSourceGenerator,
        TTest>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TTest : CSharpSourceGeneratorTestEx<TSourceGenerator>, new()
{
    public static SourceText Utf8CSharpSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string source)
    {
        return GeneralGeneratorTestHelpers.Utf8CSharpSource(source);
    }

    /// <summary>Provides a source-generator test implementation with nullable context enabled and failure-artifact support.</summary>
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

        private async Task HandleGeneratorAssertionFailure(
            CancellationToken cancellationToken)
        {
            await PublishGeneratorAssertionFailureArtifacts(cancellationToken);
        }

        /// <summary>
        /// Publishes debug artifacts that help inspect generator output after an assertion failure.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected virtual async Task PublishGeneratorAssertionFailureArtifacts(
            CancellationToken cancellationToken)
        {
            var testName = GetGeneratorAssertionFailureTestName();
            var artifacts = await CreateGeneratorAssertionFailureArtifacts(
                testName,
                cancellationToken);

            await OnGeneratorAssertionFailureArtifactsCreated(
                artifacts,
                cancellationToken);
        }

        /// <summary>
        /// Gets the display name used as the base file name for generator assertion failure artifacts.
        /// </summary>
        /// <returns>The test name used when generating artifact file names.</returns>
        protected virtual string GetGeneratorAssertionFailureTestName()
        {
            return $"{GetType().Name}.{typeof(TSourceGenerator).Name}";
        }

        /// <summary>
        /// Creates the generator assertion failure artifacts for the provided test name.
        /// </summary>
        /// <param name="testName">The test name used as the base file name for the generated artifacts.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The generated artifacts to publish for the failure.</returns>
        protected virtual async Task<IReadOnlyList<TestOutputArtifact>> CreateGeneratorAssertionFailureArtifacts(
            string testName,
            CancellationToken cancellationToken = default)
        {
            var result = await CreateCompilationRunGenerator(
                TestState.Sources.Select(s => s.content.ToString()),
                cancellationToken);

            return await WriteGeneratorAssertionFailureArtifacts(
                testName,
                result.ResultingDriver,
                cancellationToken);
        }

        /// <summary>
        /// Observes the generated artifacts created after a generator assertion failure.
        /// </summary>
        /// <param name="artifacts">The generated artifacts.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected virtual Task OnGeneratorAssertionFailureArtifactsCreated(
            IReadOnlyList<TestOutputArtifact> artifacts,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Writes generator assertion failure artifacts for the provided generator driver run.
        /// </summary>
        /// <param name="testName">The test name used as the base file name for the generated artifacts.</param>
        /// <param name="resultingGeneratorDriver">The generator driver containing the run result to serialize.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The artifacts written for the generator assertion failure.</returns>
        protected static async Task<IReadOnlyList<TestOutputArtifact>> WriteGeneratorAssertionFailureArtifacts(
            string testName,
            GeneratorDriver resultingGeneratorDriver,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var fileNameBase = $"{testName}_{now:yyyyMMdd_HHmmss}";
            const string directoryBase = "generator_outputs";

            Directory.CreateDirectory(directoryBase);

            var runResult = resultingGeneratorDriver.GetRunResult();

            var generatorOutputFile = Path.Combine(directoryBase, $"{fileNameBase}.cs");
            var outputMappings = GenerateMappingsFromRunResult(runResult);
            var code = GenerateSources(outputMappings);
            await File.WriteAllTextAsync(generatorOutputFile, code, cancellationToken);

            var generatorDriverInfoFile = Path.Combine(
                directoryBase,
                $"{fileNameBase}_driverinfo.txt");
            var aggregateDriverResult = AggregateGeneratorDriverResult
                .ResultsForDriver(resultingGeneratorDriver);
            var driverInfoText = GenerateDriverInfoText(aggregateDriverResult);
            await File.WriteAllTextAsync(
                generatorDriverInfoFile,
                driverInfoText, cancellationToken);

            return [
                new()
                {
                    FilePath = generatorOutputFile,
                    DisplayName = "Expected generator sources output",
                    Description = "A RoseLynn-friendly representation of the actual outputs to be copied as expected",
                },
                new()
                {
                    FilePath = generatorDriverInfoFile,
                    DisplayName = "Generator driver run result information",
                    Description = "Extra information aggregated from the driver's run result, helping with debug",
                },
            ];
        }

        private static SourceCollection GenerateMappingsFromRunResult(
            GeneratorDriverRunResult runResult)
        {
            var results = runResult.Results;
            var outputMappings = new SourceCollection();

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

        private static string GenerateSources(
            SourceCollection outputSources)
        {
            var builder = new StringBuilder();

            builder.AppendLine("var generatedSources = new SourceCollection");
            builder.AppendLine("{");

            foreach (var mapping in outputSources)
            {
                var hintName = EscapeVerbatimStringLiteral(mapping.Key);
                var source = EscapeRawStringLiteral(mapping.Value.ToString());

                builder.AppendLine($"    [@\"{hintName}\"] = Utf8CSharpSource(\"\"\"");
                builder.AppendLine(source);
                builder.AppendLine("    \"\"\"),");
            }

            builder.AppendLine("};");

            return builder.ToString();
        }

        private static string EscapeVerbatimStringLiteral(string value)
        {
            return value.Replace("\"", "\"\"");
        }

        private static string EscapeRawStringLiteral(string value)
        {
            return value.Replace("\"\"\"", "\"\"\"\"");
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

