#nullable enable

using Microsoft.CodeAnalysis;
using System.ComponentModel;

namespace RoseLynn;

/// <summary>Denotes the operator method kind of the operator method symbol.</summary>
public enum OperatorMethodKind
{
    /// <summary>
    /// Reflects a user-defined operator method, equivalent to methods with
    /// the <seealso cref="MethodKind.UserDefinedOperator"/> method kind.
    /// </summary>
    UserDefined,
    /// <summary>
    /// Reflects a built-in operator method, equivalent to methods with
    /// the <seealso cref="MethodKind.BuiltinOperator"/> method kind.
    /// </summary>
    Builtin,
    /// <summary>
    /// Reflects a conversion operator method, equivalent to methods with
    /// the <seealso cref="MethodKind.Conversion"/> method kind.
    /// </summary>
    Conversion,
}

public static class OperatorMethodKindExtensions
{
    public static OperatorMethodKind GetOperatorMethodKind(this MethodKind methodKind) => methodKind switch
    {
        MethodKind.UserDefinedOperator => OperatorMethodKind.UserDefined,
        MethodKind.BuiltinOperator => OperatorMethodKind.Builtin,
        MethodKind.Conversion => OperatorMethodKind.Conversion,

        _ => throw new InvalidEnumArgumentException("The given MethodKind does not reflect an operator method kind.")
    };
    public static MethodKind GetMethodKind(this OperatorMethodKind operatorMethodKind) => operatorMethodKind switch
    {
        OperatorMethodKind.UserDefined => MethodKind.UserDefinedOperator,
        OperatorMethodKind.Builtin => MethodKind.BuiltinOperator,
        OperatorMethodKind.Conversion => MethodKind.Conversion,

        _ => throw new InvalidEnumArgumentException("The given value is undefined.")
    };
}
