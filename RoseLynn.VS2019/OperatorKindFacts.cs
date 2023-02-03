#nullable enable

using System;

namespace RoseLynn;

public static partial class OperatorKindFacts
{
    public static OperatorKind MapNameToKind(string operatorMethodName, out OperatorCheckingMode checkingMode)
    {
        var operatorName = ParseOperatorMethodName(operatorMethodName, out checkingMode);
        return MapNameToKind_Generated(operatorName);
    }

    public static bool CanBeChecked(OperatorKind kind)
    {
        return CanBeChecked_Generated(kind);
    }

    public static bool IsUnary(OperatorKind kind)
    {
        return IsUnary_Generated(kind);
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
