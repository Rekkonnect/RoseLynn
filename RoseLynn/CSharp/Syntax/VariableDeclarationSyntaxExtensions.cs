using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using RoseLynn.CSharp.Syntax;
using System.Collections.Generic;

namespace RoseLynn.CSharp.Syntax;

/// <summary>
/// Provides extensions for the <see cref="VariableDeclarationSyntax"/> node.
/// </summary>
public static class VariableDeclarationSyntaxExtensions
{
    /// <summary>
    /// Gets the declared variables of the given <seealso cref="VariableDeclarationSyntax"/>,
    /// in conjuction with its appropriate <seealso cref="SemanticModel"/>.
    /// </summary>
    /// <param name="variableDeclarationSyntax">
    /// The <seealso cref="VariableDeclarationSyntax"/> whose local variables to get.
    /// </param>
    /// <param name="semanticModel">
    /// The <seealso cref="SemanticModel"/> that contains information about the specified node.
    /// </param>
    /// <returns>The defined local symbols for the given <seealso cref="VariableDeclarationSyntax"/>.</returns>
    public static IEnumerable<ILocalSymbol> GetDeclaredVariables(this VariableDeclarationSyntax variableDeclarationSyntax, SemanticModel semanticModel)
    {
        var variableDeclarationOperation = semanticModel.GetOperation(variableDeclarationSyntax) as IVariableDeclarationOperation;
        return variableDeclarationOperation!.GetDeclaredVariables();
    }
}
