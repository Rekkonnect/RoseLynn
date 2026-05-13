using System.Threading.Tasks;

namespace RoseLynn.Writing.Test;

public abstract class BaseIndentedStringBuilderTests
{
    protected static async Task AssertStrings(IndentedStringBuilder builder, string expected)
    {
        await Assert.That(builder.ToString()).IsEqualTo(expected);
    }
}
