using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RoseLynn
{
    /// <summary>Provides useful extensions for the <seealso cref="ITypeSymbol"/> type.</summary>
    public static class ITypeSymbolExtensions
    {
        /// <summary>Determines whether a <seealso cref="ITypeSymbol"/> equals a <see cref="Type"/>, by comparing their identifiers and their assemblies.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> of the type.</param>
        /// <param name="type">The <see cref="Type"/> instance of the type.</param>
        /// <returns><see langword="true"/> if the <seealso cref="ITypeSymbol"/> is found to be represented by the specified <seealso cref="Type"/>.</returns>
        /// <remarks>During the comparison of the type symbol's and the type's names, only the type's metadata name is used, which could exceed the max character limit. In such cases, invalid results might be yielded.</remarks>
        public static bool EqualsType(this ITypeSymbol symbol, Type type)
        {
            return type.FullName == symbol.MetadataName
                && type.Assembly.FullName == symbol.ContainingAssembly.MetadataName;
        }

        /// <summary>Determines whether the <seealso cref="ITypeSymbol"/> represents a valid type argument type.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type to analyze.</param>
        /// <returns><see langword="true"/> if the <seealso cref="ITypeSymbol"/> represents a valid type argument type, otherwise <see langword="false"/>.</returns>
        public static bool IsValidTypeArgument(this ITypeSymbol symbol)
        {
            return !IsInvalidTypeArgument(symbol);
        }
        /// <summary>Determines whether the <seealso cref="ITypeSymbol"/> represents an invalid type argument type.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type to analyze.</param>
        /// <returns><see langword="true"/> if the <seealso cref="ITypeSymbol"/> represents an invalid type argument type, otherwise <see langword="false"/>.</returns>
        public static bool IsInvalidTypeArgument(this ITypeSymbol symbol)
        {
            return symbol is IPointerTypeSymbol
                || symbol.SpecialType is SpecialType.System_Void;
        }

        // TODO: Test validity for all cases
        /// <summary>Determines whether a <seealso cref="ITypeSymbol"/> represents a non-null type. Nullability context is taken into account, depending on the <seealso cref="NullableAnnotation"/> result.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type to analyze.</param>
        /// <returns><see langword="true"/> if the represented type is a value type, or not annotated as a nullable reference type.</returns>
        public static bool IsNotNull(this ITypeSymbol symbol)
        {
            if (symbol.IsValueType)
                return true;

            return symbol.NullableAnnotation is NullableAnnotation.NotAnnotated;
        }

        /// <summary>Determines whether a <see cref="ITypeSymbol"/> represents a type that inherits another type.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type inheritance to analyze.</param>
        /// <param name="other">The <seealso cref="INamedTypeSymbol"/> which may be inherited by the original type.</param>
        /// <returns><see langword="true"/> if the type represented by <paramref name="symbol"/> inherits the type represented by <paramref name="other"/>, otherwise <see langword="false"/>.</returns>
        public static bool Inherits(this ITypeSymbol symbol, INamedTypeSymbol other)
        {
            if (other.TypeKind is TypeKind.Interface)
                return symbol.AllInterfaces.Contains(other);

            return symbol.GetAllBaseTypes().Contains(other, SymbolEqualityComparer.Default);
        }

        /// <summary>Gets all the base types that the type represented by the <seealso cref="ITypeSymbol"/> inherits.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type's base types to get.</param>
        /// <returns>All the base types that the type represented by the given <seealso cref="ITypeSymbol"/> inherits, with the first being the directly inherited, and the last being the deepest inherited type.</returns>
        public static IEnumerable<INamedTypeSymbol> GetAllBaseTypes(this ITypeSymbol symbol)
        {
            if (symbol is null)
                yield break;

            var currentType = symbol;
            while (true)
            {
                var baseType = currentType.BaseType;
                if (baseType is null)
                    yield break;

                yield return baseType;
                currentType = baseType;
            }
        }
        /// <summary>Gets all the base types and interfaces that the type represented by the <seealso cref="ITypeSymbol"/> inherits.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type's base types and inherited interfaces to get.</param>
        /// <returns>All the base types that the type represented by the given <seealso cref="ITypeSymbol"/> inherits, with the first being the directly inherited, each sequential type being the successively deeper inherited type, followed by the inherited interfaces from <seealso cref="ITypeSymbol.AllInterfaces"/>.</returns>
        public static IEnumerable<INamedTypeSymbol> GetAllBaseTypesAndInterfaces(this ITypeSymbol symbol)
        {
            return GetAllBaseTypes(symbol).Concat(symbol.AllInterfaces);
        }
        /// <summary>Gets all the base types and directly inherited interfaces that the type represented by the <seealso cref="ITypeSymbol"/> inherits.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type's base types and directly inherited interfaces to get.</param>
        /// <returns>All the base types that the type represented by the given <seealso cref="ITypeSymbol"/> inherits, with the first being the directly inherited, each sequential type being the successively deeper inherited type, followed by the directly inherited interfaces from <seealso cref="ITypeSymbol.Interfaces"/>.</returns>
        public static IEnumerable<INamedTypeSymbol> GetAllBaseTypesAndDirectInterfaces(this ITypeSymbol symbol)
        {
            return GetAllBaseTypes(symbol).Concat(symbol.Interfaces);
        }

        /// <summary>Gets the base type and all the interfaces that the type represented by the <seealso cref="ITypeSymbol"/> inherits.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type's base types and inherited interfaces to get.</param>
        /// <returns>The base type that the type represented by the given <seealso cref="ITypeSymbol"/> inherits, followed by the inherited interfaces from <seealso cref="ITypeSymbol.AllInterfaces"/>.</returns>
        public static IEnumerable<INamedTypeSymbol> GetBaseTypeAndInterfaces(this ITypeSymbol symbol)
        {
            return GetBaseTypeAndInterfaces(symbol, symbol.AllInterfaces);
        }
        /// <summary>Gets the base type and all the directly inherited interfaces that the type represented by the <seealso cref="ITypeSymbol"/> inherits.</summary>
        /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type's base types and directly inherited interfaces to get.</param>
        /// <returns>The base type that the type represented by the given <seealso cref="ITypeSymbol"/> inherits, followed by the directly inherited interfaces from <seealso cref="ITypeSymbol.Interfaces"/>.</returns>
        public static IEnumerable<INamedTypeSymbol> GetBaseTypeAndDirectInterfaces(this ITypeSymbol symbol)
        {
            return GetBaseTypeAndInterfaces(symbol, symbol.Interfaces);
        }

        private static IEnumerable<INamedTypeSymbol> GetBaseTypeAndInterfaces(this ITypeSymbol symbol, ImmutableArray<INamedTypeSymbol> interfaces)
        {
            var baseType = symbol.BaseType;
            if (baseType is not null)
                yield return baseType;

            foreach (var baseInterface in interfaces)
                yield return baseInterface;
        }
    }
}
