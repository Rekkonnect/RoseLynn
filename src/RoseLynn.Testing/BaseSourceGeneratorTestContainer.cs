using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace RoseLynn.Testing;

/// <summary>Represents a source generator test container, providing helpful features for building tests for source generators.</summary>
/// <typeparam name="TSourceGenerator">The type of the source generator.</typeparam>
/// <typeparam name="TVerifier">The type of the verifier.</typeparam>
/// <typeparam name="TSourceGeneratorTest">The type of the source generator test fixture.</typeparam>
public abstract class BaseSourceGeneratorTestContainer<TSourceGenerator, TVerifier, TSourceGeneratorTest>
    : BaseGeneratorTestContainer<TSourceGenerator, TVerifier, TSourceGeneratorTest>

    where TSourceGenerator : ISourceGenerator, new()
    where TVerifier : IVerifier, new()
    where TSourceGeneratorTest : CSharpSourceGeneratorTest<TSourceGenerator, TVerifier>, new()
{
    /// <inheritdoc/>
    protected override void CreateDriverGenerator(
        out TSourceGenerator generator,
        out GeneratorDriver driver)
    {
        generator = new();
        driver = CSharpGeneratorDriver.Create(generator);
    }
}
