using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.Analyzers;

namespace RoseLynn.Testing;

/// <summary>Provides a base test fixture for <seealso cref="DiagnosticSuppressor"/> implementations.</summary>
/// <typeparam name="TSuppressor">The suppressor type under test.</typeparam>
public abstract class BaseSuppressorTests<TSuppressor>
    where TSuppressor : StoredDescriptorDiagnosticSuppressor, new()
{
    private TSuppressor? suppressor;

    /// <summary>Gets the <seealso cref="StoredDescriptorDiagnosticSuppressor"/> instance under test.</summary>
    protected TSuppressor Suppressor => suppressor ??= new TSuppressor();

    /// <summary>Gets the suppression descriptor with the given suppression ID from the suppressor's supported suppressions.</summary>
    /// <param name="suppressionId">The suppression ID to look up.</param>
    /// <returns>The matching <seealso cref="SuppressionDescriptor"/>, or <see langword="null"/> if not found.</returns>
    protected SuppressionDescriptor? GetSuppressionDescriptor(string suppressionId)
    {
        return Suppressor.SupportedSuppressions
            .FirstOrDefault(s => string.Equals(s.Id, suppressionId, StringComparison.Ordinal));
    }

    /// <summary>Asserts that the suppressor declares a suppression with the given ID.</summary>
    /// <param name="suppressionId">The suppression ID to verify.</param>
    protected void AssertSuppressionDeclared(string suppressionId)
    {
        var descriptor = GetSuppressionDescriptor(suppressionId);
        if (descriptor is null)
            throw new InvalidOperationException(
                $"Suppressor does not declare a suppression with ID '{suppressionId}'.");
    }
}
