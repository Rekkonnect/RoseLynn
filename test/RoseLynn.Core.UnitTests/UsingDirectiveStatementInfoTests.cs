using RoseLynn.CSharp.Syntax;
using RoseLynn.Test.Resources;

namespace RoseLynn.Test;

/// <summary>Tests formatting and sorting behavior for <see cref="UsingDirectiveStatementInfo"/> values.</summary>
public class UsingDirectiveStatementInfoTests
{
    [Test]
    public async Task SimpleUsings()
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

        await Assert.That(actualUsings).IsEqualTo(expectedUsings);
    }

    [Test]
    public async Task SimpleUsingsSorted()
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

        await Assert.That(actualUsings).IsEqualTo(expectedUsings);
    }

    [Test]
    public async Task VariousUsingsKinds()
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

        await Assert.That(actualUsings).IsEqualTo(expectedUsings);
    }

    [Test]
    public async Task VariousUsingsKindsSorted()
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

        await Assert.That(actualUsings).IsEqualTo(expectedUsings);
    }

    [Test]
    public async Task UsingForType()
    {
        await AssertUsingCreation(
            UsingDirectiveKind.Using,
            $"using {ExampleTypes.BaseNamespace};");

        await AssertUsingCreation(
            UsingDirectiveKind.GlobalUsing,
            $"global using {ExampleTypes.BaseNamespace};");

        await AssertUsingCreation(
            UsingDirectiveKind.UsingStatic,
            $"using static {ExampleTypes.BaseNamespace}.{nameof(ExampleClass)};");

        await AssertUsingCreation(
            UsingDirectiveKind.GlobalUsingStatic,
            $"global using static {ExampleTypes.BaseNamespace}.{nameof(ExampleClass)};");

        await AssertInvalidUsingCreation(UsingDirectiveKind.UsingAlias);
        await AssertInvalidUsingCreation(UsingDirectiveKind.GlobalUsingAlias);

        static async Task AssertInvalidUsingCreation(UsingDirectiveKind kind)
        {
            await Assert.That(() => CreateExampleUsing(kind)).Throws<Exception>();
        }

        static async Task AssertUsingCreation(UsingDirectiveKind kind, string expectedUsing)
        {
            var created = CreateExampleUsing(kind);
            await Assert.That(created.ToString()).IsEqualTo(expectedUsing);
        }

        static UsingDirectiveStatementInfo CreateExampleUsing(UsingDirectiveKind kind)
        {
            return UsingDirectiveStatementInfo.UsingForType<ExampleClass>(kind);
        }
    }

    [Test]
    public async Task SortingComparer_HandlesNulls()
    {
        var comparer = UsingDirectiveStatementInfo.SortingComparer.Instance;
        var usingDirective = new UsingDirectiveStatementInfo(UsingDirectiveKind.Using, "System");

        await Assert.That(comparer.Compare(null, null)).IsEqualTo(0);
        await Assert.That(comparer.Compare(null, usingDirective)).IsEqualTo(-1);
        await Assert.That(comparer.Compare(usingDirective, null)).IsEqualTo(1);
    }

    [Test]
    public async Task IdentifierSortKeyComparer_UsesOrdinalComparison()
    {
        var comparer = UsingDirectiveStatementInfo.SortingComparer.AscendingIdentifierSortKey.Instance;
        var lowerCase = new UsingDirectiveStatementInfo(UsingDirectiveKind.Using, "a");
        var upperCase = new UsingDirectiveStatementInfo(UsingDirectiveKind.Using, "A");

        await Assert.That(comparer.Compare(lowerCase, upperCase)).IsGreaterThan(0);
    }
}
