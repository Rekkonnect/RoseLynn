using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn;

public sealed class ConstructibleTypedConstant
{
    private readonly TypedConstantKind _kind;
    private readonly object? _constantValue;
    private readonly ImmutableArray<ConstructibleTypedConstant> _values;

    public TypedConstantKind Kind => _kind;
    public object? Value => _constantValue;
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

    public static ConstructibleTypedConstant Array(
        params ImmutableArray<ConstructibleTypedConstant> values)
    {
        return new(TypedConstantKind.Array, null, values);
    }

    public static ConstructibleTypedConstant FromValue(
        TypedConstantKind kind, object? value)
    {
        return new(kind, value, default);
    }

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
