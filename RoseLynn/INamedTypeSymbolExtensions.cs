using Microsoft.CodeAnalysis;

namespace RoseLynn
{
    public static class INamedTypeSymbolExtensions
    {
        public static bool IsUnboundGenericTypeSafe(this INamedTypeSymbol symbol)
        {
            return symbol.IsGenericType && symbol.IsUnboundGenericType;
        }
        public static bool IsBoundGenericTypeSafe(this INamedTypeSymbol symbol)
        {
            return symbol.IsGenericType && !symbol.IsUnboundGenericType;
        }
    }
}
