using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax;

#nullable enable

/// <summary>Contains extensions for the <seealso cref="MemberDeclarationSyntax"/> class.</summary>
public static class MemberDeclarationSyntaxExtensions
{
    /// <summary>Gets a <seealso cref="SyntaxNodeOrToken"/> representing the name of the declared member.</summary>
    /// <param name="declarationSyntax">The <seealso cref="MemberDeclarationSyntax"/> whose <seealso cref="SyntaxNodeOrToken"/> denoting the name to get.</param>
    /// <returns>The <seealso cref="SyntaxNodeOrToken"/> representing either the <seealso cref="SyntaxToken"/> for the identifier, or the <see cref="NameSyntax"/> that denotes the declared member's name.</returns>
    public static SyntaxNodeOrToken GetIdentifierTokenOrNameSyntax(this MemberDeclarationSyntax declarationSyntax)
    {
        return declarationSyntax switch
        {
            // VS2019 does not support namespace declarations
            BaseNamespaceDeclarationSyntax namespaceDeclaration => namespaceDeclaration.Name,

            _ => VS2019.MemberDeclarationSyntaxExtensions.GetIdentifierTokenOrNameSyntax(declarationSyntax),
        };
    }
}
