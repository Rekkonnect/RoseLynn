using Microsoft.CodeAnalysis;

namespace RoseLynn.InternalGenerators;

public static class DiagnosticFactoryEx
{
    public static Diagnostic Create(DiagnosticDescriptor descriptor, SyntaxReference reference)
    {
        return Diagnostic.Create(descriptor, LocationFactoryEx.Create(reference));
    }
}
