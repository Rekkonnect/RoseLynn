using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp;
using RoseLynn.Core.Semantic;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoseLynn.CSharp.Semantic;

/// <summary>Provides helpers for detecting and enumerating instance member references within a syntax tree, primarily for refactoring and static-conversion analysis.</summary>
public static class InstanceReferenceHelpers
{
    /// <summary>Returns <see langword="true"/> if the given declaration contains any instance member references that cannot be trivially converted to static, recursing into referenced methods.</summary>
    /// <param name="declaration">The syntax node representing the member declaration to analyse.</param>
    /// <param name="model">The semantic model for the declaration.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public static async Task<bool> HasUnconvertibleInstanceReferences(
        this SyntaxNode declaration,
        SemanticModel model,
        CancellationToken cancellationToken)
    {
        var declared = model.GetDeclaredSymbol(declaration, cancellationToken);
        if (declared is null)
        {
            return false;
        }

        var instanceReferences = GetInstanceReferences(
            declaration, model, cancellationToken);

        foreach (var info in instanceReferences)
        {
            // Methods are easily transitively convertible, so if they do not
            // have any references to instance fields, properties or events,
            // converting them to static is desirable
            // Converting instance fields, properties or events is an incorrect
            // fix for 99.999% of the cases, if not 100%
            if (info.Symbol is not IMethodSymbol)
            {
                return true;
            }
        }

        var convertedMethodsLocations = instanceReferences
            .Select(s => s.Symbol!)
            .SelectMany(s => s.Locations)
            ;

        foreach (var location in convertedMethodsLocations)
        {
            var node = await location.GetNodeAsync(cancellationToken);
            if (node is null)
            {
                continue;
            }

            var nextModel = model.GetSemanticModelForNode(node);
            Asserts.NotNull(nextModel);
            var hasUnconvertible = await HasUnconvertibleInstanceReferences(
                node, nextModel, cancellationToken);
            if (hasUnconvertible)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Asynchronously enumerates the syntax nodes of instance methods referenced within the given declaration that can be safely converted to static.</summary>
    /// <param name="declaration">The syntax node representing the member declaration to analyse.</param>
    /// <param name="model">The semantic model for the declaration.</param>
    /// <param name="cancellationToken">A token to cancel the enumeration.</param>
    public static async IAsyncEnumerable<SyntaxNode> GetConvertibleInstanceMembersReferences(
        this SyntaxNode declaration,
        SemanticModel model,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        var declared = model.GetDeclaredSymbol(declaration, cancellationToken);
        if (declared is null)
        {
            yield break;
        }

        var instanceReferences = GetInstanceReferences(
            declaration, model, cancellationToken);

        var convertedMethodsLocations = instanceReferences
            .Select(s => s.Symbol)
            .OfType<IMethodSymbol>()
            .SelectMany(s => s.Locations)
            ;

        foreach (var location in convertedMethodsLocations)
        {
            var node = await location.GetNodeAsync(cancellationToken);
            if (node is null)
            {
                continue;
            }

            yield return node;

            var nextModel = model.Compilation.GetSemanticModel(node.SyntaxTree);
            var nextReferences = GetConvertibleInstanceMembersReferences(
                node, nextModel, cancellationToken);
            await foreach (var reference in nextReferences)
            {
                yield return reference;
            }
        }
    }

    private static ImmutableArray<SymbolInstanceInfo> GetInstanceReferences(
        SyntaxNode declaration,
        SemanticModel model,
        CancellationToken cancellationToken)
    {
        var visitor = RootSymbolReferencesOfExpressionVisitor.Instance;
        return declaration.ChildNodes()
            .SelectMany(visitor.Visit)
            .Select(node => GetSymbolInstanceInfo(node, model, cancellationToken))
            .Where(info => info.IsInstanceReference is true)
            .ToImmutableArray();
    }

    /// <summary>Returns <see langword="true"/> if the given declaration contains any direct instance member references.</summary>
    /// <param name="declaration">The syntax node representing the member declaration to analyse.</param>
    /// <param name="model">The semantic model for the declaration.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public static bool HasInstanceReferences(
        this SyntaxNode declaration,
        SemanticModel model,
        CancellationToken cancellationToken)
    {
        var declared = model.GetDeclaredSymbol(declaration, cancellationToken);
        if (declared is null)
        {
            return false;
        }

        var visitor = RootSymbolReferencesOfExpressionVisitor.Instance;
        return declaration.ChildNodes()
            .SelectMany(visitor.Visit)
            .Any(node => GetSymbolInstanceInfo(node, model, cancellationToken)
                .IsInstanceReference);
    }

    private static SymbolInstanceInfo GetSymbolInstanceInfo(
        SyntaxNode node, SemanticModel model, CancellationToken cancellationToken)
    {
        var symbolInfo = model.GetSymbolInfo(node, cancellationToken);
        var symbol = symbolInfo.SymbolOrSingleCandidateOrDefault();

        var isInstance = symbol switch
        {
            IPropertySymbol or
            IFieldSymbol or
            IEventSymbol or
            IMethodSymbol => !symbol.IsStatic,
            IParameterSymbol { IsThis: true } => true,
            IParameterSymbol parameter => (parameter.ContainingSymbol as IMethodSymbol)!
                .IsPrimaryConstructor(cancellationToken),
            _ => false,
        };
        return new(symbol, isInstance);
    }

    private readonly record struct SymbolInstanceInfo(
        ISymbol? Symbol,
        bool IsInstanceReference);

    /// <summary>Returns the resolved symbol if available, or the single candidate symbol if exactly one candidate was found, or <see langword="null"/>.</summary>
    /// <param name="symbolInfo">The <seealso cref="SymbolInfo"/> to inspect.</param>
    public static ISymbol? SymbolOrSingleCandidateOrDefault(this SymbolInfo symbolInfo)
    {
        return symbolInfo.Symbol
            ?? (symbolInfo.CandidateSymbols is [var single] ? single : null);
    }

    private sealed class RootSymbolReferencesOfExpressionVisitor
        : CSharpSyntaxVisitor<IEnumerable<SyntaxNode>>
    {
        public static readonly RootSymbolReferencesOfExpressionVisitor Instance = new();

        public override IEnumerable<SyntaxNode> VisitElementBindingExpression(
            ElementBindingExpressionSyntax node)
        {
            return VisitArguments(node.ArgumentList.Arguments);
        }

        public override IEnumerable<SyntaxNode>? VisitMemberBindingExpression(
            MemberBindingExpressionSyntax node)
        {
            return [];
        }

        public override IEnumerable<SyntaxNode> VisitMemberAccessExpression(
            MemberAccessExpressionSyntax node)
        {
            return Visit(node.Expression);
        }

        public override IEnumerable<SyntaxNode> VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            return Visit(node.Expression);
        }

        public override IEnumerable<SyntaxNode> VisitThisExpression(ThisExpressionSyntax node)
        {
            return [node];
        }

        public override IEnumerable<SyntaxNode> VisitIdentifierName(IdentifierNameSyntax node)
        {
            return [node];
        }

        public override IEnumerable<SyntaxNode> VisitGenericName(GenericNameSyntax node)
        {
            return [node];
        }

        public override IEnumerable<SyntaxNode> Visit(SyntaxNode? node)
        {
            return base.Visit(node) ?? [];
        }

        public override IEnumerable<SyntaxNode> DefaultVisit(SyntaxNode node)
        {
            return VisitMany(node.ChildNodes());
        }

        private IEnumerable<SyntaxNode> VisitMany(IEnumerable<SyntaxNode> nodes)
        {
            return nodes.SelectMany(Visit);
        }

        private IEnumerable<SyntaxNode> VisitArguments(
            IReadOnlyList<ArgumentSyntax> arguments)
        {
            foreach (var argument in arguments)
            {
                var argumentRoots = Visit(argument.Expression);
                foreach (var root in argumentRoots)
                {
                    yield return root;
                }
            }
        }
    }
}
