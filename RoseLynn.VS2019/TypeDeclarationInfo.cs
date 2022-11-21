using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn;

#nullable enable

/// <summary>Contains information about a type declaration, linking its declaration syntax node with the declared type symbol.</summary>
public class TypeDeclarationInfo
{
    /// <summary>The declaration syntax node.</summary>
    public BaseTypeDeclarationSyntax DeclarationNode { get; }
    /// <summary>The declared type symbol.</summary>
    public INamedTypeSymbol DeclaredType { get; }

    /// <summary>Initializes a new instance of the <seealso cref="TypeDeclarationInfo"/> class from a declaration node and its semantic model, from which to retrieve the declared type symbol.</summary>
    /// <param name="declarationNode">The declaration node.</param>
    /// <param name="semanticModel">The semantic model from which the declared type symbol will be retrieved.</param>
    public TypeDeclarationInfo(BaseTypeDeclarationSyntax declarationNode, SemanticModel semanticModel)
    {
        DeclarationNode = declarationNode;
        DeclaredType = semanticModel.GetDeclaredSymbol(declarationNode)!;
    }
}