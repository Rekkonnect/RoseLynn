using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.Testing;

namespace RoseLynn.Test;

public sealed class ISymbolExtensionsTests
{
    private const string TestSource = """
        using System;

        namespace MyApp.Domain
        {
            [Obsolete("deprecated")]
            public class Annotated
            {
                public int Field;
                public string Property { get; set; } = "";
                public void Method(int x) {}
                public event EventHandler SomeEvent;
            }

            public class Outer
            {
                public class Inner {}
            }

            public enum MyEnum { A, B }
            public interface IMyInterface {}
            public delegate void MyDelegate();
        }
        """;

    private static async Task<Compilation> CreateCompilationAsync(CancellationToken ct)
    {
        var tree = CSharpSyntaxTree.ParseText(TestSource, cancellationToken: ct);
        var test = new CSharpCompilationTest();
        return (await test.CreateCompilation(ct))!.AddSyntaxTrees([tree]);
    }

    private static INamedTypeSymbol GetType(Compilation c, string metadataName) =>
        c.GetTypeByMetadataName(metadataName)!;

    [Test]
    public async Task GetIdentifiableSymbolKind_ForClass_ReturnsClass(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var type = GetType(c, "MyApp.Domain.Annotated");
        await Assert.That(type.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Class);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForStruct_ReturnsStruct(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        await Assert.That(intType.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Struct);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForInterface_ReturnsInterface(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var type = GetType(c, "MyApp.Domain.IMyInterface");
        await Assert.That(type.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Interface);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForEnum_ReturnsEnum(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var type = GetType(c, "MyApp.Domain.MyEnum");
        await Assert.That(type.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Enum);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForDelegate_ReturnsDelegate(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var type = GetType(c, "MyApp.Domain.MyDelegate");
        await Assert.That(type.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Delegate);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForMethod_ReturnsMethod(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var method = annotated.GetMembers("Method").OfType<IMethodSymbol>().First();
        await Assert.That(method.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Method);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForField_ReturnsField(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var field = annotated.GetMembers("Field").OfType<IFieldSymbol>().First();
        await Assert.That(field.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Field);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForProperty_ReturnsProperty(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var property = annotated.GetMembers("Property").OfType<IPropertySymbol>().First();
        await Assert.That(property.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Property);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForEvent_ReturnsEvent(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var ev = annotated.GetMembers("SomeEvent").OfType<IEventSymbol>().First();
        await Assert.That(ev.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Event);
    }

    [Test]
    public async Task GetIdentifiableSymbolKind_ForNamespace_ReturnsNamespace(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var ns = annotated.ContainingNamespace;
        await Assert.That(ns.GetIdentifiableSymbolKind()).IsEqualTo(IdentifiableSymbolKind.Namespace);
    }

    [Test]
    public async Task GetSymbolType_ForField_ReturnsFieldType(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var field = annotated.GetMembers("Field").OfType<IFieldSymbol>().First();
        var type = field.GetSymbolType();
        await Assert.That(type?.SpecialType).IsEqualTo(SpecialType.System_Int32);
    }

    [Test]
    public async Task GetSymbolType_ForProperty_ReturnsPropertyType(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var property = annotated.GetMembers("Property").OfType<IPropertySymbol>().First();
        var type = property.GetSymbolType();
        await Assert.That(type?.SpecialType).IsEqualTo(SpecialType.System_String);
    }

    [Test]
    public async Task GetSymbolType_ForMethod_ReturnsReturnType(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var method = annotated.GetMembers("Method").OfType<IMethodSymbol>().First();
        var type = method.GetSymbolType();
        await Assert.That(type?.SpecialType).IsEqualTo(SpecialType.System_Void);
    }

    [Test]
    public async Task GetSymbolType_ForTypeSymbol_ReturnsSelf(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var type = intType.GetSymbolType();
        await Assert.That(type).IsEqualTo(intType);
    }

    [Test]
    public async Task IsFieldOrProperty_ForField_ReturnsTrue(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var field = annotated.GetMembers("Field").OfType<IFieldSymbol>().First();
        await Assert.That(field.IsFieldOrProperty()).IsTrue();
    }

    [Test]
    public async Task IsFieldOrProperty_ForProperty_ReturnsTrue(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var property = annotated.GetMembers("Property").OfType<IPropertySymbol>().First();
        await Assert.That(property.IsFieldOrProperty()).IsTrue();
    }

    [Test]
    public async Task IsFieldOrProperty_ForMethod_ReturnsFalse(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var method = annotated.GetMembers("Method").OfType<IMethodSymbol>().First();
        await Assert.That(method.IsFieldOrProperty()).IsFalse();
    }

    [Test]
    public async Task HasAttributeNamed_WhenAttributePresent_ReturnsTrue(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        await Assert.That(annotated.HasAttributeNamed("ObsoleteAttribute")).IsTrue();
    }

    [Test]
    public async Task HasAttributeNamed_WhenAttributeAbsent_ReturnsFalse(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        await Assert.That(annotated.HasAttributeNamed("SerializableAttribute")).IsFalse();
    }

    [Test]
    public async Task HasAttributeNamed_GenericOverload_FindsByTypeName(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        await Assert.That(annotated.HasAttributeNamed<ObsoleteAttribute>()).IsTrue();
    }

    [Test]
    public async Task FirstOrDefaultAttributeNamed_WhenPresent_ReturnsAttributeData(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var attr = annotated.FirstOrDefaultAttributeNamed("ObsoleteAttribute");
        await Assert.That(attr).IsNotNull();
    }

    [Test]
    public async Task FirstOrDefaultAttributeNamed_WhenAbsent_ReturnsNull(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var attr = annotated.FirstOrDefaultAttributeNamed("NonExistentAttribute");
        await Assert.That(attr).IsNull();
    }

    [Test]
    public async Task GetContainingNamespaces_ForNestedNamespace_ReturnsMostSpecificLast(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var namespaces = annotated.GetContainingNamespaces()
            .Where(ns => ns.Name.Length > 0) // exclude global namespace
            .Select(ns => ns.Name)
            .ToList();
        // Outermost first: MyApp, Domain
        await Assert.That(namespaces).Contains("MyApp");
        await Assert.That(namespaces).Contains("Domain");
        await Assert.That(namespaces.Last()).IsEqualTo("Domain");
    }

    [Test]
    public async Task GetContainingTypes_ForNestedType_ReturnsOuterFirst(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var innerType = GetType(c, "MyApp.Domain.Outer+Inner");
        var containers = innerType.GetContainingTypes().ToList();
        await Assert.That(containers.Count).IsEqualTo(1);
        await Assert.That(containers[0].Name).IsEqualTo("Outer");
    }

    [Test]
    public async Task GetContainingTypes_ForTopLevelType_ReturnsEmpty(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var containers = annotated.GetContainingTypes().ToList();
        await Assert.That(containers).IsEmpty();
    }

    [Test]
    public async Task MatchesKindAndFullSymbolName_SameSymbol_ReturnsTrue(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var sameIntType = c.GetSpecialType(SpecialType.System_Int32);
        await Assert.That(intType.MatchesKindAndFullSymbolName(sameIntType)).IsTrue();
    }

    [Test]
    public async Task MatchesKindAndFullSymbolName_DifferentSymbols_ReturnsFalse(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var stringType = c.GetSpecialType(SpecialType.System_String);
        await Assert.That(intType.MatchesKindAndFullSymbolName(stringType)).IsFalse();
    }

    [Test]
    public async Task GetAttributesNamed_ReturnsAllMatchingAttributes(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var attrs = annotated.GetAttributesNamed("ObsoleteAttribute").ToList();
        await Assert.That(attrs.Count).IsEqualTo(1);
    }

    [Test]
    public async Task SelectSymbolNames_ReturnsNames(CancellationToken ct)
    {
        var c = await CreateCompilationAsync(ct);
        var annotated = GetType(c, "MyApp.Domain.Annotated");
        var members = annotated.GetMembers()
            .Where(m => m.Kind is SymbolKind.Field or SymbolKind.Property or SymbolKind.Method);
        var names = members.SelectSymbolNames().ToList();
        await Assert.That(names).Contains("Field");
        await Assert.That(names).Contains("Property");
        await Assert.That(names).Contains("Method");
    }
}
