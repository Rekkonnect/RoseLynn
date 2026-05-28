namespace RoseLynn.PoC.UnitTests;

public sealed class POC0001_TwoParameterMethodCodeFixProviderTests
    : PoCCodeFixTestsBase<TwoParameterMethodAnalyzer, TwoParameterMethodCodeFixProvider>
{
    [Test]
    public async Task RemovesTheSecondParameterFromABlockBodiedMethod()
    {
        await TestCodeFixAsync(
            """
namespace Demo;

public class Sample
{
    public void Paint(int coats, {|POC0001:string color|})
    {
    }
}
""",
            """
namespace Demo;

public class Sample
{
    public void Paint(int coats)
    {
    }
}
""");
    }

    [Test]
    public async Task PreservesTheFirstParameterAndMethodBodyForExpressionBodiedMethods()
    {
        await TestCodeFixAsync(
            """
namespace Demo;

public class Sample
{
    public int Mix(in int value, {|POC0001:int factor|}) => value;
}
""",
            """
namespace Demo;

public class Sample
{
    public int Mix(in int value) => value;
}
""");
    }
}
