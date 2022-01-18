#nullable enable

using Microsoft.CodeAnalysis;
using System.Linq;

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

        /// <summary>Determines whether the given <seealso cref="INamedTypeSymbol"/> has a public parameterless instance constructor.</summary>
        /// <param name="typeSymbol">The <seealso cref="INamedTypeSymbol"/> whose instance constructors to analyze.</param>
        /// <returns><see langword="true"/> if the given <seealso cref="INamedTypeSymbol"/> contains a public parameterless instance constructor, otherwise <see langword="false"/>.</returns>
        public static bool HasPublicParameterlessInstanceConstructor(this INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.InstanceConstructors.Any(IMethodSymbolExtensions.IsPublicParameterlessMethod);
        }

        /// <summary>Gets the destructor <seealso cref="IMethodSymbol"/> of a <seealso cref="INamedTypeSymbol"/>.</summary>
        /// <param name="typeSymbol">The <seealso cref="INamedTypeSymbol"/> whose destructor to get.</param>
        /// <returns>The <seealso cref="IMethodSymbol"/> representing the destructor of the type, if contained, otherwise <see langword="null"/>.</returns>
        /// <remarks>
        /// This method iterates through all the containing members of the <seealso cref="INamedTypeSymbol"/> and
        /// finds the first that is an <seealso cref="IMethodSymbol"/> of kind <seealso cref="MethodKind.Destructor"/>.
        /// The API does not currently offer any direct retrieval mechanism, and is unlikely to change in the future.
        /// </remarks>
        public static IMethodSymbol? GetDestructor(this INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(member => member is { MethodKind: MethodKind.Destructor });
        }
    }
}
