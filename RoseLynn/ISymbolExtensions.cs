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
            switch (symbol)
            {
                case INamedTypeSymbol t:
                    return t.Arity;
                case IMethodSymbol m:
                    return m.Arity;
            }
            return 0;
        }
        public static ImmutableArray<ITypeParameterSymbol> GetTypeParameters(this ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol t:
                    return t.TypeParameters;
                case IMethodSymbol m:
                    return m.TypeParameters;
            }
            return ImmutableArray<ITypeParameterSymbol>.Empty;
        }
        public static ImmutableArray<ITypeSymbol> GetTypeArguments(this ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol t:
                    return t.TypeArguments;
                case IMethodSymbol m:
                    return m.TypeArguments;
            }
            return ImmutableArray<ITypeSymbol>.Empty;
        }
    }
}
