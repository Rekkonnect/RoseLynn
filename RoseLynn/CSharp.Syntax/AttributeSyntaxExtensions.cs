using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax
{
    public static class AttributeSyntaxExtensions
    {
        /// <summary>Gets the identifier string of the <seealso cref="AttributeSyntax"/>.</summary>
        /// <param name="attribute">The <seealso cref="AttributeSyntax"/> whose identifier string to get.</param>
        /// <returns>The text value of the string identifier of the <seealso cref="AttributeSyntax"/>.</returns>
        public static string GetAttributeIdentifierString(this AttributeSyntax attribute) => (attribute.Name as IdentifierNameSyntax)?.Identifier.ValueText;
    }
}
