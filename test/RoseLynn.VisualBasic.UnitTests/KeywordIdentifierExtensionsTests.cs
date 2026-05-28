namespace RoseLynn.VisualBasic.Test;

public sealed class KeywordIdentifierExtensionsTests
{
    [Test]
    [Arguments(SpecialType.System_Boolean, "Boolean")]
    [Arguments(SpecialType.System_Byte, "Byte")]
    [Arguments(SpecialType.System_Char, "Char")]
    [Arguments(SpecialType.System_DateTime, "Date")]
    [Arguments(SpecialType.System_Decimal, "Decimal")]
    [Arguments(SpecialType.System_Double, "Double")]
    [Arguments(SpecialType.System_Int16, "Short")]
    [Arguments(SpecialType.System_Int32, "Integer")]
    [Arguments(SpecialType.System_Int64, "Long")]
    [Arguments(SpecialType.System_Object, "Object")]
    [Arguments(SpecialType.System_SByte, "SByte")]
    [Arguments(SpecialType.System_Single, "Single")]
    [Arguments(SpecialType.System_String, "String")]
    [Arguments(SpecialType.System_UInt16, "UShort")]
    [Arguments(SpecialType.System_UInt32, "UInteger")]
    [Arguments(SpecialType.System_UInt64, "ULong")]
    public async Task GetKeywordIdentifierForPredefinedType(SpecialType specialType, string expectedKeyword)
    {
        await Assert.That(specialType.GetKeywordIdentifierForPredefinedType()).IsEqualTo(expectedKeyword);
    }

    [Test]
    public async Task GetKeywordIdentifierForPredefinedType_None_ReturnsNull()
    {
        await Assert.That(SpecialType.None.GetKeywordIdentifierForPredefinedType()).IsNull();
    }

    [Test]
    public async Task GetKeywordIdentifierForPredefinedType_SystemVoid_ReturnsNull()
    {
        await Assert.That(SpecialType.System_Void.GetKeywordIdentifierForPredefinedType()).IsNull();
    }
}
