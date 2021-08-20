using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax
{
    public static class AttributeSyntaxExtensions
    {
        public static string GetAttributeIdentifierString(this AttributeSyntax attribute) => (attribute.Name as IdentifierNameSyntax)?.Identifier.ValueText;
    }
}
