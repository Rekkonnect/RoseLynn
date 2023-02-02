#nullable enable

using System;

namespace RoseLynn;

/// <summary>
/// Represents an operator function's kind.
/// </summary>
public enum OperatorKind
{
    /// <summary>
    /// The addition (<c>+</c>) operator.
    /// </summary>
    [CSharpOperatorToken("+")]
    [CanBeCheckedOperator]
    [BinaryOperator]
    Addition,
    /// <summary>
    /// The subtraction (<c>-</c>) operator.
    /// </summary>
    [CSharpOperatorToken("-")]
    [CanBeCheckedOperator]
    [BinaryOperator]
    Subtraction,
    /// <summary>
    /// The multiplication (<c>*</c>) operator.
    /// </summary>
    [CSharpOperatorToken("*")]
    [CanBeCheckedOperator]
    [BinaryOperator]
    Multiply,
    /// <summary>
    /// The division (<c>/</c>) operator.
    /// </summary>
    [CSharpOperatorToken("/")]
    [CanBeCheckedOperator]
    [BinaryOperator]
    Division,
    /// <summary>
    /// The modulus (<c>%</c>) operator.
    /// </summary>
    [CSharpOperatorToken("%")]
    [BinaryOperator]
    Modulus,

    /// <summary>
    /// The bitwise AND (<c>&amp;</c>) operator.
    /// </summary>
    [CSharpOperatorToken("&")]
    [BinaryOperator]
    BitwiseAnd,
    /// <summary>
    /// The bitwise OR (<c>|</c>) operator.
    /// </summary>
    [CSharpOperatorToken("|")]
    [BinaryOperator]
    BitwiseOr,
    /// <summary>
    /// The bitwise XOR (<c>^</c>) operator.
    /// </summary>
    [CSharpOperatorToken("^")]
    [BinaryOperator]
    ExclusiveOr,
    /// <summary>
    /// The left shift (<c>&lt;&lt;</c>) operator.
    /// </summary>
    [CSharpOperatorToken("<<")]
    [BinaryOperator]
    LeftShift,
    /// <summary>
    /// The right shift (<c>&gt;&gt;</c>) operator.
    /// </summary>
    [CSharpOperatorToken(">>")]
    [BinaryOperator]
    RightShift,
    /// <summary>
    /// The unsigned right shift (<c>&gt;&gt;&gt;</c>) operator.
    /// </summary>
    [CSharpOperatorToken(">>>")]
    [BinaryOperator]
    UnsignedRightShift,

    /// <summary>
    /// The logical NOT (<c>!</c>) operator.
    /// </summary>
    [CSharpOperatorToken("!")]
    [UnaryOperator]
    LogicalNot,
    /// <summary>
    /// The bitwise NOT (<c>~</c>) operator.
    /// </summary>
    [CSharpOperatorToken("~")]
    [UnaryOperator]
    OnesComplement,

    /// <summary>
    /// The unary plus (<c>+</c>) operator.
    /// </summary>
    [CSharpOperatorToken("+")]
    [UnaryOperator]
    UnaryPlus,
    /// <summary>
    /// The unary negation (<c>-</c>) operator.
    /// </summary>
    [CSharpOperatorToken("-")]
    [CanBeCheckedOperator]
    [UnaryOperator]
    UnaryNegation,
    /// <summary>
    /// The decrementation (<c>--</c>) operator.
    /// </summary>
    [CSharpOperatorToken("--")]
    [CanBeCheckedOperator]
    [UnaryOperator]
    Decrement,
    /// <summary>
    /// The incrementation (<c>++</c>) operator.
    /// </summary>
    [CSharpOperatorToken("++")]
    [CanBeCheckedOperator]
    [UnaryOperator]
    Increment,

    /// <summary>
    /// The equality (<c>==</c>) operator.
    /// </summary>
    [CSharpOperatorToken("==")]
    [BinaryOperator]
    Equality,
    /// <summary>
    /// The inequality (<c>!=</c>) operator.
    /// </summary>
    [CSharpOperatorToken("!=")]
    [BinaryOperator]
    Inequality,
    /// <summary>
    /// The less than (<c>&lt;</c>) operator.
    /// </summary>
    [CSharpOperatorToken("<")]
    [BinaryOperator]
    LessThan,
    /// <summary>
    /// The greater than (<c>&gt;</c>) operator.
    /// </summary>
    [CSharpOperatorToken(">")]
    [BinaryOperator]
    GreaterThan,
    /// <summary>
    /// The less than or equal (<c>&lt;=</c>) operator.
    /// </summary>
    [CSharpOperatorToken("<=")]
    [BinaryOperator]
    LessThanOrEqual,
    /// <summary>
    /// The greater than or equal (<c>&gt;=</c>) operator.
    /// </summary>
    [CSharpOperatorToken(">=")]
    [BinaryOperator]
    GreaterThanOrEqual,

    /// <summary>
    /// The <c>true</c> operator, evaluating whether the value
    /// can be treated as a boolean <see langword="true"/> literal.
    /// </summary>
    [CSharpOperatorToken("true")]
    [UnaryOperator]
    True,
    /// <summary>
    /// The <c>false</c> operator, evaluating whether the value
    /// can be treated as a boolean <see langword="false"/> literal.
    /// </summary>
    [CSharpOperatorToken("false")]
    [UnaryOperator]
    False,

    /// <summary>
    /// An <see langword="implicit"/> conversion operator.
    /// </summary>
    [UnaryOperator]
    Implicit,
    /// <summary>
    /// An <see langword="explicit"/> conversion operator.
    /// </summary>
    [UnaryOperator]
    Explicit,
}

/// <summary>
/// Denotes the operator's syntactical token for C#.
/// </summary>
public sealed class CSharpOperatorTokenAttribute : Attribute
{
    /// <summary>
    /// The syntactical token reflecting the operator.
    /// </summary>
    public string Token { get; }

    public CSharpOperatorTokenAttribute(string token)
    {
        Token = token;
    }
}

/// <summary>
/// Denotes that the operator can have a <see langword="checked"/>
/// variant.
/// </summary>
public sealed class CanBeCheckedOperatorAttribute : Attribute { }

/// <summary>
/// Denotes the arity of the operator.
/// </summary>
public abstract class OperatorArityAttribute : Attribute { }
/// <summary>
/// Denotes that the operator is unary.
/// </summary>
public sealed class UnaryOperatorAttribute : OperatorArityAttribute { }
/// <summary>
/// Denotes that the operator is binary.
/// </summary>
public sealed class BinaryOperatorAttribute : OperatorArityAttribute { }
