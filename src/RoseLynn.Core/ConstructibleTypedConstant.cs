using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn.Core;

/// <summary>Represents a mutable, constructible analog of <seealso cref="TypedConstant"/> for programmatic creation of typed constant values.</summary>
public sealed class ConstructibleTypedConstant
{
    private readonly TypedConstantKind _kind;
    private readonly object? _constantValue;
    private readonly ImmutableArray<ConstructibleTypedConstant> _values;

    /// <summary>Gets the kind of this typed constant.</summary>
    public TypedConstantKind Kind => _kind;
    /// <summary>Gets the primitive or enum value, or <see langword="null"/> for array constants.</summary>
    public object? Value => _constantValue;
    /// <summary>Gets the elements of an array constant, or <see langword="null"/> if this is not an array.</summary>
    public ImmutableArray<ConstructibleTypedConstant>? Values => _values.IsDefault ? null : _values;

    private ConstructibleTypedConstant(
        TypedConstantKind kind,
        object? constantValue,
        ImmutableArray<ConstructibleTypedConstant> values)
    {
        _kind = kind;
        _constantValue = constantValue;
        _values = values;
    }

    /// <summary>Creates an array-kind <seealso cref="ConstructibleTypedConstant"/> from the given element values.</summary>
    /// <param name="values">The element values of the array.</param>
    /// <returns>A new array-kind constant.</returns>
    public static ConstructibleTypedConstant Array(
        params ImmutableArray<ConstructibleTypedConstant> values)
    {
        return new(TypedConstantKind.Array, null, values);
    }

    /// <summary>Creates a non-array <seealso cref="ConstructibleTypedConstant"/> with the given kind and scalar value.</summary>
    /// <param name="kind">The typed constant kind (must not be <seealso cref="TypedConstantKind.Array"/>).</param>
    /// <param name="value">The scalar value.</param>
    /// <returns>A new scalar-kind constant.</returns>
    public static ConstructibleTypedConstant FromValue(
        TypedConstantKind kind, object? value)
    {
        return new(kind, value, default);
    }

    /// <summary>Creates a <seealso cref="ConstructibleTypedConstant"/> from an existing <seealso cref="TypedConstant"/>, recursively converting array elements.</summary>
    /// <param name="constant">The Roslyn typed constant to convert.</param>
    /// <returns>A <seealso cref="ConstructibleTypedConstant"/> equivalent to <paramref name="constant"/>.</returns>
    public static ConstructibleTypedConstant FromTypedConstant(TypedConstant constant)
    {
        if (constant.Kind is TypedConstantKind.Array)
        {
            return Array(
                constant.Values
                    .Select(FromTypedConstant)
                    .ToImmutableArray());
        }

        return FromValue(constant.Kind, constant.Value);
    }
}
