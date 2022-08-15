using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn;

/// <summary>Provides extensions for the <seealso cref="SyntaxTree"/> class.</summary>
public static class SyntaxTreeExtensions
{
    /// <summary>Gets all the nodes of the <seealso cref="SyntaxTree"/> of the specified type.</summary>
    /// <typeparam name="T">The type of the nodes to get.</typeparam>
    /// <param name="syntaxTree">The <seealso cref="SyntaxTree"/> whose nodes of the specified type to get.</param>
    /// <returns>A collection of <seealso cref="SyntaxNode"/> of the specified type contained in the specified <seealso cref="SyntaxTree"/>.</returns>
    public static IEnumerable<T> NodesOfType<T>(this SyntaxTree syntaxTree)
        where T : SyntaxNode
    {
        return syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<T>();
    }
}
