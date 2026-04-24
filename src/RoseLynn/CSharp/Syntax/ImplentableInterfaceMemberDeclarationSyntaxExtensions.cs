using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax;

#nullable enable

/// <summary>
/// Provides extensions for <seealso cref="MemberDeclarationSyntax"/> nodes representing
/// implementable members that can be declared in an interface, including:
/// <list type="bullet">
/// <item><seealso cref="BasePropertyDeclarationSyntax"/></item>
/// <item><seealso cref="MethodDeclarationSyntax"/></item>
/// </list>
/// </summary>
public static class ImplentableInterfaceMemberDeclarationSyntaxExtensions
{
    /// <summary>
    /// Gets the explicit interface specifier of the declared member.
    /// </summary>
    /// <param name="node">
    /// The <seealso cref="MemberDeclarationSyntax"/> representing an invokable member.
    /// </param>
    /// <returns>
    /// The <seealso cref="ExplicitInterfaceSpecifierSyntax"/> of the declared member,
    /// or <see langword="null"/> if the <seealso cref="MemberDeclarationSyntax"/>
    /// does not reflect the declaration of a member that can be declared in an interface.
    /// </returns>
    public static ExplicitInterfaceSpecifierSyntax? GetExplicitInterfaceSpecifier(this MemberDeclarationSyntax node)
    {
        return node switch
        {
            BasePropertyDeclarationSyntax p => p.ExplicitInterfaceSpecifier,
            MethodDeclarationSyntax m       => m.ExplicitInterfaceSpecifier,
            _ => null,
        };
    }
}
