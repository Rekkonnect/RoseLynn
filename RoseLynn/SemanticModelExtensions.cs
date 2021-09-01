using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn
{
    /// <summary>Provides useful extensions for the <seealso cref="SemanticModel"/> type.</summary>
    public static class SemanticModelExtensions
    {
        /// <summary>Gets the associated <seealso cref="TypeInfo"/> for the type in a <seealso cref="BaseTypeSyntax"/>.</summary>
        /// <param name="semanticModel">The <seealso cref="SemanticModel"/> from which to get the information about the type in the <seealso cref="BaseTypeSyntax"/>.</param>
        /// <param name="baseType">The <seealso cref="BaseTypeSyntax"/> whose type info to get from the semantic model.</param>
        /// <returns>The <seealso cref="TypeInfo"/> regarding the type in the <seealso cref="BaseTypeSyntax"/>.</returns>
        public static TypeInfo GetTypeInfo(this SemanticModel semanticModel, BaseTypeSyntax baseType) => semanticModel.GetTypeInfo(baseType.Type);
    }
}
