using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;
using RoseLynn.Generators.Testing;

namespace RoseLynn.Testing;

/// <summary>
/// Provides additional abstractions over commonly-encountered boilerplate code from
/// <seealso cref="CSharpSourceGeneratorTest{TSourceGenerator, TVerifier}"/>
/// for incremental source generators.
/// </summary>
/// <typeparam name="TIncrementalGenerator">The type of the incremental source generator.</typeparam>
/// <typeparam name="TVerifier">The type of the verifier.</typeparam>
public abstract class CSharpIncrementalGeneratorTestEx<TIncrementalGenerator, TVerifier>
    : CSharpSourceGeneratorTestEx<InterfacingSourceGenerator, TVerifier>

    where TIncrementalGenerator : IIncrementalGenerator, new()
    where TVerifier : IVerifier, new()
{
    protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
    {
        return new[] { new TIncrementalGenerator().AsSourceGenerator() };
    }
}

