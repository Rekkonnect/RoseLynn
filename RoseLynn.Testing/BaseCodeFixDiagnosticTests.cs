using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.CodeFixes;
using System.Threading.Tasks;

namespace RoseLynn.Testing
{
    /// <summary>Provides an analyzer test fixture with built-in mechanisms to identify the tested diagnostic rule.</summary>
    /// <typeparam name="TAnalyzer">The type of the analyzer that emits the diagnostic that triggers the code fix.</typeparam>
    /// <typeparam name="TCodeFix">The type of the code fix that is triggered from the emitted diagnostic.</typeparam>
    public abstract class BaseCodeFixDiagnosticTests<TAnalyzer, TCodeFix> : BaseAnalyzerTestFixture
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : MultipleDiagnosticCodeFixProvider, new()
    {
        /// <summary>Verifies that the code fix works as intended, provided the initial source code with markup, and the expected result.</summary>
        /// <param name="markupCode">The initial source code with markup.</param>
        /// <param name="expected">The expected result source code after the code fix.</param>
        /// <param name="codeActionIndex">The index of the code action to perform.</param>
        public void TestCodeFix(string markupCode, string expected, int codeActionIndex = 0)
        {
            TestCodeFixAsync(markupCode, expected, codeActionIndex).Wait();
        }
        /// <summary>Verifies that the code fix works as intended, provided the initial source code with markup, and the expected result.</summary>
        /// <param name="markupCode">The initial source code with markup.</param>
        /// <param name="expected">The expected result source code after the code fix.</param>
        /// <param name="codeActionIndex">The index of the code action to perform.</param>
        /// <returns>The task that encapsulates performing the code fix.</returns>
        public Task TestCodeFixAsync(string markupCode, string expected, int codeActionIndex = 0)
        {
            ReplaceAsteriskMarkup(ref markupCode);
            return VerifyCodeFixAsync(markupCode, expected, codeActionIndex);
        }

        private void ReplaceAsteriskMarkup(ref string markupCode) => markupCode = ReplaceAsteriskMarkup(markupCode);
        private string ReplaceAsteriskMarkup(string markupCode) => markupCode.Replace("{|*", $"{{|{TestedDiagnosticRule.Id}");

        /// <summary>Verifies that the code fix works as intended, provided the initial source code with markup, and the expected result.</summary>
        /// <param name="markupCode">The initial source code with markup.</param>
        /// <param name="expected">The expected result source code after the code fix.</param>
        /// <param name="codeActionIndex">The index of the code action to perform.</param>
        /// <returns>The task that encapsulates performing the code fix.</returns>
        /// <remarks>When overriding this, prefer using the VerifyCodeFixAsync function in the CSharpCodeFixVerifier type provided with the template.</remarks>
        protected abstract Task VerifyCodeFixAsync(string markupCode, string expected, int codeActionIndex);
    }
}
