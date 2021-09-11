using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax
{
    /// <summary>Provides extensions for <seealso cref="MemberDeclarationSyntax"/> nodes that may be generic. All extensions only apply to <seealso cref="BaseTypeDeclarationSyntax"/>, <seealso cref="DelegateDeclarationSyntax"/> and <seealso cref="MethodDeclarationSyntax"/>.</summary>
    public static class NamedMemberDeclarationSyntaxExtensions
    {
        /// <summary>Gets the identifier of the declaring member.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The identifier of the declaring member if it can be generic, otherwise <see langword="null"/>.</returns>
        public static SyntaxToken GetIdentifier(this MemberDeclarationSyntax node)
        {
            return node switch
            {
                BaseTypeDeclarationSyntax t => t.Identifier,
                DelegateDeclarationSyntax d => d.Identifier,
                MethodDeclarationSyntax   m => m.Identifier,
                _ => default,
            };
        }

        /// <summary>Creates a new <seealso cref="MemberDeclarationSyntax"/> with the specified identifier.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/> from which to create the new node.</param>
        /// <param name="identifier">The identifier of the created node.</param>
        /// <returns>The resulting node with the specified identifier.</returns>
        public static MemberDeclarationSyntax WithIdentifier(this MemberDeclarationSyntax node, SyntaxToken identifier)
        {
            return node switch
            {
                BaseTypeDeclarationSyntax t => t.WithIdentifier(identifier),
                DelegateDeclarationSyntax d => d.WithIdentifier(identifier),
                MethodDeclarationSyntax   m => m.WithIdentifier(identifier),
                _ => null,
            };
        }
    }
}
