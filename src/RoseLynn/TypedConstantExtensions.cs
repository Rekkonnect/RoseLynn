
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn.Core;

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

    /// <summary>Returns the value at <paramref name="index"/> in <paramref name="array"/> cast to <typeparamref name="T"/>, or <paramref name="default"/> if missing, array-kind, or not castable.</summary>
    /// <typeparam name="T">The expected value type.</typeparam>
    /// <param name="array">The typed constant array.</param>
    /// <param name="index">The element index.</param>
    /// <param name="default">The fallback value.</param>
    /// <returns>The cast value or <paramref name="default"/>.</returns>
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

    /// <summary>Tries to get the value at <paramref name="index"/> in <paramref name="array"/> cast to <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected value type.</typeparam>
    /// <param name="array">The typed constant array.</param>
    /// <param name="index">The element index.</param>
    /// <param name="value">The cast value if successful; otherwise default.</param>
    /// <returns><see langword="true"/> if the element exists and can be cast to <typeparamref name="T"/>; otherwise <see langword="false"/>.</returns>
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

    /// <summary>Tries to get the element values at <paramref name="index"/> when the element is an array-kind constant.</summary>
    /// <param name="array">The typed constant array.</param>
    /// <param name="index">The element index.</param>
    /// <param name="values">The array elements if the constant is array-kind; otherwise an empty array.</param>
    /// <returns><see langword="true"/> if the element at <paramref name="index"/> is an array constant; otherwise <see langword="false"/>.</returns>
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
