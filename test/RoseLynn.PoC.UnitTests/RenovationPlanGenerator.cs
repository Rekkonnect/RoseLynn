using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.Generators;
using System.Collections.Immutable;
using System.Text;

namespace RoseLynn.PoC.UnitTests;

public sealed class RenovationPlanGenerator : IIncrementalGenerator
{
    private const string RenovateAttributeMetadataName = "RoseLynn.PoC.UnitTests.RenovateAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var targets = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                RenovateAttributeMetadataName,
                static (attributeContext, cancellationToken) => GetTarget(attributeContext, cancellationToken))
            .WhereNotNull();

        context.RegisterSourceOutput(
            targets,
            static (productionContext, target) =>
            {
                productionContext.AddSource(target!.HintName, GenerateSource(target));
            });
    }

    private static RenovationTarget? GetTarget(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        if (context.TargetNode is not TypeDeclarationSyntax declaration)
            return null;

        if (!declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(declaration, cancellationToken) is not INamedTypeSymbol symbol)
            return null;

        var containingTypes = GetContainingTypes(symbol);
        var members = declaration.Members
            .Select(GetMemberName)
            .OfType<string>()
            .Where(static name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.Ordinal)
            .ToImmutableArray();

        return new(
            NamespaceName: symbol.ContainingNamespace is { IsGlobalNamespace: false } containingNamespace
                ? containingNamespace.ToDisplayString()
                : string.Empty,
            TypeName: symbol.Name,
            TypeKindDeclaration: GetTypeKindDeclaration(declaration),
            TypeParameters: declaration.TypeParameterList?.ToFullString() ?? string.Empty,
            ContainingTypes: containingTypes,
            Members: members,
            HintName: $"{GetFullyQualifiedMetadataName(symbol)}.RenovationPlan.g.cs");
    }

    private static ImmutableArray<ContainingTypeTarget> GetContainingTypes(INamedTypeSymbol symbol)
    {
        var builder = ImmutableArray.CreateBuilder<ContainingTypeTarget>();
        for (var current = symbol.ContainingType; current is not null; current = current.ContainingType)
        {
            builder.Add(new(
                Name: current.Name,
                TypeKindDeclaration: GetTypeKindDeclaration(current),
                TypeParameters: GetTypeParameters(current)));
        }

        builder.Reverse();
        return builder.ToImmutable();
    }

    private static string GetTypeKindDeclaration(TypeDeclarationSyntax declaration)
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

    private static string GetTypeKindDeclaration(INamedTypeSymbol symbol)
    {
        return symbol switch
        {
            { IsRecord: true, TypeKind: TypeKind.Struct } => "partial record struct",
            { IsRecord: true } => "partial record",
            { TypeKind: TypeKind.Interface } => "partial interface",
            { TypeKind: TypeKind.Struct } => "partial struct",
            _ => "partial class",
        };
    }

    private static string GetTypeParameters(INamedTypeSymbol symbol)
    {
        if (symbol.TypeParameters.Length == 0)
            return string.Empty;

        return $"<{string.Join(", ", symbol.TypeParameters.Select(static p => p.Name))}>";
    }

    private static string GetFullyQualifiedMetadataName(INamedTypeSymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", string.Empty, StringComparison.Ordinal)
            .Replace('<', '{')
            .Replace('>', '}');
    }

    private static string? GetMemberName(MemberDeclarationSyntax member)
    {
        return member switch
        {
            FieldDeclarationSyntax field => field.Declaration.Variables.FirstOrDefault()?.Identifier.ValueText,
            EventFieldDeclarationSyntax eventField => eventField.Declaration.Variables.FirstOrDefault()?.Identifier.ValueText,
            PropertyDeclarationSyntax property => property.Identifier.ValueText,
            IndexerDeclarationSyntax => "Indexer",
            EventDeclarationSyntax @event => @event.Identifier.ValueText,
            MethodDeclarationSyntax method => method.Identifier.ValueText,
            ConstructorDeclarationSyntax constructor => constructor.Identifier.ValueText,
            DestructorDeclarationSyntax => "Destructor",
            OperatorDeclarationSyntax @operator => @operator.OperatorToken.ValueText,
            ConversionOperatorDeclarationSyntax conversion => conversion.Type.ToString(),
            TypeDeclarationSyntax type => type.Identifier.ValueText,
            DelegateDeclarationSyntax @delegate => @delegate.Identifier.ValueText,
            EnumDeclarationSyntax @enum => @enum.Identifier.ValueText,
            _ => null,
        };
    }

    private static string GenerateSource(RenovationTarget target)
    {
        var builder = new StringBuilder();

        builder.AppendLine("// <auto-generated/>");

        if (!string.IsNullOrEmpty(target.NamespaceName))
        {
            builder.Append("namespace ");
            builder.Append(target.NamespaceName);
            builder.AppendLine(";");
            builder.AppendLine();
        }

        foreach (var containingType in target.ContainingTypes)
        {
            builder.Append(containingType.TypeKindDeclaration);
            builder.Append(' ');
            builder.Append(containingType.Name);
            builder.Append(containingType.TypeParameters);
            builder.AppendLine();
            builder.AppendLine("{");
        }

        builder.Append(target.TypeKindDeclaration);
        builder.Append(' ');
        builder.Append(target.TypeName);
        builder.Append(target.TypeParameters);
        builder.AppendLine();
        builder.AppendLine("{");

        foreach (var member in target.Members)
        {
            builder.Append("    public static readonly global::System.Collections.Generic.IReadOnlyList<string> ");
            builder.Append(SanitizeIdentifier(member));
            builder.Append("RenovationPlan = new string[] { \"");
            builder.Append(EscapeString(member));
            builder.AppendLine("\" };");
        }

        builder.AppendLine("}");

        foreach (var _ in target.ContainingTypes)
        {
            builder.AppendLine("}");
        }

        return builder.ToString();
    }

    private static string SanitizeIdentifier(string memberName)
    {
        var builder = new StringBuilder(memberName.Length);
        foreach (var character in memberName)
        {
            builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }

        if (builder.Length == 0 || !SyntaxFacts.IsIdentifierStartCharacter(builder[0]))
            builder.Insert(0, '_');

        return builder.ToString();
    }

    private static string EscapeString(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
    }

    private sealed record RenovationTarget(
        string NamespaceName,
        string TypeName,
        string TypeKindDeclaration,
        string TypeParameters,
        ImmutableArray<ContainingTypeTarget> ContainingTypes,
        ImmutableArray<string> Members,
        string HintName);

    private sealed record ContainingTypeTarget(
        string Name,
        string TypeKindDeclaration,
        string TypeParameters);
}
