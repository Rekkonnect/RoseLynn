# Analyzer Test Fixture

The `BaseAnalyzerTestFixture` provides a base implementation of a test fixture that tests a specific diagnostic rule, as returned by `TestedDiagnosticRule`.

By default, `TestedDiagnosticRule` will be the `DiagnosticDescriptor` returned by the [`DiagnosticDescriptorStorage`](../RoseLynn.Analyzers/descriptorStorage.md) property. The rule ID is detected from the name of the test fixture type.

For example, a test fixture for the CS0101 rule should be named `CS0101...`, which would be successfully detected. Otherwise, the property `TestedDiagnosticRule` should be overridden to return the actual `DiagnotsicDescriptor` that will be accounted.

Below are listed templates for using the abstracted test fixtures.

## Diagnostic Test Fixture Template

An example of defining a diagnostic test fixture using the Gu.Roslyn.Asserts framework:

```csharp
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoseLynn.Analyzers;
using RoseLynn.Testing;

public abstract class BaseExampleDiagnosticTests<TAnalyzer> : BaseDiagnosticTests
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected ExpectedDiagnostic ExpectedDiagnostic => ExpectedDiagnostic.Create(TestedDiagnosticRule);
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => ExampleDiagnosticDescriptorStorage.Instance;

    protected sealed override DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance() => new TAnalyzer();
    protected override UsingsProviderBase GetNewUsingsProviderInstance() => ExampleUsingsProvider.Instance;

    protected override void ValidateCode(string testCode)
    {
        RoslynAssert.Valid(GetNewDiagnosticAnalyzerInstance(), testCode);
    }
    protected override void AssertDiagnostics(string testCode)
    {
        RoslynAssert.Diagnostics(GetNewDiagnosticAnalyzerInstance(), ExpectedDiagnostic, testCode);
    }
}
```

## Code Fix Test Fixture Template

An example of defining a code fix test fixture using the Microsoft.CodeAnalysis.Testing framework:

```csharp
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoseLynn.Analyzers;
using RoseLynn.Testing;
using System.Threading.Tasks;

public abstract class BaseCodeFixTests<TAnalyzer, TCodeFix> : BaseCodeFixDiagnosticTests<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : ExampleCodeFixProvider, new()
{
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => ExampleDiagnosticDescriptorStorage.Instance;

    protected sealed override async Task VerifyCodeFixAsync(string markupCode, string expected, int codeActionIndex)
    {
        await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCodeFixAsync(markupCode, expected, codeActionIndex);
    }
}
```

Note: The `CSharpCodeFixVerifier<TAnalyzer, TCodeFix>` type is an automatically generated type from the code analyzer template in Visual Studio 2019.