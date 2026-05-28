using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn.Core.UnitTests;

public static class UltimateSymbolTesting
{
    public static async Task<string> ReadUltimateSymbolTestFileAsync(
        CancellationToken cancellationToken)
    {
        return await File.ReadAllTextAsync(
            "Resources/ultimate-symbol-test.cs", cancellationToken);
    }

    public static async Task<SyntaxTree> ReadUltimateSymbolTestSourceAsync(
        CancellationToken cancellationToken)
    {
        var text = await ReadUltimateSymbolTestFileAsync(cancellationToken);
        return CSharpSyntaxTree.ParseText(text, cancellationToken: cancellationToken);
    }
}
