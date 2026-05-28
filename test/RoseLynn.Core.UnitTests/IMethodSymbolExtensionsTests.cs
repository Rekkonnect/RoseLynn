using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.Testing;

namespace RoseLynn.Core.UnitTests;

public sealed class IMethodSymbolExtensionsTests
{
    private const string TestSource = """
        public static class StaticMethods
        {
            public static void PublicParameterless() {}
            public static void PublicWithParams(int x, string y) {}
            private static void PrivateParameterless() {}
            public static T Generic<T>() => default!;
            public static void ExtensionMethod(this string s, int n) {}
            public static void StaticNonExtension(int x) {}
        }

        public class InstanceMethods
        {
            public void InstanceParameterless() {}
            public int InstanceWithReturn(double d) => 0;
            public int Field;
        }

        public interface IContract { void DoWork(int x); }
        public class Implementer : IContract { public void DoWork(int x) {} }
        """;

    private static async Task<(Compilation compilation, SyntaxTree tree)> CreateCompilationAsync(
        CancellationToken ct)
    {
        var tree = CSharpSyntaxTree.ParseText(TestSource, cancellationToken: ct);
        var test = new CSharpCompilationTest();
        var compilation = (await test.CreateCompilation(ct))!.AddSyntaxTrees([tree]);
        return (compilation, tree);
    }

    private static IMethodSymbol GetMethod(Compilation c, string typeName, string methodName)
    {
        return c.GetTypeByMetadataName(typeName)!
            .GetMembers(methodName)
            .OfType<IMethodSymbol>()
            .First();
    }

    [Test]
    public async Task IsPublicParameterlessMethod_PublicParameterless_ReturnsTrue(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicParameterless");
        await Assert.That(method.IsPublicParameterlessMethod()).IsTrue();
    }

    [Test]
    public async Task IsPublicParameterlessMethod_PublicWithParams_ReturnsFalse(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicWithParams");
        await Assert.That(method.IsPublicParameterlessMethod()).IsFalse();
    }

    [Test]
    public async Task IsPublicParameterlessMethod_PrivateParameterless_ReturnsFalse(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PrivateParameterless");
        await Assert.That(method.IsPublicParameterlessMethod()).IsFalse();
    }

    [Test]
    public async Task IsParameterlessNonGenericMethod_GenericMethod_ReturnsFalse(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "Generic");
        await Assert.That(method.IsParameterlessNonGenericMethod()).IsFalse();
    }

    [Test]
    public async Task IsParameterlessNonGenericMethod_ParameterlessNonGeneric_ReturnsTrue(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicParameterless");
        await Assert.That(method.IsParameterlessNonGenericMethod()).IsTrue();
    }

    [Test]
    public async Task GetParameterTypes_ReturnsCorrectTypes(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicWithParams");
        var types = method.GetParameterTypes();
        await Assert.That(types.Length).IsEqualTo(2);
        await Assert.That(types[0].SpecialType).IsEqualTo(SpecialType.System_Int32);
        await Assert.That(types[1].SpecialType).IsEqualTo(SpecialType.System_String);
    }

    [Test]
    public async Task GetParameterTypes_ParameterlessMethod_ReturnsEmpty(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicParameterless");
        var types = method.GetParameterTypes();
        await Assert.That(types).IsEmpty();
    }

    [Test]
    public async Task GetUsedSignatureTypeSymbols_IncludesReturnTypeAndParams(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "InstanceMethods", "InstanceWithReturn");
        // Returns int, has one double param
        var types = method.GetUsedSignatureTypeSymbols();
        await Assert.That(types.Length).IsEqualTo(2); // return type + 1 param
        await Assert.That(types[0].SpecialType).IsEqualTo(SpecialType.System_Int32); // return type first
        await Assert.That(types[1].SpecialType).IsEqualTo(SpecialType.System_Double);
    }

    [Test]
    public async Task IsParameterless_ForParameterlessMethod_ReturnsTrue(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicParameterless");
        await Assert.That(method.IsParameterless()).IsTrue();
    }

    [Test]
    public async Task IsParameterless_ForMethodWithParams_ReturnsFalse(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicWithParams");
        await Assert.That(method.IsParameterless()).IsFalse();
    }

    [Test]
    public async Task IsParameterlessAndNonGeneric_ForGenericParameterless_ReturnsFalse(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "Generic");
        await Assert.That(method.IsParameterlessAndNonGeneric()).IsFalse();
    }

    [Test]
    public async Task IsParameterlessAndNonGeneric_ForNonGenericParameterless_ReturnsTrue(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicParameterless");
        await Assert.That(method.IsParameterlessAndNonGeneric()).IsTrue();
    }

    [Test]
    public async Task GetExtensionMethodParameter_ForExtensionMethod_ReturnsFirstParam(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "ExtensionMethod");
        var param = method.GetExtensionMethodParameter();
        await Assert.That(param).IsNotNull();
        await Assert.That(param!.Type.SpecialType).IsEqualTo(SpecialType.System_String);
    }

    [Test]
    public async Task GetExtensionMethodParameter_ForNonExtensionMethod_ReturnsNull(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "StaticNonExtension");
        var param = method.GetExtensionMethodParameter();
        await Assert.That(param).IsNull();
    }

    [Test]
    public async Task GetActualReceiverType_ForExtensionMethod_ReturnsExtensionParamType(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "ExtensionMethod");
        var receiver = method.GetActualReceiverType();
        await Assert.That(receiver).IsNotNull();
        await Assert.That(receiver!.SpecialType).IsEqualTo(SpecialType.System_String);
    }

    [Test]
    public async Task GetActualReceiverType_ForStaticNonExtensionMethod_ReturnsNull(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "StaticMethods", "PublicParameterless");
        var receiver = method.GetActualReceiverType();
        await Assert.That(receiver).IsNull();
    }

    [Test]
    public async Task GetActualReceiverType_ForInstanceMethod_ReturnsContainingType(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var method = GetMethod(c, "InstanceMethods", "InstanceParameterless");
        var receiver = method.GetActualReceiverType();
        await Assert.That(receiver).IsNotNull();
        await Assert.That(receiver!.Name).IsEqualTo("InstanceMethods");
    }

    [Test]
    public async Task ImplementsInterface_ForImplementedMethod_ReturnsTrue(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var implementerType = c.GetTypeByMetadataName("Implementer")!;
        var method = implementerType.GetMembers("DoWork").OfType<IMethodSymbol>().First();
        var contractInterface = c.GetTypeByMetadataName("IContract") as INamedTypeSymbol;
        await Assert.That(method.ImplementsInterface(contractInterface)).IsTrue();
    }

    [Test]
    public async Task ImplementsInterface_ForNonImplementedInterface_ReturnsFalse(CancellationToken ct)
    {
        var (c, _) = await CreateCompilationAsync(ct);
        var implementerType = c.GetTypeByMetadataName("Implementer")!;
        var method = implementerType.GetMembers("DoWork").OfType<IMethodSymbol>().First();
        // IContract2 doesn't exist – test with null
        await Assert.That(method.ImplementsInterface(null)).IsFalse();
    }
}
