using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn.Testing;

public static class TestSourceFileHelpers
{
    public static async Task<string> InferSourceName(
        string source, CancellationToken cancellationToken)
    {
        source = MarkupParsingHelpers.ExtractActualSource(source);
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken);
        var root = await tree.GetRootAsync(cancellationToken);
        return CreateFileName(root);
    }

    private static string CreateFileName(SyntaxNode root)
    {
        var primaryDeclaration = root
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .FirstOrDefault();

        var fileName = CreateDeclarationFileName(primaryDeclaration);
        if (fileName is not null)
        {
            return fileName;
        }

        var globalFeatures = root.GetGlobalSyntaxFeatures();
        if (globalFeatures.HasFlag(GlobalSyntaxFeatures.FileProgram))
        {
            return "Program.cs";
        }

        if (globalFeatures.HasFlag(GlobalSyntaxFeatures.AssemblyAttributes))
        {
            return "_AssemblyAttributes.cs";
        }

        if (globalFeatures.HasFlag(GlobalSyntaxFeatures.ModuleAttributes))
        {
            return "_ModuleAttributes.cs";
        }

        if (globalFeatures.HasFlag(GlobalSyntaxFeatures.GlobalUsings))
        {
            return "_GlobalUsings.cs";
        }

        return "TestSource.cs";
    }

    private static string? CreateDeclarationFileName(BaseTypeDeclarationSyntax? declaration)
    {
        if (declaration is null)
        {
            return null;
        }

        var arity = declaration.GetArity();
        var identifierWithArity = new IdentifierWithArity(declaration.Identifier.Text, arity);
        return $"{identifierWithArity.FullIdentifier}.cs";
    }
}
