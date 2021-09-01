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
        /// <inheritdoc/>
        protected sealed override UsingsProviderBase GetNewUsingsProviderInstance() => new TUsingsProvider();
    }

    /// <summary>Provides an analyzer diagnostic emission test fixture with built-in mechanisms to identify the tested diagnostic rule for a specific analyzer.</summary>
    /// <typeparam name="TAnalyzer">The analyzer whose diagnostic emissions to test.</typeparam>
    public abstract class BaseDiagnosticTests<TAnalyzer> : BaseDiagnosticTests
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        /// <inheritdoc/>
        protected sealed override DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance() => new TAnalyzer();
    }

    /// <summary>Provides an analyzer diagnostic emission test fixture with built-in mechanisms to identify the tested diagnostic rule.</summary>
    public abstract class BaseDiagnosticTests : BaseAnalyzerTestFixture
    {
        // TODO: Use DI
        private DiagnosticAnalyzer? analyzer;
        /// <summary>Gets the <seealso cref="DiagnosticAnalyzer"/> instance that performs the code analysis and emits the diagnostics that are to be tested.</summary>
        protected DiagnosticAnalyzer Analyzer => analyzer ??= (GetNewDiagnosticAnalyzerInstance());

        private UsingsProviderBase? usingsProvider;
        /// <summary>Gets the <seealso cref="UsingsProviderBase"/> instance that provides the default usings to append to code that undergoes testing.</summary>
        protected UsingsProviderBase UsingsProvider => usingsProvider ??= (GetNewUsingsProviderInstance() ?? UsingsProviderBase.Default);

        /// <summary>Initializes a new instance of <seealso cref="DiagnosticAnalyzer"/> for this specific test fixture.</summary>
        /// <returns>The new instance of the diagnostic analyzer for this test fixture. It must not be null.</returns>
        /// <remarks>Only override this method in deriving classes, and do not explicitly call it to avoid allocations. Prefer using the <seealso cref="Analyzer"/> property instead.</remarks>
        protected abstract DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance();
        /// <summary>Initializes a new instance of <seealso cref="UsingsProviderBase"/> for this specific test fixture.</summary>
        /// <returns>The new instance of the usings provider for this test fixture, or <see langword="null"/> to use <seealso cref="UsingsProviderBase.Default"/>.</returns>
        /// <remarks>Only override this method in deriving classes, and do not explicitly call it to avoid allocations. Prefer using the <seealso cref="UsingsProvider"/> property instead.</remarks>
        protected abstract UsingsProviderBase? GetNewUsingsProviderInstance();

        /// <summary>Validates that the given test code does not emit any diagnostics.</summary>
        /// <param name="testCode">The test code to validate.</param>
        /// <remarks>This function should call a related code validation function for the relevant code analysis testing framework.</remarks>
        protected abstract void ValidateCode(string testCode);
        /// <summary>Validates that the given test code, including the default provided usings, does not emit any diagnostics.</summary>
        /// <param name="testCode">The test code to validate.</param>
        /// <remarks>This function should call a related code validation function for the relevant code analysis testing framework.</remarks>
        protected void ValidateCodeWithUsings(string testCode)
        {
            ValidateCode(UsingsProvider.WithUsings(testCode));
        }

        /// <summary>Asserts that the given test code emits diagnostics.</summary>
        /// <param name="testCode">The test code to assert diagnostics for. The code should be marked up with diagnostic indicators using the appropriate markup style for the forwarded testing framework.</param>
        /// <remarks>This function should call a related diagnostics assertion function for the relevant code analysis testing framework.</remarks>
        protected abstract void AssertDiagnostics(string testCode);
        /// <summary>Asserts that the given test code, including the default provided usings, emits diagnostics.</summary>
        /// <param name="testCode">The test code to assert diagnostics for. The code should be marked up with diagnostic indicators using the appropriate markup style for the forwarded testing framework.</param>
        /// <remarks>This function should call a related diagnostics assertion function for the relevant code analysis testing framework.</remarks>
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
        /// <summary>Asserts diagnostics or validates the given test code, including the usings from <seealso cref="UsingsProvider"/>.</summary>
        /// <param name="testCode">The marked up test code to evaluate. Marked up diagnostics will be ignored if in validation mode.</param>
        /// <param name="assert">If <see langword="true"/>, diagnostics with the specific markup will be asserted, otherwise the code will be validated.</param>
        protected void AssertOrValidateWithUsings(string testCode, bool assert)
        {
            AssertOrValidate(UsingsProvider.WithUsings(testCode), assert);
        }
    }
}
