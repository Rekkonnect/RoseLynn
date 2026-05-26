using Microsoft.CodeAnalysis;
using RoseLynn.Testing;

namespace RoseLynn.PoC.UnitTests;

public sealed class IsolatedRoslynUsagePoCServiceTests
    : BaseCompilationTestContainer
{
    private static readonly IsolatedRoslynUsagePoCService Service = new();

    protected override CSharpCompilationTest CreateTest()
    {
        return new()
        {
            ReferenceAssemblies = PoCTestInfrastructure.ReferenceAssemblies,
        };
    }

    [Test]
    public async Task SyntaxDiscoveryFiltersDownToInterestingClasses()
    {
        var discoveredSources =
            new DiscoveredCSharpSource[]
            {
                new(
                    "Services/RenovationPipeline.cs",
                    """
namespace Demo.Services;

public partial class RenovationPipeline
{
    public void Collect()
    {
    }

    public void Run()
    {
    }
}
"""),
                new(
                    "Services/AbstractPipeline.cs",
                    """
namespace Demo.Services;

public abstract partial class AbstractPipeline
{
    public void Collect()
    {
    }

    public void Run()
    {
    }
}
"""),
                new(
                    "Services/SingleMethod.cs",
                    """
namespace Demo.Services;

public partial class SingleMethod
{
    public void Run()
    {
    }
}
"""),
            };

        var candidates = Service.DiscoverSyntaxFilteredTypes(discoveredSources);

        var singleCandidate = await Assert.That(candidates).HasSingleItem();
        await Assert.That(singleCandidate.FilePath).IsEqualTo("Services/RenovationPipeline.cs");
        await Assert.That(singleCandidate.NamespaceName).IsEqualTo("Demo.Services");
        await Assert.That(singleCandidate.TypeName).IsEqualTo("RenovationPipeline");
        await Assert.That(singleCandidate.DeclaredMethodCount).IsEqualTo(2);
    }

    [Test]
    public async Task CompilationTestCanVerifyClassUsagePermutationsWithinMethods(
        CancellationToken cancellationToken)
    {
        const string source =
            """
using System.Collections.Generic;

namespace Demo;

public sealed class RenovationPlan;
public sealed class PlanBuilder
{
    public RenovationPlan Build(BudgetCalculator calculator, PaintCatalog catalog) => new();
}

public sealed class BudgetCalculator;
public sealed class PaintCatalog;
public sealed class AuditTrail;

public sealed class RenovationService
{
    public RenovationPlan Prepare()
    {
        var builder = new PlanBuilder();
        var calculator = new BudgetCalculator();
        var catalog = new PaintCatalog();

        return builder.Build(calculator, catalog);
    }

    public void Record()
    {
        var audit = new AuditTrail();
        _ = audit;
    }

    public void UseExternalOnly()
    {
        List<string> names = [];
        names.Add("ok");
    }
}
""";

        await VerifyCompilation(source, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var reports = Service.AnalyzeMethodClassUsagePermutations(compilation, cancellationToken);

            var singleReport = await Assert.That(reports).HasSingleItem();
            await Assert.That(singleReport.MethodFullName).IsEqualTo("Demo.RenovationService.Prepare");
            await Assert.That(singleReport.UsedClassSymbols).IsEquivalentTo(
            [
                "Demo.BudgetCalculator",
                "Demo.PaintCatalog",
                "Demo.PlanBuilder",
                "Demo.RenovationPlan",
            ]);

            await Assert.That(singleReport.Permutations).IsEquivalentTo(
            [
                new ClassSymbolPermutation("Demo.BudgetCalculator", "Demo.PaintCatalog"),
                new ClassSymbolPermutation("Demo.BudgetCalculator", "Demo.PlanBuilder"),
                new ClassSymbolPermutation("Demo.BudgetCalculator", "Demo.RenovationPlan"),
                new ClassSymbolPermutation("Demo.PaintCatalog", "Demo.PlanBuilder"),
                new ClassSymbolPermutation("Demo.PaintCatalog", "Demo.RenovationPlan"),
                new ClassSymbolPermutation("Demo.PlanBuilder", "Demo.RenovationPlan"),
            ]);
        }
    }
}
