#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;

namespace RoseLynn;

/// <summary>Provides useful extensions for the <seealso cref="SemanticModel"/> type.</summary>
public static class SemanticModelExtensions
{
    /// <summary>Gets the associated <seealso cref="TypeInfo"/> for the type in a <seealso cref="BaseTypeSyntax"/>.</summary>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> from which to get the information about the type in the <seealso cref="BaseTypeSyntax"/>.</param>
    /// <param name="baseType">The <seealso cref="BaseTypeSyntax"/> whose type info to get from the semantic model.</param>
    /// <returns>The <seealso cref="TypeInfo"/> regarding the type in the <seealso cref="BaseTypeSyntax"/>.</returns>
    public static TypeInfo GetTypeInfo(this SemanticModel semanticModel, BaseTypeSyntax baseType) => semanticModel.GetTypeInfo(baseType.Type);

    /// <summary>Gets the alias info or the symbol info associated with the provided <seealso cref="SyntaxNode"/>.</summary>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> from which to get the symbol info.</param>
    /// <param name="node">The <seealso cref="SyntaxNode"/> representing a symbol.</param>
    /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> for the operation of retrieving both the symbols.</param>
    /// <returns>An <seealso cref="IAliasSymbol"/> if alias symbol information can be retrievd from <seealso cref="ModelExtensions.GetAliasInfo(SemanticModel, SyntaxNode, CancellationToken)"/>, otherwise the <seealso cref="ISymbol"/> that <seealso cref="ModelExtensions.GetSymbolInfo(SemanticModel, SyntaxNode, CancellationToken)"/> returns.</returns>
    public static ISymbol? GetAliasOrSymbolInfo(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken = default)
    {
        var alias = semanticModel.GetAliasInfo(node, cancellationToken);
        return alias ?? semanticModel.GetSymbolInfo(node, cancellationToken).Symbol;
    }

    /// <summary>Gets the symbol a given <seealso cref="SyntaxNode"/> represents reinterpreted as the specified type.</summary>
    /// <typeparam name="TSymbol">The type of the symbol to reinterpret it as.</typeparam>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> to get the symbol from.</param>
    /// <param name="node">The syntax node from which to get the symbol.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The represented <seealso cref="ISymbol"/> reinterpreted as <typeparamref name="TSymbol"/>.</returns>
    /// <remarks>
    /// The <seealso cref="ModelExtensions.GetSymbolInfo(SemanticModel, SyntaxNode, CancellationToken)"/> method is used.
    /// If better suited, consider using <seealso cref="GetTypeSymbol{TSymbol}(SemanticModel, SyntaxNode, CancellationToken)"/>.
    /// </remarks>
    public static TSymbol? GetSymbol<TSymbol>(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken = default)
        where TSymbol : class, ISymbol
    {
        return semanticModel.GetSymbolInfo(node, cancellationToken).Symbol as TSymbol;
    }
    /// <summary>Gets the type symbol a given <seealso cref="SyntaxNode"/> represents reinterpreted as the specified type.</summary>
    /// <typeparam name="TSymbol">The type of the symbol to reinterpret it as.</typeparam>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> to get the symbol from.</param>
    /// <param name="node">The syntax node from which to get the type symbol.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The represented <seealso cref="ITypeSymbol"/> reinterpreted as <typeparamref name="TSymbol"/>.</returns>
    /// <remarks>
    /// The <seealso cref="ModelExtensions.GetTypeInfo(SemanticModel, SyntaxNode, CancellationToken)"/> method is used.
    /// If better suited, consider using <seealso cref="GetSymbol{TSymbol}(SemanticModel, SyntaxNode, CancellationToken)"/>.
    /// </remarks>
    public static TSymbol? GetTypeSymbol<TSymbol>(this SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken = default)
        where TSymbol : class, ITypeSymbol
    {
        return semanticModel.GetTypeInfo(node, cancellationToken).Type as TSymbol;
    }

    /// <summary>Gets the type symbol a given <seealso cref="AttributeSyntax"/> represents.</summary>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> to get the symbol from.</param>
    /// <param name="attributeNode">The <seealso cref="AttributeSyntax"/> whose type to get.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The type symbol of the attribute.</returns>
    public static INamedTypeSymbol GetAttributeTypeSymbol(this SemanticModel semanticModel, AttributeSyntax attributeNode, CancellationToken cancellationToken = default)
    {
        return semanticModel.GetTypeSymbol<INamedTypeSymbol>(attributeNode, cancellationToken)!;
    }

    /// <summary>
    /// Gets the declared symbol based on the result of the
    /// <seealso cref="ModelExtensions.GetDeclaredSymbol(SemanticModel, SyntaxNode, CancellationToken)"/>
    /// method, or the anonymous method symbol if the provided syntax node represents
    /// an anonymous function expression syntax.
    /// </summary>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> to get the symbol from.</param>
    /// <param name="node">The syntax node that declares the symbol.</param>
    /// <returns>
    /// The result of the
    /// <seealso cref="ModelExtensions.GetDeclaredSymbol(SemanticModel, SyntaxNode, CancellationToken)"/>
    /// extension, if not <see langword="null"/>, or the symbol info from the
    /// <seealso cref="ModelExtensions.GetSymbolInfo(SemanticModel, SyntaxNode, CancellationToken)"/>
    /// extension, if the node represents an anonyous method declaration,
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// Currently, only the <seealso cref="AnonymousFunctionExpressionSyntax"/> type
    /// for C# is supported.
    /// </remarks>
    public static ISymbol? GetDeclaredOrAnonymousSymbol(this SemanticModel semanticModel, SyntaxNode node)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(node);
        if (declaredSymbol is not null)
            return declaredSymbol;

        if (node is AnonymousFunctionExpressionSyntax)
            return semanticModel.GetSymbolInfo(node).Symbol;

        // TODO: Handle the respective case for VB

        return null;
    }
}
