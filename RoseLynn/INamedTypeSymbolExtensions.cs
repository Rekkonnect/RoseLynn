#nullable enable

using Microsoft.CodeAnalysis;

namespace RoseLynn
{
    /// <summary>Provides useful extensions for the <seealso cref="INamedTypeSymbol"/> type.</summary>
    public static class INamedTypeSymbolExtensions
    {
        /// <summary>Determines whether the type represented by a <seealso cref="INamedTypeSymbol"/> is an unbound generic type. This method does not throw any exceptions, in contrast to <seealso cref="INamedTypeSymbol.IsUnboundGenericType"/>.</summary>
        /// <param name="symbol">The <seealso cref="INamedTypeSymbol"/> whose type to analyze.</param>
        /// <returns><see langword="true"/> if the type represented by <paramref name="symbol"/> is an unbound generic type; otherwise <see langword="false"/>. If <paramref name="symbol"/> is <see langword="null"/>, <see langword="false"/> is also returned.</returns>
        public static bool IsUnboundGenericTypeSafe(this INamedTypeSymbol? symbol)
        {
            return IsGenericTypeSafe(symbol) && symbol!.IsUnboundGenericType;
        }
        /// <summary>Determines whether the type represented by a <seealso cref="INamedTypeSymbol"/> is a bound generic type. This method does not throw any exceptions, in contrast to <seealso cref="INamedTypeSymbol.IsUnboundGenericType"/>.</summary>
        /// <param name="symbol">The <seealso cref="INamedTypeSymbol"/> whose type to analyze.</param>
        /// <returns><see langword="true"/> if the type represented by <paramref name="symbol"/> is a bound generic type; otherwise <see langword="false"/>. If <paramref name="symbol"/> is <see langword="null"/>, <see langword="false"/> is also returned.</returns>
        public static bool IsBoundGenericTypeSafe(this INamedTypeSymbol? symbol)
        {
            return IsGenericTypeSafe(symbol) && !symbol!.IsUnboundGenericType;
        }

        private static bool IsGenericTypeSafe(INamedTypeSymbol? symbol) => symbol?.IsGenericType is true;
    }
}
