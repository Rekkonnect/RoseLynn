using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace RoseLynn.CSharp.Syntax
{
    /// <summary>Provides useful extensions for the <seealso cref="AttributeListSyntax"/> type.</summary>
    public static class AttributeListSyntaxExtensions
    {
        // No idea how this might or might not work
        // Privated for the time being until it is proven useful and valid.
        private static IEnumerable<SyntaxNode> GetAttributedNodes(this AttributeListSyntax attributeList)
        {
            var parent = attributeList.Parent;
            var nodes = parent.ChildNodes();

            bool foundAttributeList = false;
            foreach (var n in nodes)
            {
                if (!foundAttributeList)
                {
                    if (n.Equals(attributeList))
                        foundAttributeList = true;
                }
                else
                {
                    // This needs a bit of work
                    // The main concept is that multiple fields that are declared in the same line can be attributed
                    // However, this does not apply to multiple arguments or other forms of declarations with the same parent
                    if (n is not AttributeListSyntax)
                        yield return n;
                }
            }
        }
    }
}
