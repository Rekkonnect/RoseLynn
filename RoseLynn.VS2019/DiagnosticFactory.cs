using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Contains factory methods for constructing <seealso cref="Diagnostic"/> instances.</summary>
public static class DiagnosticFactory
{
    public static Diagnostic Create(DiagnosticDescriptor descriptor, SyntaxReference reference)
    {
        return Diagnostic.Create(descriptor, LocationFactory.Create(reference));
    }
}
