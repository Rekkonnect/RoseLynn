using Microsoft.CodeAnalysis;

namespace RoseLynn.Core.Semantic;

/// <summary>Provides semantic model query helpers beyond those available on <see cref="SemanticModel"/> itself.</summary>
public static class SemanticModelExtensions
{
    /// <summary>Returns the <see cref="SemanticModel"/> from <paramref name="model"/>'s compilation that covers <paramref name="node"/>'s syntax tree.</summary>
    /// <param name="model">The semantic model whose compilation to query.</param>
    /// <param name="node">The node whose syntax tree to look up.</param>
    /// <returns>The semantic model for <paramref name="node"/>'s tree, or <see langword="null"/> if not found.</returns>
    public static SemanticModel? GetSemanticModelForNode(
        this SemanticModel model,
        SyntaxNode node)
    {
        return model.Compilation.GetSemanticModel(node.SyntaxTree);
    }

    /// <summary>Returns the declared symbol for <paramref name="node"/> if available, otherwise the resolved symbol from <see cref="SemanticModel.GetSymbolInfo"/>.</summary>
    /// <param name="model">The semantic model to query.</param>
    /// <param name="node">The syntax node to resolve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The symbol for <paramref name="node"/>, or <see langword="null"/> if neither lookup succeeds.</returns>
    public static ISymbol? GetDeclaredOrNotSymbol(
        this SemanticModel model,
        SyntaxNode node,
        CancellationToken cancellationToken = default)
    {
        return model.GetDeclaredSymbol(node, cancellationToken)
            ?? model.GetSymbolInfo(node, cancellationToken).Symbol
            ;
    }

    /// <summary>
    /// Gets all the root operations for every node provided in the list,
    /// including their descendants, according to
    /// <see cref="GetRootOperations(SemanticModel, SyntaxNode, CancellationToken)"/>.
    /// Nodes that return early are not re-evaluated.
    /// </summary>
    public static IEnumerable<IOperation> GetRootOperationsDeep(
        this SemanticModel semanticModel,
        IEnumerable<SyntaxNode> nodes,
        CancellationToken cancellationToken)
    {
        return nodes.SelectMany(
            n => semanticModel.GetRootOperations(n, cancellationToken));
    }

    /// <summary>
    /// Gets all the root operations that this node either encapsulates or represents.
    /// If the semantic model recognizes an operation for this exact node, only that
    /// single operation is returned. Otherwise, all the direct children's root operations
    /// are returned via
    /// <see cref="GetRootOperationsDeep(SemanticModel, IEnumerable{SyntaxNode}, CancellationToken)"/>.
    /// </summary>
    public static IEnumerable<IOperation> GetRootOperations(
        this SemanticModel semanticModel,
        SyntaxNode node,
        CancellationToken cancellationToken)
    {
        var operation = semanticModel.GetOperation(node, cancellationToken);
        if (operation is not null)
        {
            return [operation];
        }

        return semanticModel.GetRootOperationsDeep(node.ChildNodes(), cancellationToken);
    }
}
