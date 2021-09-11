using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax
{
    /// <summary>Provides useful extensions for certain syntax nodes.</summary>
    public static class SyntaxNodeParentRetrievalExtensions
    {
        /// <summary>Gets the parents of an <seealso cref="AttributeArgumentSyntax"/>.</summary>
        /// <param name="argument">The <seealso cref="AttributeArgumentSyntax"/> whose parents to get.</param>
        /// <param name="argumentList">The parent <seealso cref="AttributeArgumentListSyntax"/> of <paramref name="argument"/>.</param>
        /// <param name="attribute">The parent <seealso cref="AttributeSyntax"/> of <paramref name="argumentList"/>.</param>
        /// <param name="attributeList">The parent <seealso cref="AttributeListSyntax"/> of <paramref name="attribute"/>.</param>
        public static void GetAttributeRelatedParents(this AttributeArgumentSyntax argument, out AttributeArgumentListSyntax argumentList, out AttributeSyntax attribute, out AttributeListSyntax attributeList)
        {
            argumentList = argument.GetParentAttributeArgumentList();
            attribute = argumentList.GetParentAttribute();
            attributeList = attribute.GetParentAttributeList();
        }

        /// <summary>Gets the parent <seealso cref="AttributeArgumentListSyntax"/> of an <seealso cref="AttributeArgumentSyntax"/></summary>
        /// <param name="argument">The <seealso cref="AttributeArgumentSyntax"/> whose parent to get.</param>
        /// <returns>The parent <seealso cref="AttributeArgumentListSyntax"/> of <paramref name="argument"/>.</returns>
        public static AttributeArgumentListSyntax GetParentAttributeArgumentList(this AttributeArgumentSyntax argument)
        {
            return argument.Parent as AttributeArgumentListSyntax;
        }
        /// <summary>Gets the parent <seealso cref="AttributeSyntax"/> of an <seealso cref="AttributeArgumentListSyntax"/></summary>
        /// <param name="argumentList">The <seealso cref="AttributeArgumentListSyntax"/> whose parent to get.</param>
        /// <returns>The parent <seealso cref="AttributeSyntax"/> of <paramref name="argumentList"/>.</returns>
        public static AttributeSyntax GetParentAttribute(this AttributeArgumentListSyntax argumentList)
        {
            return argumentList.Parent as AttributeSyntax;
        }
        /// <summary>Gets the parent <seealso cref="AttributeListSyntax"/> of an <seealso cref="AttributeSyntax"/></summary>
        /// <param name="attribute">The <seealso cref="AttributeSyntax"/> whose parent to get.</param>
        /// <returns>The parent <seealso cref="AttributeListSyntax"/> of <paramref name="attribute"/>.</returns>
        public static AttributeListSyntax GetParentAttributeList(this AttributeSyntax attribute)
        {
            return attribute.Parent as AttributeListSyntax;
        }
    }
}
