using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;
using RoseLynn.CSharp;
using RoseLynn.CSharp.Semantic;
using RoseLynn.Semantic;
using System.Collections.Immutable;

namespace RoseLynn.PoC.UnitTests;

public sealed class IsolatedRoslynUsagePoCService
{
    public ImmutableArray<SyntaxFilteredTypeCandidate> DiscoverSyntaxFilteredTypes(
        IEnumerable<DiscoveredCSharpSource> discoveredSources,
        CancellationToken cancellationToken = default)
    {
        var builder = ImmutableArray.CreateBuilder<SyntaxFilteredTypeCandidate>();

        foreach (var discoveredSource in discoveredSources)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tree = CSharpSyntaxTree.ParseText(
                discoveredSource.Text,
                path: discoveredSource.FilePath,
                cancellationToken: cancellationToken);

            foreach (var declaration in tree.NodesOfType<ClassDeclarationSyntax>())
            {
                if (!MatchesSyntaxConstraints(declaration))
                    continue;

                builder.Add(new(
                    FilePath: discoveredSource.FilePath,
                    NamespaceName: GetNamespaceName(declaration),
                    TypeName: declaration.Identifier.ValueText,
                    DeclaredMethodCount: declaration.Members.OfType<MethodDeclarationSyntax>().Count()));
            }
        }

        return builder
            .OrderBy(static candidate => candidate.FilePath, StringComparer.Ordinal)
            .ThenBy(static candidate => candidate.NamespaceName, StringComparer.Ordinal)
            .ThenBy(static candidate => candidate.TypeName, StringComparer.Ordinal)
            .ToImmutableArray();
    }

    public ImmutableArray<MethodClassUsageReport> AnalyzeMethodClassUsagePermutations(
        Compilation compilation,
        CancellationToken cancellationToken = default)
    {
        var definedClasses = compilation.GetAllDefinedTypes()
            .Where(static type => type.TypeKind is TypeKind.Class)
            .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        var reports = compilation.NodesOfType<MethodDeclarationSyntax>()
            .Select(method => AnalyzeMethod(compilation, method, definedClasses, cancellationToken))
            .Where(static report => report is not null)
            .Cast<MethodClassUsageReport>()
            .OrderBy(static report => report.MethodFullName, StringComparer.Ordinal)
            .ToImmutableArray();

        return reports;
    }

    private static MethodClassUsageReport? AnalyzeMethod(
        Compilation compilation,
        MethodDeclarationSyntax method,
        ImmutableHashSet<INamedTypeSymbol> definedClasses,
        CancellationToken cancellationToken)
    {
        if (method.Body is null && method.ExpressionBody is null)
            return null;

        var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
        var usedClasses = CollectUsedClasses(
            semanticModel,
            method,
            definedClasses,
            cancellationToken);

        if (usedClasses.Length < 2)
            return null;

        var declaredMethod = semanticModel.GetDeclaredSymbol(method, cancellationToken);
        var methodFullName = declaredMethod?.GetFullSymbolNameDefaultDelimiters()?.FullNameString
            ?? method.Identifier.ValueText;

        return new(
            MethodFullName: methodFullName,
            UsedClassSymbols: usedClasses,
            Permutations: BuildPermutations(usedClasses));
    }

    private static ImmutableArray<string> CollectUsedClasses(
        SemanticModel semanticModel,
        MethodDeclarationSyntax method,
        ImmutableHashSet<INamedTypeSymbol> definedClasses,
        CancellationToken cancellationToken)
    {
        var scope = method.Body as SyntaxNode
            ?? method.ExpressionBody?.Expression;
        if (scope is null)
            return [];

        var usedClasses = new HashSet<string>(StringComparer.Ordinal);
        foreach (var node in scope.DescendantNodesAndSelf())
        {
            cancellationToken.ThrowIfCancellationRequested();

            AddUsedClass(semanticModel.GetSymbolInfo(node, cancellationToken)
                .SymbolOrSingleCandidateOrDefault());

            var typeInfo = semanticModel.GetTypeInfo(node, cancellationToken);
            AddUsedClass(typeInfo.Type);
            AddUsedClass(typeInfo.ConvertedType);
        }

        return usedClasses
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToImmutableArray();

        void AddUsedClass(ISymbol? symbol)
        {
            var namedType = GetSourceDefinedClassSymbol(symbol, definedClasses);
            if (namedType is null)
                return;

            var fullName = namedType.GetFullSymbolNameDefaultDelimiters()?.FullNameString;
            if (!string.IsNullOrWhiteSpace(fullName))
                usedClasses.Add(fullName);
        }
    }

    private static INamedTypeSymbol? GetSourceDefinedClassSymbol(
        ISymbol? symbol,
        ImmutableHashSet<INamedTypeSymbol> definedClasses)
    {
        var type = symbol switch
        {
            INamedTypeSymbol named => named,
            _ => symbol.GetSymbolType() as INamedTypeSymbol,
        };

        if (type is null)
            return null;

        var originalDefinition = type.OriginalDefinition;
        return definedClasses.Contains(originalDefinition)
            ? originalDefinition
            : null;
    }

    private static ImmutableArray<ClassSymbolPermutation> BuildPermutations(
        ImmutableArray<string> usedClasses)
    {
        var builder = ImmutableArray.CreateBuilder<ClassSymbolPermutation>();
        for (var leftIndex = 0; leftIndex < usedClasses.Length - 1; leftIndex++)
        {
            for (var rightIndex = leftIndex + 1; rightIndex < usedClasses.Length; rightIndex++)
            {
                builder.Add(new(usedClasses[leftIndex], usedClasses[rightIndex]));
            }
        }

        return builder.ToImmutable();
    }

    private static bool MatchesSyntaxConstraints(ClassDeclarationSyntax declaration)
    {
        return declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.PublicKeyword))
            && declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.PartialKeyword))
            && !declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.AbstractKeyword))
            && declaration.Members.OfType<MethodDeclarationSyntax>().Count() >= 2;
    }

    private static string GetNamespaceName(ClassDeclarationSyntax declaration)
    {
        var namespaces = declaration.Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Select(static ns => ns.Name.ToString())
            .Reverse()
            .ToArray();

        return namespaces.Length is 0
            ? string.Empty
            : string.Join(".", namespaces);
    }
}

public sealed record DiscoveredCSharpSource(
    string FilePath,
    string Text);

public sealed record SyntaxFilteredTypeCandidate(
    string FilePath,
    string NamespaceName,
    string TypeName,
    int DeclaredMethodCount);

public sealed record MethodClassUsageReport(
    string MethodFullName,
    ImmutableArray<string> UsedClassSymbols,
    ImmutableArray<ClassSymbolPermutation> Permutations);

public sealed record ClassSymbolPermutation(
    string Left,
    string Right);
