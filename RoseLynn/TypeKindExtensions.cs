using Microsoft.CodeAnalysis;

namespace RoseLynn
{
    /// <summary>Provides useful extensions for the <seealso cref="TypeKind"/> type.</summary>
    public static class TypeKindExtensions
    {
        /// <summary>Determines whether the given <seealso cref="TypeKind"/> can explicitly inherit types.</summary>
        /// <param name="kind">The <seealso cref="TypeKind"/> that may be able to explicitly inherit types.</param>
        /// <returns><see langword="true"/> if the <seealso cref="TypeKind"/> can explicitly inherit types during its definition, meaning it's <seealso cref="TypeKind.Class"/>, <seealso cref="TypeKind.Struct"/>, or <seealso cref="TypeKind.Interface"/>; otherwise <see langword="false"/>.</returns>
        public static bool CanExplicitlyInheritTypes(this TypeKind kind)
        {
            return kind is TypeKind.Class
                        or TypeKind.Struct
                        or TypeKind.Interface;
        }
    }
}
