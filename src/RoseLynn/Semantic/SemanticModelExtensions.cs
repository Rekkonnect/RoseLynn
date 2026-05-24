using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn.Semantic;

public static class SemanticModelExtensions
{
    public static SemanticModel? GetSemanticModelForNode(
        this SemanticModel model,
        SyntaxNode node)
    {
        return model.Compilation.GetSemanticModel(node.SyntaxTree);
    }

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
