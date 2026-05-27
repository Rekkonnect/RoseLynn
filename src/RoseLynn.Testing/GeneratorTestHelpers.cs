using Microsoft.CodeAnalysis;

namespace RoseLynn.Testing;

/// <summary>Provides low-level helpers for manipulating compilations during generator testing.</summary>
public static class GeneratorTestHelpers
{
    /// <summary>Returns a new <seealso cref="Compilation"/> with the single source tree replaced by <paramref name="newSource"/>.</summary>
    /// <param name="sourceCompilation">The compilation whose sole syntax tree to replace.</param>
    /// <param name="newSource">The new source text to substitute.</param>
    /// <returns>A compilation with the replaced source tree.</returns>
    public static Compilation ModifySingleSource(
        Compilation sourceCompilation,
        string newSource)
    {
        var firstTree = sourceCompilation.SyntaxTrees.Single();
        return sourceCompilation.ReplaceSyntaxTree(firstTree, newSource);
    }
}
