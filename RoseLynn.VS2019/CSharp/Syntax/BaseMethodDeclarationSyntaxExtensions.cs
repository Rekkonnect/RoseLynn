using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax;

/// <summary>
/// Provides useful extensions for the <seealso cref="BaseMethodDeclarationSyntax"/> type.
/// </summary>
public static class BaseMethodDeclarationSyntaxExtensions
{
    /// <summary>
    /// Determines whether the <seealso cref="BaseMethodDeclarationSyntax"/> is an operator method.
    /// </summary>
    /// <param name="baseMethodDeclaration"></param>
    /// <returns>
    /// <see langword="true"/> if the given <seealso cref="BaseMethodDeclarationSyntax"/> is of
    /// the following types:
    /// <list type="bullet">
    /// <item><seealso cref="OperatorDeclarationSyntax"/></item>
    /// <item><seealso cref="ConversionOperatorDeclarationSyntax"/></item>
    /// </list>
    /// </returns>
    public static bool IsOperatorMethod(this BaseMethodDeclarationSyntax baseMethodDeclaration)
    {
        return baseMethodDeclaration
            is OperatorDeclarationSyntax
            or ConversionOperatorDeclarationSyntax;
    }
}
