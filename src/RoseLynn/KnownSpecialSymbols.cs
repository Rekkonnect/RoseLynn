using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Provides methods for identifying well-known special symbols in a Roslyn compilation.</summary>
public static class KnownSpecialSymbols
{
    /// <summary>Returns <see langword="true"/> if <paramref name="method"/> is <c>Type.GetTypeCode</c>.</summary>
    /// <param name="method">The method symbol to test.</param>
    /// <returns><see langword="true"/> when the method matches <c>System.Type.GetTypeCode</c>.</returns>
    public static bool IsTypeGetTypeCode(IMethodSymbol method)
    {
        var containingType = method.ContainingType;
        return KnownSymbolNameMatches.SystemType.Matches(containingType)
            && method is
            {
                Name: nameof(Type.GetTypeCode)
            };
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="type"/> is an array or any form of <c>IEnumerable</c>.</summary>
    /// <param name="type">The type symbol to test.</param>
    /// <returns><see langword="true"/> for arrays and types implementing <c>IEnumerable</c>.</returns>
    public static bool IsNonScalarType(ITypeSymbol? type)
    {
        return type is IArrayTypeSymbol
            || IsIEnumerable(type)
            || (type?.AllInterfaces.Any(IsIEnumerable) is true)
            ;
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="type"/> is <c>IEnumerable</c> or <c>IEnumerable&lt;T&gt;</c>.</summary>
    /// <param name="type">The type symbol to test.</param>
    /// <returns><see langword="true"/> for the non-generic and generic <c>IEnumerable</c> special types.</returns>
    public static bool IsIEnumerable(ITypeSymbol? type)
    {
        return type is
        {
            SpecialType: SpecialType.System_Collections_IEnumerable
                or SpecialType.System_Collections_Generic_IEnumerable_T
        };
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="method"/> is or overrides <c>object.ToString</c>.</summary>
    /// <param name="method">The method symbol to test.</param>
    /// <returns><see langword="true"/> for <c>object.ToString</c> or any override of it.</returns>
    public static bool IsToString(IMethodSymbol method)
    {
        return method
            is
        {
            ContainingType.SpecialType: SpecialType.System_Object,
            Name: nameof(object.ToString)
        }
            || (method.OverriddenMethod is not null and var overridden
                && IsToString(overridden))
            ;
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="method"/> is or overrides <c>object.GetHashCode</c>.</summary>
    /// <param name="method">The method symbol to test.</param>
    /// <returns><see langword="true"/> for <c>object.GetHashCode</c> or any override of it.</returns>
    public static bool IsGetHashCode(IMethodSymbol method)
    {
        return method
            is
        {
            ContainingType.SpecialType: SpecialType.System_Object,
            Name: nameof(object.GetHashCode)
        }
            || (method.OverriddenMethod is not null and var overridden
                && IsGetHashCode(overridden))
            ;
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="method"/> is or overrides <c>object.Equals</c>.</summary>
    /// <param name="method">The method symbol to test.</param>
    /// <returns><see langword="true"/> for <c>object.Equals</c> or any override of it.</returns>
    public static bool IsEquals(IMethodSymbol method)
    {
        return method
            is
        {
            ContainingType.SpecialType: SpecialType.System_Object,
            Name: nameof(object.Equals)
        }
            || (method.OverriddenMethod is not null and var overridden
                && IsEquals(overridden))
            ;
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="method"/> is <c>object.GetType</c>.</summary>
    /// <param name="method">The method symbol to test.</param>
    /// <returns><see langword="true"/> only for the non-virtual <c>object.GetType</c> declaration.</returns>
    public static bool IsGetType(IMethodSymbol method)
    {
        return method
            is
        {
            ContainingType.SpecialType: SpecialType.System_Object,
            Name: nameof(object.GetType)
        };
    }

    /// <summary>Returns <see langword="true"/> if <paramref name="method"/> is or overrides any of <c>ToString</c>, <c>GetType</c>, <c>Equals</c>, or <c>GetHashCode</c>.</summary>
    /// <param name="method">The method symbol to test.</param>
    /// <returns><see langword="true"/> if the method is a base <see cref="object"/> instance method or an override.</returns>
    public static bool IsBaseObjectInstanceMethod(IMethodSymbol method)
    {
        return IsToString(method)
            || IsGetType(method)
            || IsEquals(method)
            || IsGetHashCode(method)
            ;
    }
}
