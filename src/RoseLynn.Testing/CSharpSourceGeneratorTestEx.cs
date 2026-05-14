using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace RoseLynn.Testing;

[NeedsDocs]
public abstract class CSharpSourceGeneratorTestEx<TSourceGenerator>
    : CSharpSourceGeneratorTest<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    public abstract ReferenceAssemblies DefaultReferenceAssemblies { get; }
    public abstract IEnumerable<MetadataReference> AdditionalReferences { get; }

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
