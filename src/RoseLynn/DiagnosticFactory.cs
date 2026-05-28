using Garyon.Extensions.Comparison;
using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Contains factory methods for constructing <seealso cref="Diagnostic"/> instances.</summary>
public static class DiagnosticFactory
{
    /// <summary>Creates a <seealso cref="Diagnostic"/> from the given descriptor and syntax reference.</summary>
    /// <param name="descriptor">The diagnostic descriptor.</param>
    /// <param name="reference">The syntax reference whose location will be used for the diagnostic.</param>
    /// <returns>The created diagnostic.</returns>
    public static Diagnostic Create(DiagnosticDescriptor descriptor, SyntaxReference reference)
    {
        return Diagnostic.Create(descriptor, LocationFactory.Create(reference));
    }
}
