#nullable enable

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace RoseLynn;

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
    /// The result is provided from the cache in <seealso cref="CachedInfrequentSpecialSymbols.Instance"/>.
    /// The Roslyn API does not currently offer any direct retrieval mechanism, and is unlikely to change in the future.
    /// </remarks>
    public static IMethodSymbol? GetDestructor(this INamedTypeSymbol typeSymbol)
    {
        return CachedInfrequentSpecialSymbols.Instance[typeSymbol].Destructor;
    }
    /// <summary>Gets the <seealso cref="IMethodSymbol"/> instances representing the extension methods contained in a <seealso cref="INamedTypeSymbol"/>.</summary>
    /// <param name="typeSymbol">The <seealso cref="INamedTypeSymbol"/> whose extension methods to get.</param>
    /// <returns>An array of <seealso cref="IMethodSymbol"/> instances representing the extension methods of the type.</returns>
    /// <remarks>
    /// The result is provided from the cache in <seealso cref="CachedInfrequentSpecialSymbols.Instance"/>.
    /// The Roslyn API does not currently offer any direct retrieval mechanism, and is unlikely to change in the future.
    /// </remarks>
    public static ImmutableArray<IMethodSymbol> GetExtensionMethods(this INamedTypeSymbol typeSymbol)
    {
        return CachedInfrequentSpecialSymbols.Instance[typeSymbol].ExtensionMethods;
    }
    /// <summary>Gets the <seealso cref="IFieldSymbol"/> instances representing the constant fields contained in a <seealso cref="INamedTypeSymbol"/>.</summary>
    /// <param name="typeSymbol">The <seealso cref="INamedTypeSymbol"/> whose constant fields to get.</param>
    /// <returns>An array of <seealso cref="IFieldSymbol"/> instances representing the constant fields of the type.</returns>
    /// <remarks>
    /// Enum members also count as constant fields.
    /// The result is provided from the cache in <seealso cref="CachedInfrequentSpecialSymbols.Instance"/>.
    /// The Roslyn API does not currently offer any direct retrieval mechanism, and is unlikely to change in the future.
    /// </remarks>
    public static ImmutableArray<IFieldSymbol> GetConstantFields(this INamedTypeSymbol typeSymbol)
    {
        return CachedInfrequentSpecialSymbols.Instance[typeSymbol].ConstantFields;
    }

    /// <summary>Gets the definition <seealso cref="IFieldSymbol"/> of the given enum <seealso cref="INamedTypeSymbol"/>.</summary>
    /// <param name="enumSymbol">The <seealso cref="INamedTypeSymbol"/> instance representing the enum whose defined value fields to get.</param>
    /// <returns>An <seealso cref="ImmutableArray{T}"/> containing the <seealso cref="IFieldSymbol"/> instances representing the defined values of the given enum.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="enumSymbol"/> does not represent an enum.</exception>
    public static ImmutableArray<IFieldSymbol> GetEnumDefinitions(this INamedTypeSymbol enumSymbol)
    {
        if (enumSymbol?.TypeKind is not TypeKind.Enum)
            throw new ArgumentException("The given symbol must represent an enum.");

        return enumSymbol.GetMembers<IFieldSymbol>().ToImmutableArray();
    }
}
