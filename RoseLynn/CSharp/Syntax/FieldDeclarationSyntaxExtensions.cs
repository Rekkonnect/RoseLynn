using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax
{
    /// <summary>Provides useful extensions for the <seealso cref="FieldDeclarationSyntax"/> type.</summary>
    public static class FieldDeclarationSyntaxExtensions
    {
        /// <summary>Gets the identifier of the declaring field.</summary>
        /// <param name="fieldDeclaration">The <seealso cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="declaredFieldIndex">The index of the field that is being declared in the field declaration node.</param>
        /// <returns>The identifier of the declaring member, otherwise <see langword="default"/>.</returns>
        public static SyntaxToken GetIdentifier(this FieldDeclarationSyntax fieldDeclaration, int declaredFieldIndex = 0)
        {
            return fieldDeclaration.Declaration.Variables[declaredFieldIndex].Identifier;
        }

        /// <summary>Gets the identifier of the declaring field.</summary>
        /// <param name="fieldDeclaration">The <seealso cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="identifier">The new identifier of the declaring field.</param>
        /// <param name="declaredFieldIndex">The index of the field that is being declared in the field declaration node.</param>
        /// <returns>The new <seealso cref="FieldDeclarationSyntax"/> with the declared field at the specified index having the new identifier.</returns>
        public static FieldDeclarationSyntax WithIdentifier(this FieldDeclarationSyntax fieldDeclaration, SyntaxToken identifier, int declaredFieldIndex = 0)
        {
            var result = fieldDeclaration.Declaration.Variables[declaredFieldIndex].WithIdentifier(identifier);
            return result.GetNearestParentOfType<FieldDeclarationSyntax>();
        }
    }
}
