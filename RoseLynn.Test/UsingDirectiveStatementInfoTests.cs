using NUnit.Framework;
using NUnit.Framework.Constraints;
using RoseLynn.CSharp.Syntax;
using RoseLynn.Test.Resources;
using System.Runtime.CompilerServices;

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

    [Test]
    public void VariousUsingsKindsSorted()
    {
        var list = new UsingDirectiveStatementInfoList
        {
            UsingDirectiveStatementInfo.LocalAlias("Alias", "A"),
            new(UsingDirectiveKind.Using, "A"),
            new(UsingDirectiveKind.GlobalUsingStatic, "B.D"),
            new(UsingDirectiveKind.GlobalUsing, "B"),
            new(UsingDirectiveKind.UsingStatic, "A.C"),
            UsingDirectiveStatementInfo.GlobalAlias("AliasB", "B"),
        };
        var actualUsings = list.Sort().ToString();
        const string expectedUsings = """
                                      global using B;
                                      global using static B.D;
                                      global using AliasB = B;
                                      using A;
                                      using static A.C;
                                      using Alias = A;
                                      """;

        Assert.AreEqual(expectedUsings, actualUsings);
    }

    [Test]
    public void UsingForType()
    {
        AssertUsingCreation(
            UsingDirectiveKind.Using,
            $"using {ExampleTypes.BaseNamespace};");

        AssertUsingCreation(
            UsingDirectiveKind.GlobalUsing,
            $"global using {ExampleTypes.BaseNamespace};");

        AssertUsingCreation(
            UsingDirectiveKind.UsingStatic,
            $"using static {ExampleTypes.BaseNamespace}.{nameof(ExampleClass)};");

        AssertUsingCreation(
            UsingDirectiveKind.GlobalUsingStatic,
            $"global using static {ExampleTypes.BaseNamespace}.{nameof(ExampleClass)};");

        AssertInvalidUsingCreation(UsingDirectiveKind.UsingAlias);
        AssertInvalidUsingCreation(UsingDirectiveKind.GlobalUsingAlias);

        static void AssertInvalidUsingCreation(UsingDirectiveKind kind)
        {
            Assert.Catch(() => CreateExampleUsing(kind));
        }

        static void AssertUsingCreation(UsingDirectiveKind kind, string expectedUsing)
        {
            var created = CreateExampleUsing(kind);
            Assert.AreEqual(expectedUsing, created.ToString());
        }

        static UsingDirectiveStatementInfo CreateExampleUsing(UsingDirectiveKind kind)
        {
            return UsingDirectiveStatementInfo.UsingForType<ExampleClass>(kind);
        }
    }
}