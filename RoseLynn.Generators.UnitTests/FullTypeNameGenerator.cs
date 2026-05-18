using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.Generators.UnitTests;

/// <summary>
/// Generates a partial declaration for each partial type that exposes its fully qualified name.
/// </summary>
/// <remarks>
/// This is a trivial generator for testing purposes only, focused 
/// on generating full type names for partial types.
/// </remarks>
public sealed class FullTypeNameGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax declaration && IsPartial(declaration),
                static (context, cancellationToken) => GetTypeInfo(context, cancellationToken))
            .Where(static typeInfo => typeInfo is not null)!;

        context.RegisterSourceOutput(
            types,
            static (productionContext, typeInfo) =>
            {
                productionContext.AddSource(typeInfo!.HintName, GenerateSource(typeInfo));
            });
    }

    private static bool IsPartial(TypeDeclarationSyntax declaration)
    {
        return declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
    }

    private static FullTypeNameTarget? GetTypeInfo(
        GeneratorSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;
        if (context.SemanticModel.GetDeclaredSymbol(declaration, cancellationToken) is not INamedTypeSymbol symbol)
        {
            return null;
        }

        var namespaceName = symbol.ContainingNamespace is { IsGlobalNamespace: false } containingNamespace
            ? containingNamespace.ToDisplayString()
            : string.Empty;
        var typeName = symbol.Name;
        var fullTypeName = string.IsNullOrEmpty(namespaceName)
            ? typeName
            : $"{namespaceName}.{typeName}";

        return new(
            NamespaceName: namespaceName,
            TypeName: typeName,
            DeclarationKeyword: GetDeclarationKeyword(declaration),
            FullTypeName: fullTypeName,
            HintName: $"{fullTypeName}.FullTypeName.g.cs");
    }

    private static string GetDeclarationKeyword(TypeDeclarationSyntax declaration)
    {
        return declaration switch
        {
            InterfaceDeclarationSyntax => "partial interface",
            StructDeclarationSyntax => "partial struct",
            RecordDeclarationSyntax { ClassOrStructKeyword.RawKind: (int)SyntaxKind.StructKeyword } => "partial record struct",
            RecordDeclarationSyntax => "partial record",
            _ => "partial class",
        };
    }

    private static string GenerateSource(FullTypeNameTarget typeInfo)
    {
        var namespacePrefix = string.IsNullOrEmpty(typeInfo.NamespaceName)
            ? string.Empty
            : $"namespace {typeInfo.NamespaceName};\r\n\r\n";

        return $$"""
{{namespacePrefix}}{{typeInfo.DeclarationKeyword}} {{typeInfo.TypeName}}
{
    public const string FullTypeName = "{{typeInfo.FullTypeName}}";
}
""";
    }

    private sealed record FullTypeNameTarget(
        string NamespaceName,
        string TypeName,
        string DeclarationKeyword,
        string FullTypeName,
        string HintName);
}