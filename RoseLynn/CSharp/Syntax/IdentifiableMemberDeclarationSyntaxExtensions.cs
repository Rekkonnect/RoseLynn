using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax
{
    /// <summary>Provides extensions for <seealso cref="MemberDeclarationSyntax"/> nodes that may have an identifier. All extensions only apply to:
    /// <list type="bullet">
    /// <item><seealso cref="BaseTypeDeclarationSyntax"/></item>
    /// <item><seealso cref="DelegateDeclarationSyntax"/></item>
    /// <item><seealso cref="MethodDeclarationSyntax"/></item>
    /// <item><seealso cref="ConstructorDeclarationSyntax"/></item>
    /// <item><seealso cref="PropertyDeclarationSyntax"/></item>
    /// <item><seealso cref="EventDeclarationSyntax"/></item>
    /// </list>
    /// </summary>
    /// <remarks>For <seealso cref="FieldDeclarationSyntax"/> nodes, use the dedicated extensions.</remarks>
    public static class IdentifiableMemberDeclarationSyntaxExtensions
    {
        /// <remarks>See <seealso cref="IdentifiableMemberDeclarationSyntaxExtensions"/> for the supported types.</remarks>
        private static void SupportedTypeRemarks() { }

        /// <summary>Gets the identifier of the declaring member.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The identifier of the declaring member if it has an identifier, otherwise <see langword="default"/>.</returns>
        /// <inheritdoc cref="SupportedTypeRemarks"/>
        public static SyntaxToken GetIdentifier(this MemberDeclarationSyntax node)
        {
            return node switch
            {
                BaseTypeDeclarationSyntax    t => t.Identifier,
                DelegateDeclarationSyntax    d => d.Identifier,
                MethodDeclarationSyntax      m => m.Identifier,
                ConstructorDeclarationSyntax c => c.Identifier,
                PropertyDeclarationSyntax    p => p.Identifier,
                EventDeclarationSyntax       e => e.Identifier,
                _ => default,
            };
        }

        /// <summary>Creates a new <seealso cref="MemberDeclarationSyntax"/> with the specified identifier.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/> from which to create the new node.</param>
        /// <param name="identifier">The identifier of the created node.</param>
        /// <returns>The resulting node with the specified identifier, if it can have one, otherwise <see langword="null"/>.</returns>
        /// <inheritdoc cref="SupportedTypeRemarks"/>
        public static MemberDeclarationSyntax WithIdentifier(this MemberDeclarationSyntax node, SyntaxToken identifier)
        {
            return node switch
            {
                BaseTypeDeclarationSyntax    t => t.WithIdentifier(identifier),
                DelegateDeclarationSyntax    d => d.WithIdentifier(identifier),
                MethodDeclarationSyntax      m => m.WithIdentifier(identifier),
                ConstructorDeclarationSyntax c => c.WithIdentifier(identifier),
                PropertyDeclarationSyntax    p => p.WithIdentifier(identifier),
                EventDeclarationSyntax       e => e.WithIdentifier(identifier),
                _ => null,
            };
        }
    }
}
