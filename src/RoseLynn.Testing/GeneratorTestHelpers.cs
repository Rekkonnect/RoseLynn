using Microsoft.CodeAnalysis;

namespace RoseLynn.Testing;

public static class GeneratorTestHelpers
{
    public static Compilation ModifySingleSource(
        Compilation sourceCompilation,
        string newSource)
    {
        var firstTree = sourceCompilation.SyntaxTrees.Single();
        return sourceCompilation.ReplaceSyntaxTree(firstTree, newSource);
    }
}
