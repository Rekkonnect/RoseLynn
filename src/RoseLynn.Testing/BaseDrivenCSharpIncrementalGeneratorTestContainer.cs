using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn.Writing;

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
    /// <param name="generatedSources">The expected generated sources.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        IEnumerable<string> sources,
        SourceCollection generatedSources,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync(
            sources,
            generatedSources,
            new TTest(),
            cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided collection of sources causes the generator to produce
    /// sources matching the given mappings, acting on a pre-initialized
    /// <typeparamref name="TTest"/> instance.
    /// </summary>
    /// <param name="sources">The original collection of sources that the source generator will act upon.</param>
    /// <param name="generatedSources">The expected generated sources.</param>
    /// <param name="test">
    /// The custom
    /// <typeparamref name="TTest"/> instance.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        IEnumerable<string> sources,
        SourceCollection generatedSources,
        TTest test,
        CancellationToken cancellationToken = default)
    {
        test.TestState.Sources.AddRange(sources);
        foreach (var generatedSource in generatedSources)
        {
            test.TestState.GeneratedSources.Add((typeof(TSourceGenerator), generatedSource.Key, generatedSource.Value));
        }

        await test.RunAsync(cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided source causes the generator to produce sources matching the given mappings.
    /// </summary>
    /// <param name="source">The original source that the source generator will act upon.</param>
    /// <param name="sources">The expected generated sources.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        string source,
        SourceCollection sources,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync([source], sources, cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided source causes the generator to produce sources matching the given mappings,
    /// acting on a pre-initialized <typeparamref name="TTest"/> instance.
    /// </summary>
    /// <param name="source">The original source that the source generator will act upon.</param>
    /// <param name="sources">The expected generated sources.</param>
    /// <param name="test">The custom <typeparamref name="TTest"/> instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        string source,
        SourceCollection sources,
        TTest test,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync([source], sources, test, cancellationToken);
    }
}

