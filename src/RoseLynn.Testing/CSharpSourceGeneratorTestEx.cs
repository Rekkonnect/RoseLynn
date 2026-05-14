using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace RoseLynn.Testing;

/// <summary>Provides a configurable base for incremental source-generator tests with predefined references.</summary>
/// <typeparam name="TSourceGenerator">The incremental generator under test.</typeparam>
public abstract class CSharpSourceGeneratorTestEx<TSourceGenerator>
    : CSharpSourceGeneratorTest<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    /// <summary>Gets the default reference assembly set for the test.</summary>
    public abstract ReferenceAssemblies DefaultReferenceAssemblies { get; }
    /// <summary>Gets the additional metadata references for the test.</summary>
    public abstract IEnumerable<MetadataReference> AdditionalReferences { get; }

    /// <summary>Initializes the test and applies its default references.</summary>
    public CSharpSourceGeneratorTestEx()
    {
        this.ApplyReferences(DefaultReferenceAssemblies, AdditionalReferences);
    }

    /// <summary>
    /// Creates a new instance of the <typeparamref name="TSourceGenerator"/>
    /// type and initializes a <seealso cref="GeneratorDriver"/> using that
    /// generator.
    /// </summary>
    /// <param name="generator">The initialized generator instance.</param>
    /// <param name="driver">The initialized <seealso cref="GeneratorDriver"/> instance.</param>
    protected void CreateDriverGenerator(
        out TSourceGenerator generator,
        out GeneratorDriver driver)
    {
        generator = new();
        var driverOptions = CreateDriverOptions();
        driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            driverOptions: driverOptions);
    }

    private static GeneratorDriverOptions CreateDriverOptions()
    {
        return new(trackIncrementalGeneratorSteps: true);
    }

    /// <summary>Creates a Roslyn compilation from source strings using the test's configured references.</summary>
    /// <param name="sources">The source texts to compile.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created compilation.</returns>
    protected async Task<Compilation> CreateCompilation(
        IEnumerable<string> sources,
        CancellationToken cancellationToken
    )
    {
        return await CSharpCompilation.Create(
            sources,
            TestState.AdditionalReferences,
            DefaultReferenceAssemblies,
            CreateParseOptions() as CSharpParseOptions,
            CreateCompilationOptions() as CSharpCompilationOptions,
            cancellationToken);
    }
}
