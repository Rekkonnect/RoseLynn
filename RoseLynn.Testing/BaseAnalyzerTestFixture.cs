using Microsoft.CodeAnalysis;
using RoseLynn.Analyzers;

namespace RoseLynn.Testing;

/// <summary>Provides an analyzer test fixture with built-in mechanisms to identify the tested diagnostic rule.</summary>
public abstract class BaseAnalyzerTestFixture
{
    private DiagnosticDescriptor testedDiagnosticRule;

    /// <summary>
    /// Gets the tested diagnostic rule. This is by default assumed to be equal to the prefix of the test type's name.<br/>
    /// For example, a test type named "CS1111_Tests" with a storage that stores rules with prefix "CS" would extract that the tested diagnostic rule is "CS1111".
    /// </summary>
    /// <remarks>This property is overridable so that the tested diagnostic rule can be explicitly specified.</remarks>
    public virtual DiagnosticDescriptor TestedDiagnosticRule
    {
        get
        {
            if (testedDiagnosticRule != null)
                return testedDiagnosticRule;

            var ruleID = GetType().Name.Substring(0, DiagnosticDescriptorStorage.DiagnosticIDLength);
            return testedDiagnosticRule = DiagnosticDescriptorStorage.GetDiagnosticDescriptor(ruleID);
        }
    }

    /// <summary>Gets the <seealso cref="DiagnosticDescriptorStorageBase"/> instance that stores the tested diagnostic rule for this type.</summary>
    protected abstract DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage { get; }
}
