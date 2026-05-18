namespace RoseLynn.Testing;

public abstract class BaseCompilationTestContainer
{
    protected async Task VerifyCompilation(
        string source,
        CSharpCompilationTest.CompilationVerifier verifier,
        CancellationToken cancellationToken)
    {
        await VerifyCompilation([source], verifier, cancellationToken);
    }

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

    protected virtual CSharpCompilationTest CreateTest()
    {
        return new CSharpCompilationTest();
    }
}
