#nullable enable

using Microsoft.CodeAnalysis.Diagnostics;

namespace RoseLynn.Testing
{
    /// <summary>Provides an analyzer diagnostic emission test fixture with built-in mechanisms to identify the tested diagnostic rule for a specific analyzer.</summary>
    /// <typeparam name="TAnalyzer">The analyzer whose diagnostic emissions to test.</typeparam>
    /// <typeparam name="TUsingsProvider">The usings provider to use.</typeparam>
    public abstract class BaseDiagnosticTests<TAnalyzer, TUsingsProvider> : BaseDiagnosticTests<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TUsingsProvider : UsingsProviderBase, new()
    {
        protected sealed override UsingsProviderBase GetNewUsingsProviderInstance() => new TUsingsProvider();
    }

    /// <summary>Provides an analyzer diagnostic emission test fixture with built-in mechanisms to identify the tested diagnostic rule for a specific analyzer.</summary>
    /// <typeparam name="TAnalyzer">The analyzer whose diagnostic emissions to test.</typeparam>
    public abstract class BaseDiagnosticTests<TAnalyzer> : BaseDiagnosticTests
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        protected sealed override DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance() => new TAnalyzer();
    }

    /// <summary>Provides an analyzer diagnostic emission test fixture with built-in mechanisms to identify the tested diagnostic rule.</summary>
    public abstract class BaseDiagnosticTests : BaseAnalyzerTestFixture
    {
        // TODO: Use DI
        private UsingsProviderBase? usingsProvider;
        protected UsingsProviderBase UsingsProvider => usingsProvider ??= (GetNewUsingsProviderInstance() ?? UsingsProviderBase.Default);

        protected abstract DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance();
        /// <summary>Initializes a new instance of the <seealso cref="UsingsProviderBase"/> for this specific test fixture.</summary>
        /// <returns>The new instance of the usings provider for this test fixture, or <see langword="null"/> to use <seealso cref="UsingsProviderBase.Default"/>.</returns>
        /// <remarks>Only override this method in deriving classes, and do not explicitly call it to avoid allocations. Prefer using the <seealso cref="UsingsProvider"/> property instead.</remarks>
        protected abstract UsingsProviderBase? GetNewUsingsProviderInstance();

        protected abstract void ValidateCode(string testCode);
        protected void ValidateCodeWithUsings(string testCode)
        {
            ValidateCode(UsingsProvider.WithUsings(testCode));
        }

        protected abstract void AssertDiagnostics(string testCode);
        protected void AssertDiagnosticsWithUsings(string testCode)
        {
            AssertDiagnostics(UsingsProvider.WithUsings(testCode));
        }

        /// <summary>Asserts diagnostics or validates the given test code.</summary>
        /// <param name="testCode">The marked up test code to evaluate. Marked up diagnostics will be ignored if in validation mode.</param>
        /// <param name="assert">If <see langword="true"/>, diagnostics with the specific markup will be asserted, otherwise the code will be validated.</param>
        protected void AssertOrValidate(string testCode, bool assert)
        {
            if (assert)
                AssertDiagnostics(testCode);
            else
                ValidateCode(testCode.Replace("↓", ""));
        }
        protected void AssertOrValidateWithUsings(string testCode, bool assert)
        {
            AssertOrValidate(UsingsProvider.WithUsings(testCode), assert);
        }
    }
}
