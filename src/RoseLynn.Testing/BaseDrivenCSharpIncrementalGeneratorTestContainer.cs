using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn.Generators;

namespace RoseLynn.Testing;

/// <summary>Provides generator-verifier helpers backed by a configurable C# source-generator test type.</summary>
/// <typeparam name="TSourceGenerator">The incremental generator under test.</typeparam>
/// <typeparam name="TTest">The concrete verifier test type.</typeparam>
public abstract class BaseDrivenCSharpIncrementalGeneratorTestContainer<TSourceGenerator, TTest>
    : CSharpSourceGeneratorVerifier<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TTest : CSharpSourceGeneratorTest<TSourceGenerator, DefaultVerifier>, new()
{
    /// <summary>
    /// Verifies that the provided collection of sources causes the generator
    /// to produce sources matching the given mappings.
    /// </summary>
    /// <param name="sources">The original collection of sources that the source generator will act upon.</param>
    /// <param name="mappings">The expected generated source mappings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        IEnumerable<string> sources,
        GeneratedSourceMappings mappings,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync(
            sources,
            mappings,
            new TTest(),
            cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided collection of sources causes the generator to produce
    /// sources matching the given mappings, acting on a pre-initialized
    /// <typeparamref name="TTest"/> instance.
    /// </summary>
    /// <param name="sources">The original collection of sources that the source generator will act upon.</param>
    /// <param name="mappings">The expected generated source mappings.</param>
    /// <param name="test">
    /// The custom
    /// <typeparamref name="TTest"/> instance.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        IEnumerable<string> sources,
        GeneratedSourceMappings mappings,
        TTest test,
        CancellationToken cancellationToken = default)
    {
        test.TestState.Sources.AddRange(sources);
        foreach (var mapping in mappings)
        {
            test.TestState.GeneratedSources.Add((typeof(TSourceGenerator), mapping.Key, mapping.Value));
        }

        await test.RunAsync(cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided source causes the generator to produce sources matching the given mappings.
    /// </summary>
    /// <param name="source">The original source that the source generator will act upon.</param>
    /// <param name="mappings">The expected generated source mappings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        string source,
        GeneratedSourceMappings mappings,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync([source], mappings, cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided source causes the generator to produce sources matching the given mappings,
    /// acting on a pre-initialized <typeparamref name="TTest"/> instance.
    /// </summary>
    /// <param name="source">The original source that the source generator will act upon.</param>
    /// <param name="mappings">The expected generated source mappings.</param>
    /// <param name="test">The custom <typeparamref name="TTest"/> instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        string source,
        GeneratedSourceMappings mappings,
        TTest test,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync([source], mappings, test, cancellationToken);
    }
}

