namespace RoseLynn.PoC.UnitTests;

public sealed class POC0001_TwoParameterMethodAnalyzerTests
    : PoCDiagnosticTestsBase<TwoParameterMethodAnalyzer>
{
    [Test]
    public async Task ReportsADiagnosticForEachMethodWithExactlyTwoParameters()
    {
        await AssertDiagnostics(
            """
namespace Demo;

public class Sample
{
    public void M1(int first, {|POC0001:string second|})
    {
    }

    public int M2(string left, {|POC0001:bool right|}) => 0;
}
""");
    }

    [Test]
    public async Task IgnoresMethodsWithOtherParameterCountsAndNonMethodDeclarations()
    {
        await ValidateCode(
            """
namespace Demo;

public class Sample
{
    public Sample(int left, int right)
    {
    }

    public void Zero()
    {
    }

    public void One(int only)
    {
    }

    public void Three(int first, int second, int third)
    {
    }

    public void Wrapper()
    {
        void Local(int left, int right)
        {
        }
    }
}
""");
    }
}
