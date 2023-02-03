using Microsoft.CodeAnalysis;
using RoseLynn.Analyzers;
using System.Resources;

namespace RoseLynn.InternalGenerators;

public class GeneratorDiagnosticDescriptorStorage : DiagnosticDescriptorStorageBase
{
    public static GeneratorDiagnosticDescriptorStorage Instance { get; } = new();

    // No docs
    protected override string BaseRuleDocsURI => "";
    protected override string DiagnosticIDPrefix => "REN"; // Ros[E]lyn[N]

    protected override ResourceManager ResourceManager => DiagnosticResources.ResourceManager;

    private GeneratorDiagnosticDescriptorStorage()
    {
        // Gladly I never imposed a type restriction
        SetDefaultDiagnosticAnalyzer<OperatorKindFactsGenerator>();

        CreateDiagnosticDescriptor(0001, Categories.Validity);
    }

    protected override DiagnosticSeverity? GetDefaultSeverity(string category)
    {
        return category switch
        {
            Categories.Validity => DiagnosticSeverity.Error,
            _ => null,
        };
    }

    private static class Categories
    {
        public const string Validity = nameof(Validity);
    }
}
