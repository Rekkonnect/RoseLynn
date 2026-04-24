#nullable enable

using System;

namespace RoseLynn;

public static partial class OperatorKindFacts
{
    public static OperatorKind MapNameToKind(string operatorMethodName, out OperatorCheckingMode checkingMode)
    {
        var operatorName = ParseOperatorMethodName(operatorMethodName, out checkingMode);
        return operatorName switch
        {
            nameof(OperatorKind.Addition) => OperatorKind.Addition,
            nameof(OperatorKind.Subtraction) => OperatorKind.Subtraction,
            nameof(OperatorKind.Multiply) => OperatorKind.Multiply,
            nameof(OperatorKind.Division) => OperatorKind.Division,
            nameof(OperatorKind.Modulus) => OperatorKind.Modulus,
            nameof(OperatorKind.BitwiseAnd) => OperatorKind.BitwiseAnd,
            nameof(OperatorKind.BitwiseOr) => OperatorKind.BitwiseOr,
            nameof(OperatorKind.ExclusiveOr) => OperatorKind.ExclusiveOr,
            nameof(OperatorKind.LeftShift) => OperatorKind.LeftShift,
            nameof(OperatorKind.RightShift) => OperatorKind.RightShift,
            nameof(OperatorKind.UnsignedRightShift) => OperatorKind.UnsignedRightShift,
            nameof(OperatorKind.LogicalNot) => OperatorKind.LogicalNot,
            nameof(OperatorKind.OnesComplement) => OperatorKind.OnesComplement,
            nameof(OperatorKind.UnaryPlus) => OperatorKind.UnaryPlus,
            nameof(OperatorKind.UnaryNegation) => OperatorKind.UnaryNegation,
            nameof(OperatorKind.Decrement) => OperatorKind.Decrement,
            nameof(OperatorKind.Increment) => OperatorKind.Increment,
            nameof(OperatorKind.Equality) => OperatorKind.Equality,
            nameof(OperatorKind.Inequality) => OperatorKind.Inequality,
            nameof(OperatorKind.LessThan) => OperatorKind.LessThan,
            nameof(OperatorKind.GreaterThan) => OperatorKind.GreaterThan,
            nameof(OperatorKind.LessThanOrEqual) => OperatorKind.LessThanOrEqual,
            nameof(OperatorKind.GreaterThanOrEqual) => OperatorKind.GreaterThanOrEqual,
            nameof(OperatorKind.True) => OperatorKind.True,
            nameof(OperatorKind.False) => OperatorKind.False,
            nameof(OperatorKind.Implicit) => OperatorKind.Implicit,
            nameof(OperatorKind.Explicit) => OperatorKind.Explicit,
            _ => throw new ArgumentException("The given name does not reflect an operator name."),
        };
    }

    public static bool CanBeChecked(OperatorKind kind)
    {
        return kind is OperatorKind.Addition
            or OperatorKind.Subtraction
            or OperatorKind.Multiply
            or OperatorKind.Division
            or OperatorKind.UnaryNegation
            or OperatorKind.Decrement
            or OperatorKind.Increment;
    }

    public static bool IsUnary(OperatorKind kind)
    {
        return kind is OperatorKind.LogicalNot
            or OperatorKind.OnesComplement
            or OperatorKind.UnaryPlus
            or OperatorKind.UnaryNegation
            or OperatorKind.Decrement
            or OperatorKind.Increment
            or OperatorKind.True
            or OperatorKind.False
            or OperatorKind.Implicit
            or OperatorKind.Explicit;
    }
    public static bool IsBinary(OperatorKind kind)
    {
        return !IsUnary(kind);
    }

    private static string ParseOperatorMethodName(string name, out OperatorCheckingMode checkingMode)
    {
        const string opPrefix = "op_";
        const string checkedPrefix = "Checked";

        var opPrefixSpan = opPrefix.AsSpan();
        var checkedPrefixSpan = checkedPrefix.AsSpan();

        checkingMode = OperatorCheckingMode.Undefined;

        var operatorName = name.AsSpan();
        if (operatorName.StartsWith(opPrefixSpan, StringComparison.Ordinal))
        {
            operatorName = operatorName.Slice(opPrefix.Length);
            if (operatorName.StartsWith(checkedPrefixSpan, StringComparison.Ordinal))
            {
                operatorName = operatorName.Slice(checkedPrefix.Length);
                checkingMode = OperatorCheckingMode.Checked;
            }
        }

        return operatorName.ToString();
    }
}
