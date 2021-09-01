using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn
{
    /// <summary>Provides useful extensions for <seealso cref="ISymbol"/> instances that can be generic.</summary>
    public static class TypeParameterizableSymbolExtensions
    {
        // This is in dire need of some better abstraction
        // Say something like IGenericSupportSymbol

        /// <summary>Gets the arity of the <seealso cref="ISymbol"/>.</summary>
        /// <param name="symbol">The symbol whose arity to get.</param>
        /// <returns>The arity of the symbol, if generic, otherwise 0.</returns>
        public static int GetArity(this ISymbol symbol)
        {
            return symbol switch
            {
                INamedTypeSymbol t => t.Arity,
                IMethodSymbol    m => m.Arity,
                _ => 0,
            };
        }
        /// <summary>Gets the type parameters of the <seealso cref="ISymbol"/>.</summary>
        /// <param name="symbol">The symbol whose type parameters to get.</param>
        /// <returns>The type parameters of the symbol, if generic, otherwise <seealso cref="ImmutableArray{T}.Empty"/>.</returns>
        public static ImmutableArray<ITypeParameterSymbol> GetTypeParameters(this ISymbol symbol)
        {
            return symbol switch
            {
                INamedTypeSymbol t => t.TypeParameters,
                IMethodSymbol    m => m.TypeParameters,
                _ => ImmutableArray<ITypeParameterSymbol>.Empty,
            };
        }
        /// <summary>Gets the type arguments of the <seealso cref="ISymbol"/>.</summary>
        /// <param name="symbol">The symbol whose type arguments to get.</param>
        /// <returns>The type arguments of the symbol, if generic, otherwise <seealso cref="ImmutableArray{T}.Empty"/>.</returns>
        public static ImmutableArray<ITypeSymbol> GetTypeArguments(this ISymbol symbol)
        {
            return symbol switch
            {
                INamedTypeSymbol t => t.TypeArguments,
                IMethodSymbol    m => m.TypeArguments,
                _ => ImmutableArray<ITypeSymbol>.Empty,
            };
        }
    }
}
