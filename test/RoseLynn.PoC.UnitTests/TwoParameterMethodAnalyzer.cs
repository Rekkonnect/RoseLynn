using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.Analyzers;
using System.Collections.Immutable;

namespace RoseLynn.PoC.UnitTests;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TwoParameterMethodAnalyzer : CSharpDiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor POC0001_TwoParameterMethodRule = new(
        id: "POC0001",
        title: "Method contains two parameters",
        messageFormat: "Method '{0}' contains two parameters",
        category: PoCDiagnosticDescriptorStorage.Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Demonstrates a PoC analyzer that flags methods declaring exactly two parameters.",
        helpLinkUri: "https://example.invalid/POC0001");

    protected override ImmutableArray<DiagnosticDescriptor> DeclaredDiagnostics =>
    [
        POC0001_TwoParameterMethodRule,
    ];

    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => PoCDiagnosticDescriptorStorage.Instance;

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeMethod, Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.ParameterList.Parameters.Count != 2)
            return;

        var diagnostic = Diagnostic.Create(
            POC0001_TwoParameterMethodRule,
            method.ParameterList.Parameters[1].GetLocation(),
            method.Identifier.ValueText);

        context.ReportDiagnostic(diagnostic);
    }
}
