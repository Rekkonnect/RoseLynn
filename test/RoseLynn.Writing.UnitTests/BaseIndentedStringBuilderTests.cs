namespace RoseLynn.Writing.UnitTests;

public abstract class BaseIndentedStringBuilderTests
{
    protected static async Task AssertStrings(IndentedStringBuilder builder, string expected)
    {
        await Assert.That(builder.ToString()).IsEqualTo(expected);
    }
}
