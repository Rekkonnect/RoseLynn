#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;

namespace RoseLynn
{
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
    }
}
