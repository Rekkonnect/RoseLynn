using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn.Analyzers;
using RoseLynn.CodeFixes;
using RoseLynn.Testing;

namespace RoseLynn.PoC.UnitTests;

public static class PoCTestInfrastructure
{
    public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net100;

    public static MetadataReference RenovateAttributeReference =>
        MetadataReference.CreateFromFile(typeof(RenovateAttribute).Assembly.Location);
}

public abstract class PoCDiagnosticTestsBase<TAnalyzer> : BaseDiagnosticTests<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public override DiagnosticDescriptor TestedDiagnosticRule =>
        PoCDiagnosticDescriptorStorage.Instance.POC0001_TwoParameterMethodRule;

    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage =>
        PoCDiagnosticDescriptorStorage.Instance;

    protected override UsingsProviderBase? GetNewUsingsProviderInstance() => null;

    protected override void ValidateCode(string testCode)
    {
        ValidateCodeAsync(testCode).GetAwaiter().GetResult();
    }

    protected override void AssertDiagnostics(string testCode)
    {
        AssertDiagnosticsAsync(testCode).GetAwaiter().GetResult();
    }

    private static Task ValidateCodeAsync(string testCode)
    {
        var test = CreateAnalyzerTest(testCode);
        return test.RunAsync();
    }

    private static Task AssertDiagnosticsAsync(string testCode)
    {
        var test = CreateAnalyzerTest(testCode);
        return test.RunAsync();
    }

    private static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateAnalyzerTest(string testCode)
    {
        return new()
        {
            TestCode = testCode,
            ReferenceAssemblies = PoCTestInfrastructure.ReferenceAssemblies,
        };
    }
}

public abstract class PoCCodeFixTestsBase<TAnalyzer, TCodeFix>
    : BaseCodeFixDiagnosticTests<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : MultipleDiagnosticCodeFixProvider, new()
{
    public override DiagnosticDescriptor TestedDiagnosticRule =>
        PoCDiagnosticDescriptorStorage.Instance.POC0001_TwoParameterMethodRule;

    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage =>
        PoCDiagnosticDescriptorStorage.Instance;

    protected override Task VerifyCodeFixAsync(string markupCode, string expected, int codeActionIndex)
    {
        var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
        {
            TestCode = markupCode,
            FixedCode = expected,
            CodeActionIndex = codeActionIndex,
            ReferenceAssemblies = PoCTestInfrastructure.ReferenceAssemblies,
        };

        return test.RunAsync();
    }
}

public abstract class PoCGeneratorTestContainer<TGenerator, TTest>
    : BaseGeneratorTestContainer<TGenerator, TTest>
    where TGenerator : IIncrementalGenerator, new()
    where TTest : PoCGeneratorTestContainer<TGenerator, TTest>.PoCGeneratorTest, new()
{
    public abstract class PoCGeneratorTest : SimpleGeneratorTest
    {
        public override ReferenceAssemblies DefaultReferenceAssemblies =>
            PoCTestInfrastructure.ReferenceAssemblies;

        public override IEnumerable<MetadataReference> AdditionalReferences =>
        [
            PoCTestInfrastructure.RenovateAttributeReference,
        ];
    }
}
