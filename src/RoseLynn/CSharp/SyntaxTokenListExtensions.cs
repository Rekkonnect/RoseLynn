using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn.CSharp;

/// <summary>
/// Provides useful extensions for the <seealso cref="SyntaxTokenList"/> type.
/// </summary>
public static class SyntaxTokenListExtensions
{
    /// <summary>
    /// Determines whether a <seealso cref="SyntaxTokenList"/> contians a
    /// <seealso cref="SyntaxToken"/> of the specified <seealso cref="SyntaxKind"/>.
    /// </summary>
    /// <param name="syntaxTokens">The <seealso cref="SyntaxTokenList"/> to enumerate.</param>
    /// <param name="syntaxKind">The <seealso cref="SyntaxKind"/> that is searched for.</param>
    /// <returns>
    /// <see langword="true"/> if there is at least one <seealso cref="SyntaxToken"/>
    /// with the specified <seealso cref="SyntaxKind"/>, otherwise <see langword="false"/>.
    /// </returns>
    public static bool HasKind(this SyntaxTokenList syntaxTokens, SyntaxKind syntaxKind)
    {
        foreach (var token in syntaxTokens)
        {
            if (token.IsKind(syntaxKind))
                return true;
        }
        return false;
    }
}
