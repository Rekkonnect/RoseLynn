using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;

namespace RoseLynn.Testing;

/// <summary>
/// Provides a <see cref="AnalyzerTest{TVerifier}"/> with improved functionality
/// around testing <see cref="GeneratorDriver"/>s and their incrementality, involving
/// a single incremental generator.
/// </summary>
public abstract class CSharpSingleGeneratorDriverTest<TSourceGenerator> : CSharpGeneratorDriverTest
    where TSourceGenerator : IIncrementalGenerator, new()
{
    /// <inheritdoc/>
    protected override ImmutableArray<IIncrementalGenerator> CreateGenerators()
    {
        return [new TSourceGenerator()];
    }
}
