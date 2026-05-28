using Microsoft.CodeAnalysis;
using RoseLynn.Testing;
using System.Diagnostics.CodeAnalysis;

namespace RoseLynn.Generators.UnitTests;

/// <summary>Verifies the incrementality behavior of <see cref="FullTypeNameGenerator"/>.</summary>
public sealed class FullTypeNameGeneratorIncrementalityTests
    : BaseGeneratorIncrementalityTestContainer<FullTypeNameGenerator>
{
    [StringSyntax(LanguageNames.CSharp)]
    private const string BaselineSource = """
namespace CustomType.Example.Namespace;

partial class Type
{
}
""";

    [StringSyntax(LanguageNames.CSharp)]
    private const string RenamedTypeSource = """
namespace CustomType.Example.OtherNamespace;

partial class RenamedType
{
}
""";

    [StringSyntax(LanguageNames.CSharp)]
    private const string SourceWithAddedMembers = """
namespace CustomType.Example.Namespace;

partial class Type
{
    public void M()
    {
    }

    public int P { get; set; }
}
""";

    [StringSyntax(LanguageNames.CSharp)]
    private const string SourceWithBaseList = """
namespace CustomType.Example.Namespace;

partial class Type : object
{
}
""";

    [StringSyntax(LanguageNames.CSharp)]
    private const string StructSource = """
namespace CustomType.Example.Namespace;

partial struct Type
{
}
""";

    [Test]
    public async Task ChangingTypeNameOrNamespaceInvalidatesTheOutput(CancellationToken cancellationToken)
    {
        await VerifyHasNonCachedGenerationSteps(BaselineSource, RenamedTypeSource, AssertTrue, cancellationToken);
        await VerifyHasNonCachedGenerationSteps(RenamedTypeSource, BaselineSource, AssertTrue, cancellationToken);
    }

    [Test]
    public async Task ChangingMembersDoesNotInvalidateTheOutput(CancellationToken cancellationToken)
    {
        await VerifyHasOnlyCachedGenerationStepsSymmetrically(
            BaselineSource,
            SourceWithAddedMembers,
            AssertTrue,
            cancellationToken);
    }

    [Test]
    public async Task ChangingBaseTypeListDoesNotInvalidateTheOutput(CancellationToken cancellationToken)
    {
        await VerifyHasOnlyCachedGenerationStepsSymmetrically(
            BaselineSource,
            SourceWithBaseList,
            AssertTrue,
            cancellationToken);
    }

    [Test]
    public async Task ChangingTypeKindInvalidatesTheOutput(CancellationToken cancellationToken)
    {
        await VerifyHasNonCachedGenerationSteps(BaselineSource, StructSource, AssertTrue, cancellationToken);
        await VerifyHasNonCachedGenerationSteps(StructSource, BaselineSource, AssertTrue, cancellationToken);
    }

    private static async ValueTask AssertTrue(bool value)
    {
        await Assert.That(value).IsTrue();
    }
}