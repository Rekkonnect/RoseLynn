namespace RoseLynn.Testing;

/// <summary>Provides a base container for running compilation verification tests.</summary>
public abstract class BaseCompilationTestContainer
{
    /// <summary>Verifies a single source file against the given compilation verifier.</summary>
    /// <param name="source">The source text to compile.</param>
    /// <param name="verifier">The verifier to run against the compilation result.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    protected async Task VerifyCompilation(
        string source,
        CSharpCompilationTest.CompilationVerifier verifier,
        CancellationToken cancellationToken)
    {
        await VerifyCompilation([source], verifier, cancellationToken);
    }

    /// <summary>Verifies multiple source files against the given compilation verifier.</summary>
    /// <param name="sources">The source texts to compile.</param>
    /// <param name="verifier">The verifier to run against the compilation result.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    protected async Task VerifyCompilation(
        IEnumerable<string> sources,
        CSharpCompilationTest.CompilationVerifier verifier,
        CancellationToken cancellationToken)
    {
        var test = CreateTest();
        test.TestState.Sources.AddRange(sources);
        test.CompilationTestState.CompilationVerifier = verifier;

        await test.RunAsync(cancellationToken);
    }

    /// <summary>Creates a new <seealso cref="CSharpCompilationTest"/> instance for running the compilation test.</summary>
    /// <returns>A new <seealso cref="CSharpCompilationTest"/> instance.</returns>
    protected virtual CSharpCompilationTest CreateTest()
    {
        return new CSharpCompilationTest();
    }
}
