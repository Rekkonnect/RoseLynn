
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn;

/// <summary>Contains extension methods for the <seealso cref="TypedConstant"/> type.</summary>
public static class TypedConstantExtensions
{
    /// <summary>Determines whether the given <seealso cref="TypedConstant"/> value represents a defined enum value.</summary>
    /// <param name="constant">The <seealso cref="TypedConstant"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the given <seealso cref="TypedConstant"/> represents an enum constant, its value is not <see langword="null"/>,
    /// and matches one of the defined values of the enum type; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsDefinedEnumValue(this TypedConstant constant)
    {
        var enumSymbol = constant.Type as INamedTypeSymbol;
        if (enumSymbol?.TypeKind is not TypeKind.Enum)
            return false;

        if (constant.Value is null)
            return false;

        var definedEnumValues = enumSymbol.GetEnumDefinitions();
        foreach (var value in definedEnumValues)
        {
            if (value.ConstantValue!.Equals(constant.Value))
                return true;
        }
        return false;
    }

    public static T? ValueOrDefault<T>(
        this ImmutableArray<TypedConstant> array,
        int index,
        T? @default = default)
    {
        var constant = array.ElementAtOrDefault(index);
        if (constant.Kind is TypedConstantKind.Array)
        {
            return @default;
        }

        var value = constant.Value;
        if (value is T t)
        {
            return t;
        }

        return @default;
    }

    public static bool TryGetValue<T>(
        this ImmutableArray<TypedConstant> array,
        int index,
        out T? value)
    {
        var constant = array.ElementAtOrDefault(index);
        if (constant.Kind is TypedConstantKind.Array)
        {
            value = default;
            return false;
        }

        var constantValue = constant.Value;
        if (constantValue is T t)
        {
            value = t;
            return true;
        }

        value = default;
        return false;
    }

    public static bool TryGetValues(
        this ImmutableArray<TypedConstant> array,
        int index,
        out ImmutableArray<TypedConstant> values)
    {
        var constant = array.ElementAtOrDefault(index);
        if (constant.Kind is TypedConstantKind.Array)
        {
            values = constant.Values;
            return true;
        }

        values = [];
        return false;
    }
}
