using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.Core.SignatureViews;
using RoseLynn.Testing;

namespace RoseLynn.Core.UnitTests;

public sealed class SignatureViewTests : BaseCompilationTestContainer
{
    [Test]
    public async Task AllSymbolsSignatureViewsExist(CancellationToken cancellationToken)
    {
        var test = CreateTest();
        var compilation = await test.CreateCompilation(cancellationToken);
        var tree = await UltimateSymbolTesting.ReadUltimateSymbolTestSourceAsync(cancellationToken);
        compilation = compilation!.AddSyntaxTrees([tree]);
        var semanticModel = compilation.GetSemanticModel(tree);
        var root = await tree.GetRootAsync(cancellationToken);
        foreach (var node in root.DescendantNodesAndSelf(descendIntoTrivia: true))
        {
            var symbol = semanticModel.GetSymbolInfo(node, cancellationToken).Symbol;
            if (symbol is not null)
            {
                await AssertSymbolSignatureView(symbol);
            }

            var declaredSymbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);
            if (declaredSymbol is not null)
            {
                await AssertSymbolSignatureView(declaredSymbol);
            }

            var preprocessingInfo = semanticModel.GetPreprocessingSymbolInfo(node);
            if (preprocessingInfo.Symbol is not null)
            {
                await AssertSymbolSignatureView(preprocessingInfo.Symbol);
            }
        }

        async Task AssertSymbolSignatureView(ISymbol symbol)
        {
            var view = symbol.ToSymbolSignatureView(compilation);
            await Assert.That(view).IsNotNull();
        }
    }
}
