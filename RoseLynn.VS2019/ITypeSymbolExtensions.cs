using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace RoseLynn;

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
        return type.FullName == symbol.GetFullSymbolName()!.FullNameString
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
    /// <summary>Gets the base type and all the directly inherited interfaces that are inherited by the type represented by the <seealso cref="ITypeSymbol"/>.</summary>
    /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose type's base types and directly inherited interfaces to get.</param>
    /// <returns>The base type that the type represented by the given <seealso cref="ITypeSymbol"/> inherits, followed by the directly inherited interfaces from <seealso cref="ITypeSymbol.Interfaces"/>.</returns>
    public static IEnumerable<INamedTypeSymbol> GetBaseTypeAndDirectInterfaces(this ITypeSymbol symbol)
    {
        return GetBaseTypeAndInterfaces(symbol, symbol.Interfaces);
    }

    /// <summary>Gets all the inherited members of the given <seealso cref="ITypeSymbol"/>.</summary>
    /// <param name="typeSymbol">The <seealso cref="ITypeSymbol"/> whose inherited members to get.</param>
    /// <returns>All the members that are inherited from the base types of the given <seealso cref="ITypeSymbol"/>. Members defined in the type itself are not included.</returns>
    /// <remarks>Consider <seealso cref="GetAllMembersIncludingInherited(ITypeSymbol)"/> if you want to include defined members in the requested type.</remarks>
    public static IEnumerable<ISymbol> GetAllInheritedMembers(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetAllBaseTypesAndInterfaces().SelectMany(type => type.GetMembers());
    }
    /// <summary>Gets all defined members, including inherited ones, of the given <seealso cref="ITypeSymbol"/>.</summary>
    /// <param name="typeSymbol">The <seealso cref="ITypeSymbol"/> whose inherited and defined members to get.</param>
    /// <returns>All the members that are inherited from the base types of the given <seealso cref="ITypeSymbol"/>, including members defined in the type itself.</returns>
    /// <remarks>Consider <seealso cref="GetAllInheritedMembers(ITypeSymbol)"/> if you want to exclude defined members in the requested type.</remarks>
    public static IEnumerable<ISymbol> GetAllMembersIncludingInherited(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().Concat(typeSymbol.GetAllInheritedMembers());
    }
    /// <summary>Gets all the interface-inherited members of the given <seealso cref="ITypeSymbol"/>.</summary>
    /// <param name="typeSymbol">The <seealso cref="ITypeSymbol"/> whose members inherited from interfaces to get.</param>
    /// <returns>All the members that are inherited from the interfaces of the given <seealso cref="ITypeSymbol"/>. Members defined in the type itself and base classes are not included.</returns>
    public static IEnumerable<ISymbol> GetAllInheritedInterfaceMembers(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.AllInterfaces.SelectMany(type => type.GetMembers());
    }

    private static IEnumerable<INamedTypeSymbol> GetBaseTypeAndInterfaces(this ITypeSymbol symbol, ImmutableArray<INamedTypeSymbol> interfaces)
    {
        var baseType = symbol.BaseType;
        if (baseType is not null)
            yield return baseType;

        foreach (var baseInterface in interfaces)
            yield return baseInterface;
    }

    /// <summary>
    /// Gets a deep 
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <param name="qualifiers"></param>
    /// <returns></returns>
    public static ISymbol? GetQualifiedMember(this ITypeSymbol typeSymbol, string[] qualifiers)
    {
        ISymbol? currentMember = null;
        var currentType = typeSymbol;
        for (int i = 0; i < qualifiers.Length; i++)
        {
            var members = currentType.GetMembers();
            var fieldsProperties = members.Where(IsFieldOrProperty);
            var targetMember = fieldsProperties.FirstOrDefault(m => m.Name == qualifiers[i]);
            if (targetMember is null)
            {
                break;
            }

            currentType = targetMember.GetSymbolType();
            currentMember = targetMember;
        }

        return currentMember;
    }
}
