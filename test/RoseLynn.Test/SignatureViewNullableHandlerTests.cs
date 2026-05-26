using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn.CSharp;
using RoseLynn.CSharp.SignatureViews;
using RoseLynn.Testing;
using System.Diagnostics.CodeAnalysis;

namespace RoseLynn.Test;

public sealed class SignatureViewNullableHandlerTests
    : BaseCompilationTestContainer
{
    [Test]
    public async Task ClassBecomesNullable()
    {
        var view = WellKnownSymbolSignatureViews.System_String;
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Class);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.None);

        var nullableType = view.NullableHandler().MakeNullable();
        await Assert.That(
            nullableType.NullableHandler().AcceptsNull).IsTrue();
        await Assert.That(
            nullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.Annotated);
        await Assert.That(
            nullableType.FullyQualifiedName).IsEqualTo("global::System.String?");
    }

    [Test]
    public async Task ClassBecomesNotNullable()
    {
        var view = WellKnownSymbolSignatureViews.System_String;
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Class);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.None);

        var notNullableType = view.NullableHandler().MakeNotNullable();
        await Assert.That(
            notNullableType.NullableHandler().AcceptsNull).IsFalse();
        await Assert.That(
            notNullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(
            notNullableType.FullyQualifiedName).IsEqualTo("global::System.String");
    }

    [Test]
    public async Task StructBecomesNullable()
    {
        var view = WellKnownSymbolSignatureViews.System_Int32;
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Struct);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var nullableType = view.NullableHandler().MakeNullable();
        await Assert.That(
            nullableType.NullableHandler().AcceptsNull).IsTrue();
        await Assert.That(
            nullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(
            nullableType.FullyQualifiedName).IsEqualTo("global::System.Nullable<global::System.Int32>");
    }

    [Test]
    public async Task StructRemainsNotNullable()
    {
        var view = WellKnownSymbolSignatureViews.System_Int32;
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Struct);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var notNullableType = view.NullableHandler().MakeNotNullable();
        await Assert.That(
            notNullableType.NullableHandler().AcceptsNull).IsFalse();
        await Assert.That(
            notNullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(
            notNullableType.FullyQualifiedName).IsEqualTo("global::System.Int32");
    }

    [Test]
    public async Task NullableStructRemainsNullable()
    {
        var view = WellKnownSymbolSignatureViews.System_Nullable_T;
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Struct);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var nullableType = view.NullableHandler().MakeNullable();
        await Assert.That(
            nullableType.NullableHandler().AcceptsNull).IsTrue();
        await Assert.That(
            nullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(
            nullableType.FullyQualifiedName).IsEqualTo("global::System.Nullable<T>");
    }

    [Test]
    public async Task NullableStructBecomesNotNullable()
    {
        var view = WellKnownSymbolSignatureViews.System_Nullable_T;
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Struct);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var notNullableType = view.NullableHandler().MakeNotNullable();
        await Assert.That(
            notNullableType.NullableHandler().AcceptsNull).IsFalse();
        await Assert.That(
            notNullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(
            notNullableType.FullyQualifiedName).IsEqualTo("T");
    }

    [Test]
    public async Task ConstructedNullableStructRemainsNullable()
    {
        var view = WellKnownSymbolSignatureViews.System_Nullable_T
            .Construct([WellKnownSymbolSignatureViews.System_Int32]);
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Struct);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var nullableType = view.NullableHandler().MakeNullable();
        await Assert.That(
            nullableType.NullableHandler().AcceptsNull).IsTrue();
        await Assert.That(
            nullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(
            nullableType.FullyQualifiedName).IsEqualTo("global::System.Nullable<global::System.Int32>");
    }

    [Test]
    public async Task ConstructedNullableStructBecomesNotNullable()
    {
        var view = WellKnownSymbolSignatureViews.System_Nullable_T
            .Construct([WellKnownSymbolSignatureViews.System_Int32]);
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Struct);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var notNullableType = view.NullableHandler().MakeNotNullable();
        await Assert.That(
            notNullableType.NullableHandler().AcceptsNull).IsFalse();
        await Assert.That(
            notNullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(
            notNullableType.FullyQualifiedName).IsEqualTo("global::System.Int32");
    }

    [Test]
    public async Task TupleBecomesNullable(CancellationToken cancellationToken)
    {
        const string markup = """
            [|(int a, int b)|] v = default;
            """;
        var view = await GetSymbolView(markup, cancellationToken)
            as INamedTypeSymbolSignatureView;

        await Assert.That(view).IsNotNull();
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Struct);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var nullableType = view.NullableHandler().MakeNullable();
        await Assert.That(
            nullableType.NullableHandler().AcceptsNull).IsTrue();
        await Assert.That(
            nullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(nullableType.SimplerIdentifier())
            .IsEqualTo("(int a, int b)?");
    }

    [Test]
    public async Task TupleRemainsNotNullable(CancellationToken cancellationToken)
    {
        const string markup = """
            [|(int a, int b)|] v = default;
            """;
        var view = await GetSymbolView(markup, cancellationToken)
            as INamedTypeSymbolSignatureView;

        await Assert.That(view).IsNotNull();
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.Struct);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var notNullableType = view.NullableHandler().MakeNotNullable();
        await Assert.That(
            notNullableType.NullableHandler().AcceptsNull).IsFalse();
        await Assert.That(
            notNullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(notNullableType.SimplerIdentifier())
            .IsEqualTo("(int a, int b)");
    }

    // In the below tests, the type parameters are discovered through their
    // using symbols, instead of directly and in isolation
    // This workaround is required because, if we only marked up T?,
    // GetSymbolInfo returns just T, without the nullable annotation
    // The annotation is only respected when it derives from the symbol
    // that uses its nullable-annotated version

    [Test]
    public async Task UnconstrainedTypeParameterBecomesNullable(CancellationToken cancellationToken)
    {
        const string markup = """
            public class C<T>([|T value|]);
            """;
        var parameterView = await GetSymbolView(markup, cancellationToken)
            as IParameterSymbolSignatureView;
        await Assert.That(parameterView).IsNotNull();

        var view = parameterView.Type;
        await Assert.That(view).IsNotNull();
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.TypeParameter);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);

        var nullableType = view.NullableHandler().MakeNullable();
        await Assert.That(
            nullableType.NullableHandler().AcceptsNull).IsTrue();
        await Assert.That(
            nullableType.NullableAnnotation).IsEqualTo(NullableAnnotation.Annotated);
        await Assert.That(nullableType.FullyQualifiedName).IsEqualTo("T?");
    }

    [Test]
    public async Task UnconstrainedNullableTypeParameterBecomesNotNullable(CancellationToken cancellationToken)
    {
        const string markup = """
            public class C<T>([|T? value|]);
            """;
        var parameterView = await GetSymbolView(markup, cancellationToken)
            as IParameterSymbolSignatureView;
        await Assert.That(parameterView).IsNotNull();

        var view = parameterView.Type;
        await Assert.That(view).IsNotNull();
        await Assert.That(view.TypeKind).IsEqualTo(TypeKind.TypeParameter);
        await Assert.That(
            view.NullableAnnotation).IsEqualTo(NullableAnnotation.Annotated);

        var notNullableType = view.NullableHandler().MakeNotNullable();
        await Assert.That(
            notNullableType.NullableHandler().AcceptsNull).IsFalse();
        await Assert.That(notNullableType.NullableAnnotation)
            .IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(notNullableType.FullyQualifiedName).IsEqualTo("T");
    }

    // TODO: Test more type parameter constraints

    private async Task<ISymbolSignatureView?> GetSymbolView(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string markup,
        CancellationToken cancellationToken)
    {
        TestFileMarkupParser.GetSpan(markup, out var source, out var span);
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken);
        var test = CreateTest();
        var compilation = await test.CreateCompilation(cancellationToken);
        compilation = compilation!
            .WithNullableContextOptions(NullableContextOptions.Enable)
            .AddSyntaxTrees([tree]);
        var semanticModel = compilation.GetSemanticModel(tree);
        var root = await tree.GetRootAsync(cancellationToken);
        var node = root.FindNode(span);
        var symbol = semanticModel.GetDeclaredSymbol(node, cancellationToken)
            ?? semanticModel.GetSymbolInfo(node, cancellationToken).Symbol;
        return symbol?.ToSymbolSignatureView(compilation);
    }
}
