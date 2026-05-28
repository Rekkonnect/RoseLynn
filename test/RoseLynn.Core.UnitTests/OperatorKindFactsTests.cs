namespace RoseLynn.Core.UnitTests;

public sealed class OperatorKindFactsTests
{
    [Test]
    [Arguments(OperatorKind.Addition)]
    [Arguments(OperatorKind.Subtraction)]
    [Arguments(OperatorKind.Multiply)]
    [Arguments(OperatorKind.Division)]
    [Arguments(OperatorKind.UnaryNegation)]
    [Arguments(OperatorKind.Decrement)]
    [Arguments(OperatorKind.Increment)]
    [Arguments(OperatorKind.Explicit)]
    public async Task CanBeChecked_ReturnsTrue_ForCheckableOperator(OperatorKind kind)
    {
        await Assert.That(OperatorKindFacts.CanBeChecked(kind)).IsTrue();
    }

    [Test]
    [Arguments(OperatorKind.Modulus)]
    [Arguments(OperatorKind.BitwiseAnd)]
    [Arguments(OperatorKind.BitwiseOr)]
    [Arguments(OperatorKind.ExclusiveOr)]
    [Arguments(OperatorKind.LeftShift)]
    [Arguments(OperatorKind.RightShift)]
    [Arguments(OperatorKind.UnsignedRightShift)]
    [Arguments(OperatorKind.LogicalNot)]
    [Arguments(OperatorKind.OnesComplement)]
    [Arguments(OperatorKind.UnaryPlus)]
    [Arguments(OperatorKind.Equality)]
    [Arguments(OperatorKind.Inequality)]
    [Arguments(OperatorKind.LessThan)]
    [Arguments(OperatorKind.GreaterThan)]
    [Arguments(OperatorKind.LessThanOrEqual)]
    [Arguments(OperatorKind.GreaterThanOrEqual)]
    [Arguments(OperatorKind.True)]
    [Arguments(OperatorKind.False)]
    [Arguments(OperatorKind.Implicit)]
    public async Task CanBeChecked_ReturnsFalse_ForNonCheckableOperator(OperatorKind kind)
    {
        await Assert.That(OperatorKindFacts.CanBeChecked(kind)).IsFalse();
    }

    [Test]
    [Arguments(OperatorKind.LogicalNot)]
    [Arguments(OperatorKind.OnesComplement)]
    [Arguments(OperatorKind.UnaryPlus)]
    [Arguments(OperatorKind.UnaryNegation)]
    [Arguments(OperatorKind.Decrement)]
    [Arguments(OperatorKind.Increment)]
    [Arguments(OperatorKind.True)]
    [Arguments(OperatorKind.False)]
    [Arguments(OperatorKind.Implicit)]
    [Arguments(OperatorKind.Explicit)]
    public async Task IsUnary_ReturnsTrue_ForUnaryOperator(OperatorKind kind)
    {
        await Assert.That(OperatorKindFacts.IsUnary(kind)).IsTrue();
    }

    [Test]
    [Arguments(OperatorKind.Addition)]
    [Arguments(OperatorKind.Subtraction)]
    [Arguments(OperatorKind.Multiply)]
    [Arguments(OperatorKind.Division)]
    [Arguments(OperatorKind.Modulus)]
    [Arguments(OperatorKind.UnsignedRightShift)]
    [Arguments(OperatorKind.LeftShift)]
    [Arguments(OperatorKind.RightShift)]
    [Arguments(OperatorKind.Equality)]
    [Arguments(OperatorKind.Inequality)]
    public async Task IsBinary_ReturnsTrue_ForBinaryOperator(OperatorKind kind)
    {
        await Assert.That(OperatorKindFacts.IsBinary(kind)).IsTrue();
    }

    [Test]
    [Arguments("op_Addition", OperatorKind.Addition, OperatorCheckingMode.Undefined)]
    [Arguments("op_CheckedAddition", OperatorKind.Addition, OperatorCheckingMode.Checked)]
    [Arguments("op_Subtraction", OperatorKind.Subtraction, OperatorCheckingMode.Undefined)]
    [Arguments("op_CheckedSubtraction", OperatorKind.Subtraction, OperatorCheckingMode.Checked)]
    [Arguments("op_Multiply", OperatorKind.Multiply, OperatorCheckingMode.Undefined)]
    [Arguments("op_CheckedMultiply", OperatorKind.Multiply, OperatorCheckingMode.Checked)]
    [Arguments("op_Division", OperatorKind.Division, OperatorCheckingMode.Undefined)]
    [Arguments("op_CheckedDivision", OperatorKind.Division, OperatorCheckingMode.Checked)]
    [Arguments("op_UnaryNegation", OperatorKind.UnaryNegation, OperatorCheckingMode.Undefined)]
    [Arguments("op_CheckedUnaryNegation", OperatorKind.UnaryNegation, OperatorCheckingMode.Checked)]
    [Arguments("op_Increment", OperatorKind.Increment, OperatorCheckingMode.Undefined)]
    [Arguments("op_CheckedIncrement", OperatorKind.Increment, OperatorCheckingMode.Checked)]
    [Arguments("op_Decrement", OperatorKind.Decrement, OperatorCheckingMode.Undefined)]
    [Arguments("op_CheckedDecrement", OperatorKind.Decrement, OperatorCheckingMode.Checked)]
    [Arguments("op_Explicit", OperatorKind.Explicit, OperatorCheckingMode.Undefined)]
    [Arguments("op_CheckedExplicit", OperatorKind.Explicit, OperatorCheckingMode.Checked)]
    [Arguments("op_Implicit", OperatorKind.Implicit, OperatorCheckingMode.Undefined)]
    [Arguments("op_UnsignedRightShift", OperatorKind.UnsignedRightShift, OperatorCheckingMode.Undefined)]
    public async Task MapNameToKind_ParsesCorrectly(string methodName, OperatorKind expectedKind, OperatorCheckingMode expectedMode)
    {
        var kind = OperatorKindFacts.MapNameToKind(methodName, out var mode);
        await Assert.That(kind).IsEqualTo(expectedKind);
        await Assert.That(mode).IsEqualTo(expectedMode);
    }
}
