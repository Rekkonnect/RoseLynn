using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn
{
    public static class ISymbolExtensions
    {
        // This is in dire need of some better abstraction
        // Say something like IGenericSupportSymbol
        public static int GetArity(this ISymbol symbol)
        {
            return symbol switch
            {
                INamedTypeSymbol t => t.Arity,
                IMethodSymbol m => m.Arity,
                _ => 0,
            };
        }
        public static ImmutableArray<ITypeParameterSymbol> GetTypeParameters(this ISymbol symbol)
        {
            return symbol switch
            {
                INamedTypeSymbol t => t.TypeParameters,
                IMethodSymbol m => m.TypeParameters,
                _ => ImmutableArray<ITypeParameterSymbol>.Empty,
            };
        }
        public static ImmutableArray<ITypeSymbol> GetTypeArguments(this ISymbol symbol)
        {
            return symbol switch
            {
                INamedTypeSymbol t => t.TypeArguments,
                IMethodSymbol m => m.TypeArguments,
                _ => ImmutableArray<ITypeSymbol>.Empty,
            };
        }
    }
}
