using NUnit.Framework;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn.Test;

public class UsingDirectiveStatementInfoTests
{
    [Test]
    public void SimpleUsings()
    {
        var list = new UsingDirectiveStatementInfoList();
        list.AddRange(UsingDirectiveKind.Using, "A", "B", "C", "A.D.E", "A.A0", "F.G");
        var actualUsings = list.ToString();
        const string expectedUsings = """
                                      using A;
                                      using B;
                                      using C;
                                      using A.D.E;
                                      using A.A0;
                                      using F.G;
                                      """;

        Assert.AreEqual(expectedUsings, actualUsings);
    }

    [Test]
    public void SimpleUsingsSorted()
    {
        var list = new UsingDirectiveStatementInfoList();
        list.AddRange(UsingDirectiveKind.Using, "A", "B", "C", "A.D.E", "A.A0", "F.G");
        var actualUsings = list.Sort().ToString();
        const string expectedUsings = """
                                      using A;
                                      using A.A0;
                                      using A.D.E;
                                      using B;
                                      using C;
                                      using F.G;
                                      """;

        Assert.AreEqual(expectedUsings, actualUsings);
    }

    [Test]
    public void VariousUsingsKinds()
    {
        var list = new UsingDirectiveStatementInfoList
        {
            new(UsingDirectiveKind.Using, "A"),
            new(UsingDirectiveKind.GlobalUsing, "B"),
            new(UsingDirectiveKind.UsingStatic, "A.C"),
            new(UsingDirectiveKind.GlobalUsingStatic, "B.D"),
            UsingDirectiveStatementInfo.LocalAlias("Alias", "A"),
            UsingDirectiveStatementInfo.GlobalAlias("AliasB", "B"),
        };
        var actualUsings = list.ToString();
        const string expectedUsings = """
                                      using A;
                                      global using B;
                                      using static A.C;
                                      global using static B.D;
                                      using Alias = A;
                                      global using AliasB = B;
                                      """;

        Assert.AreEqual(expectedUsings, actualUsings);
    }
}