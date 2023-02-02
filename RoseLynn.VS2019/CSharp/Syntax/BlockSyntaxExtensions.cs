using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.CSharp.Syntax;

/// <summary>
/// Provides extensions for the <seealso cref="BlockSyntax"/> node.
/// </summary>
public static class BlockSyntaxExtensions
{
    /// <summary>
    /// Gets all the child nodes of the given block syntax, exlcuding local method delcarations.
    /// </summary>
    /// <param name="blockSyntax">
    /// The block syntax from which to filter out <seealso cref="LocalFunctionStatementSyntax"/> nodes.
    /// </param>
    /// <returns>
    /// All of the block syntax's filtered statements' <seealso cref="SyntaxNode.ChildNodes"/> results.
    /// </returns>
    public static IEnumerable<SyntaxNode> ChildNodesExcludingLocalMethods(this BlockSyntax blockSyntax)
    {
        var statements = blockSyntax.Statements;
        var nonLocalMethodSyntaxes = statements.Where(s => s is not LocalFunctionStatementSyntax);
        return nonLocalMethodSyntaxes.SelectMany(s => s.ChildNodes());
    }
}