using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace RoseLynn.Testing;

/// <summary>Represents a source generator test container, providing helpful features for building tests for source generators.</summary>
/// <typeparam name="TIncrementalGenerator">The type of the source generator.</typeparam>
/// <typeparam name="TVerifier">The type of the verifier.</typeparam>
/// <typeparam name="TSourceGeneratorTest">The type of the source generator test fixture.</typeparam>
public abstract class BaseIncrementalGeneratorTestContainer<TIncrementalGenerator, TVerifier, TSourceGeneratorTest>
    : BaseGeneratorTestContainer<TIncrementalGenerator, TVerifier, TSourceGeneratorTest>

    where TIncrementalGenerator : IIncrementalGenerator, new()
    where TVerifier : IVerifier, new()
    where TSourceGeneratorTest : CSharpSourceGeneratorTest<InterfacingSourceGenerator, TVerifier>, new()
{
    /// <inheritdoc/>
    protected override void CreateDriverGenerator(
        out TIncrementalGenerator generator,
        out GeneratorDriver driver)
    {
        generator = new();
        driver = CSharpGeneratorDriver.Create(generator);
    }
}
