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
            return kind
                is TypeKind.Class
                or TypeKind.Struct
                or TypeKind.Interface;
        }

        /// <summary>Gets the respective <seealso cref="IdentifiableSymbolKind"/> flag for a given <seealso cref="TypeKind"/>.</summary>
        /// <param name="kind">The <seealso cref="TypeKind"/> whose respective <seealso cref="IdentifiableSymbolKind"/> flag to get.</param>
        /// <returns>The respective flag in <seealso cref="IdentifiableSymbolKind"/> for the given <seealso cref="TypeKind"/>.</returns>
        public static IdentifiableSymbolKind GetIdentifiableSymbolKindFlag(this TypeKind kind)
        {
            return kind switch
            {
                TypeKind.Class         => IdentifiableSymbolKind.Class,
                TypeKind.Struct        => IdentifiableSymbolKind.Struct,
                TypeKind.Interface     => IdentifiableSymbolKind.Interface,
                TypeKind.Enum          => IdentifiableSymbolKind.Enum,
                TypeKind.Delegate      => IdentifiableSymbolKind.Delegate,
                TypeKind.TypeParameter => IdentifiableSymbolKind.GenericParameter,

                _ => IdentifiableSymbolKind.None,
            };
        }
    }
}
