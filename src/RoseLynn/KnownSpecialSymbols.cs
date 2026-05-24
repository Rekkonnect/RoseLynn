using Microsoft.CodeAnalysis;

namespace RoseLynn;

public static class KnownSpecialSymbols
{
    public static bool IsTypeGetTypeCode(IMethodSymbol method)
    {
        var containingType = method.ContainingType;
        return KnownSymbolNameMatches.SystemType.Matches(containingType)
            && method is
            {
                Name: nameof(Type.GetTypeCode)
            };
    }

    public static bool IsNonScalarType(ITypeSymbol? type)
    {
        return type is IArrayTypeSymbol
            || IsIEnumerable(type)
            || (type?.AllInterfaces.Any(IsIEnumerable) is true)
            ;
    }

    public static bool IsIEnumerable(ITypeSymbol? type)
    {
        return type is
        {
            SpecialType: SpecialType.System_Collections_IEnumerable
                or SpecialType.System_Collections_Generic_IEnumerable_T
        };
    }

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

    public static bool IsGetType(IMethodSymbol method)
    {
        return method
            is
        {
            ContainingType.SpecialType: SpecialType.System_Object,
            Name: nameof(object.GetType)
        };
    }

    public static bool IsBaseObjectInstanceMethod(IMethodSymbol method)
    {
        return IsToString(method)
            || IsGetType(method)
            || IsEquals(method)
            || IsGetHashCode(method)
            ;
    }
}
