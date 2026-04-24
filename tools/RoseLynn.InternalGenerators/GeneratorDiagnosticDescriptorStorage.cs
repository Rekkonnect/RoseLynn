using Microsoft.CodeAnalysis;

namespace RoseLynn.InternalGenerators;

#pragma warning disable RS2008 // Enable analyzer release tracking

public static class GeneratorDiagnosticDescriptorStorage
{
    public static readonly DiagnosticDescriptor Rose0001 = new(
        id: "ROSE0001",
        title: "Missing arity defition for operator kind",
        messageFormat: "Operator kinds should always have defined arities",
        category: Categories.Validity,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static class Categories
    {
        public const string Validity = nameof(Validity);
    }
}
