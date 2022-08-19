#nullable enable

using System;

namespace RoseLynn;

public enum OperatorKind
{
    [CSharpOperatorToken("+")]
    [CanBeCheckedOperator]
    [BinaryOperator]
    Addition,
    [CSharpOperatorToken("-")]
    [CanBeCheckedOperator]
    [BinaryOperator]
    Subtraction,
    [CSharpOperatorToken("*")]
    [CanBeCheckedOperator]
    [BinaryOperator]
    Multiply,
    [CSharpOperatorToken("/")]
    [CanBeCheckedOperator]
    [BinaryOperator]
    Division,
    [CSharpOperatorToken("%")]
    [BinaryOperator]
    Modulus,

    [CSharpOperatorToken("&")]
    [BinaryOperator]
    BitwiseAnd,
    [CSharpOperatorToken("|")]
    [BinaryOperator]
    BitwiseOr,
    [CSharpOperatorToken("^")]
    [BinaryOperator]
    ExclusiveOr,
    [CSharpOperatorToken("<<")]
    [BinaryOperator]
    LeftShift,
    [CSharpOperatorToken(">>")]
    [BinaryOperator]
    RightShift,
    [CSharpOperatorToken(">>>")]
    [BinaryOperator]
    UnsignedRightShift,

    [CSharpOperatorToken("!")]
    [UnaryOperator]
    LogicalNot,
    [CSharpOperatorToken("~")]
    [UnaryOperator]
    OnesComplement,

    [CSharpOperatorToken("+")]
    [UnaryOperator]
    UnaryPlus,
    [CSharpOperatorToken("-")]
    [CanBeCheckedOperator]
    [UnaryOperator]
    UnaryNegation,
    [CSharpOperatorToken("--")]
    [CanBeCheckedOperator]
    [UnaryOperator]
    Decrement,
    [CSharpOperatorToken("++")]
    [CanBeCheckedOperator]
    [UnaryOperator]
    Increment,

    [CSharpOperatorToken("==")]
    [BinaryOperator]
    Equality,
    [CSharpOperatorToken("!=")]
    [BinaryOperator]
    Inequality,
    [CSharpOperatorToken("<")]
    [BinaryOperator]
    LessThan,
    [CSharpOperatorToken(">")]
    [BinaryOperator]
    GreaterThan,
    [CSharpOperatorToken("<=")]
    [BinaryOperator]
    LessThanOrEqual,
    [CSharpOperatorToken(">=")]
    [BinaryOperator]
    GreaterThanOrEqual,

    [CSharpOperatorToken("true")]
    [UnaryOperator]
    True,
    [CSharpOperatorToken("false")]
    [UnaryOperator]
    False,

    [UnaryOperator]
    Implicit,
    [UnaryOperator]
    Explicit,
}

public sealed class CSharpOperatorTokenAttribute : Attribute
{
    public string Token { get; }

    public CSharpOperatorTokenAttribute(string token)
    {
        Token = token;
    }
}

public sealed class CanBeCheckedOperatorAttribute : Attribute { }

public abstract class NaryOperatorAttribute : Attribute { }
public sealed class UnaryOperatorAttribute : NaryOperatorAttribute { }
public sealed class BinaryOperatorAttribute : NaryOperatorAttribute { }
