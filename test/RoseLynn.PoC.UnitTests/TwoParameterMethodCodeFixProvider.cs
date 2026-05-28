using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CodeFixes;

namespace RoseLynn.PoC.UnitTests;

public sealed class TwoParameterMethodCodeFixProvider : MultipleDiagnosticCodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors =>
    [
        PoCDiagnosticDescriptorStorage.Instance.POC0001_TwoParameterMethodRule,
    ];

    public override string CodeFixTitle => "Remove second parameter";

    protected override async Task<Document> PerformCodeFixActionAsync(
        CodeFixContext context,
        SyntaxNode syntaxNode,
        CancellationToken cancellationToken)
    {
        var parameter = syntaxNode as ParameterSyntax
            ?? syntaxNode.FirstAncestorOrSelf<ParameterSyntax>();
        if (parameter?.Parent is not ParameterListSyntax parameterList)
            return context.Document;

        var method = parameterList.Parent as MethodDeclarationSyntax;
        if (method is null || parameterList.Parameters.Count < 2)
            return context.Document;

        var updatedMethod = method.WithParameterList(
            parameterList.WithParameters(parameterList.Parameters.RemoveAt(1)));

        return await context.Document.WithSyntaxRootAsync(
            root => root.ReplaceNode(method, updatedMethod),
            cancellationToken);
    }
}
