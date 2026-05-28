using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn.Core.SignatureViews;
using RoseLynn.Testing;
using System.Diagnostics.CodeAnalysis;

namespace RoseLynn.Test;

public sealed class SymbolSignatureViewTests : BaseCompilationTestContainer
{
    [Test]
    public async Task TupleType(CancellationToken cancellationToken)
    {
        const string markup = """
            [|(int a, int b)|] v = default;
            """;
        var compilationInfo = await CreateCompilation(markup, cancellationToken);
        var semanticModel = compilationInfo.SemanticModel;
        var tupleNode = compilationInfo.Node;

        var tupleTypeInfo = semanticModel.GetTypeInfo(
            tupleNode, cancellationToken: cancellationToken);
        var type = tupleTypeInfo.Type as INamedTypeSymbol;
        await Assert.That(type).IsNotNull();

        var signatureView = type.ToSymbolSignatureView(semanticModel.Compilation);
        await Assert.That(signatureView.FullyQualifiedName)
            .IsEqualTo("global::System.ValueTuple<global::System.Int32, global::System.Int32>");

        await Assert.That(signatureView.TupleFieldNames)
            .IsEquivalentTo(["a", "b"]);

        await Assert.That(signatureView.SimplerTupleIdentifier())
            .IsEqualTo("(global::System.Int32 a, global::System.Int32 b)");
    }

    [Test]
    public async Task AnonymousType(CancellationToken cancellationToken)
    {
        const string markup = """
            _ = [|new { Length = 1 }|];
            """;
        var compilationInfo = await CreateCompilation(markup, cancellationToken);
        var semanticModel = compilationInfo.SemanticModel;
        var tupleNode = compilationInfo.Node;

        var tupleTypeInfo = semanticModel.GetTypeInfo(
            tupleNode, cancellationToken: cancellationToken);
        var type = tupleTypeInfo.Type as INamedTypeSymbol;
        await Assert.That(type).IsNotNull();

        var signatureView = type.ToSymbolSignatureView(semanticModel.Compilation);
        await Assert.That(signatureView.FullyQualifiedName)
            .IsEqualTo(string.Empty);
    }

    private async Task<CreatedCompilationInfo> CreateCompilation(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string markup,
        CancellationToken cancellationToken)
    {
        TestFileMarkupParser.GetSpan(markup, out var source, out var span);
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken);
        var test = CreateTest();
        var compilation = await test.CreateCompilation(cancellationToken);
        compilation = compilation!.AddSyntaxTrees([tree]);
        var semanticModel = compilation.GetSemanticModel(tree);
        var root = await tree.GetRootAsync(cancellationToken);
        var node = root.FindNode(span);
        return new CreatedCompilationInfo(test, semanticModel, root, node);
    }

    private record class CreatedCompilationInfo(
        CSharpCompilationTest Test,
        SemanticModel SemanticModel,
        SyntaxNode Root,
        SyntaxNode Node)
    {
        public Compilation Compilation => SemanticModel.Compilation;
    }
}
