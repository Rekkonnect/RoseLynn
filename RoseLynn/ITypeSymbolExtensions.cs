using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RoseLynn
{
    public static class ITypeSymbolExtensions
    {
        public static bool EqualsType(this ITypeSymbol symbol, Type type)
        {
            return type.FullName == symbol.MetadataName
                && type.Assembly.FullName == symbol.ContainingAssembly.MetadataName;
        }

        public static bool IsValidTypeArgument(this ITypeSymbol symbol)
        {
            return !IsInvalidTypeArgument(symbol);
        }
        public static bool IsInvalidTypeArgument(this ITypeSymbol symbol)
        {
            return symbol is IPointerTypeSymbol
                || symbol.SpecialType is SpecialType.System_Void;
        }

        public static bool IsNotNull(this ITypeSymbol symbol)
        {
            if (symbol.IsValueType)
                return true;

            return symbol.NullableAnnotation is NullableAnnotation.Annotated;
        }

        public static bool Inherits(this ITypeSymbol symbol, INamedTypeSymbol other)
        {
            if (other.TypeKind is TypeKind.Interface)
                return symbol.AllInterfaces.Contains(other);

            return symbol.GetAllBaseTypes().Contains(other, SymbolEqualityComparer.Default);
        }

        public static IEnumerable<INamedTypeSymbol> GetAllBaseTypes(this ITypeSymbol symbol)
        {
            var currentType = symbol;
            while (currentType != null)
            {
                var baseType = currentType.BaseType;
                if (baseType != null)
                    yield return baseType;
                currentType = baseType;
            }
        }
        public static IEnumerable<INamedTypeSymbol> GetAllBaseTypesAndInterfaces(this ITypeSymbol symbol)
        {
            return GetAllBaseTypes(symbol).Concat(symbol.AllInterfaces);
        }
        public static IEnumerable<INamedTypeSymbol> GetAllBaseTypesAndDirectInterfaces(this ITypeSymbol symbol)
        {
            return GetAllBaseTypes(symbol).Concat(symbol.Interfaces);
        }
        public static IEnumerable<INamedTypeSymbol> GetBaseTypeAndInterfaces(this ITypeSymbol symbol)
        {
            return GetBaseTypeAndInterfaces(symbol, symbol.AllInterfaces);
        }
        public static IEnumerable<INamedTypeSymbol> GetBaseTypeAndDirectInterfaces(this ITypeSymbol symbol)
        {
            return GetBaseTypeAndInterfaces(symbol, symbol.Interfaces);
        }

        private static IEnumerable<INamedTypeSymbol> GetBaseTypeAndInterfaces(this ITypeSymbol symbol, ImmutableArray<INamedTypeSymbol> interfaces)
        {
            var baseType = symbol.BaseType;
            if (baseType != null)
                yield return baseType;

            foreach (var baseInterface in interfaces)
                yield return baseInterface;
        }
    }
}
