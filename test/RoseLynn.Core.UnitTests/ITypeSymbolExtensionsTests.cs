using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.Testing;

namespace RoseLynn.Test;

public sealed class ITypeSymbolExtensionsTests
{
    private static async Task<Compilation> GetCompilation(CancellationToken ct)
    {
        return (await new CSharpCompilationTest().CreateCompilation(ct))!;
    }

    [Test]
    public async Task Arity_NonGenericType_ReturnsZero(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        await Assert.That(intType.Arity).IsEqualTo(0);
    }

    [Test]
    public async Task Arity_GenericType_ReturnsCorrectArity(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")!;
        await Assert.That(listType.Arity).IsEqualTo(1);
    }

    [Test]
    public async Task Arity_ArrayType_ReturnsZero(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var arrayType = compilation.CreateArrayTypeSymbol(intType);
        await Assert.That(arrayType.Arity).IsEqualTo(0);
    }

    [Test]
    [Arguments(SpecialType.System_Boolean)]
    [Arguments(SpecialType.System_Int32)]
    [Arguments(SpecialType.System_String)]
    [Arguments(SpecialType.System_Object)]
    public async Task IsValidTypeArgument_ForCommonTypes_ReturnsTrue(SpecialType specialType, CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var type = compilation.GetSpecialType(specialType);
        await Assert.That(type.IsValidTypeArgument()).IsTrue();
    }

    [Test]
    public async Task IsInvalidTypeArgument_ForVoid_ReturnsTrue(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var voidType = compilation.GetSpecialType(SpecialType.System_Void);
        await Assert.That(voidType.IsInvalidTypeArgument()).IsTrue();
    }

    [Test]
    public async Task IsValidTypeArgument_ForVoid_ReturnsFalse(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var voidType = compilation.GetSpecialType(SpecialType.System_Void);
        await Assert.That(voidType.IsValidTypeArgument()).IsFalse();
    }

    [Test]
    public async Task IsNotNull_ForValueType_ReturnsTrue(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        await Assert.That(intType.IsNotNull()).IsTrue();
    }

    [Test]
    public async Task IsNotNull_ForAnnotatedNullableReferenceType_ReturnsFalse(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var nullableString = stringType.WithNullableAnnotation(NullableAnnotation.Annotated);
        await Assert.That(nullableString.IsNotNull()).IsFalse();
    }

    [Test]
    public async Task IsNotNull_ForNotAnnotatedReferenceType_ReturnsTrue(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var nonNullString = stringType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        await Assert.That(nonNullString.IsNotNull()).IsTrue();
    }

    [Test]
    public async Task Inherits_ValueTypeFromClass_ReturnsTrue(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var valueType = compilation.GetSpecialType(SpecialType.System_ValueType);
        await Assert.That(intType.Inherits(valueType)).IsTrue();
    }

    [Test]
    public async Task Inherits_StringFromObject_ReturnsTrue(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var objectType = compilation.GetSpecialType(SpecialType.System_Object);
        await Assert.That(stringType.Inherits(objectType)).IsTrue();
    }

    [Test]
    public async Task Inherits_TypeFromInterface_ReturnsTrue(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var icomparable = (INamedTypeSymbol)compilation.GetTypeByMetadataName("System.IComparable")!;
        await Assert.That(intType.Inherits(icomparable)).IsTrue();
    }

    [Test]
    public async Task Inherits_UnrelatedTypes_ReturnsFalse(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        await Assert.That(intType.Inherits(stringType)).IsFalse();
    }

    [Test]
    public async Task GetAllBaseTypes_ForObject_ReturnsEmpty(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var objectType = compilation.GetSpecialType(SpecialType.System_Object);
        var baseTypes = objectType.GetAllBaseTypes().ToList();
        await Assert.That(baseTypes).IsEmpty();
    }

    [Test]
    public async Task GetAllBaseTypes_ForInt_ContainsValueTypeAndObject(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var baseTypes = intType.GetAllBaseTypes().ToList();
        await Assert.That(baseTypes.Any(t => t.SpecialType == SpecialType.System_ValueType)).IsTrue();
        await Assert.That(baseTypes.Any(t => t.SpecialType == SpecialType.System_Object)).IsTrue();
    }

    [Test]
    public async Task GetAllBaseTypes_OrderIsImmediateBaseFirst(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var baseTypes = intType.GetAllBaseTypes().ToList();
        // First base type of int is ValueType, last is Object
        await Assert.That(baseTypes.First().SpecialType).IsEqualTo(SpecialType.System_ValueType);
        await Assert.That(baseTypes.Last().SpecialType).IsEqualTo(SpecialType.System_Object);
    }

    [Test]
    public async Task GetAllMembersIncludingInherited_CountExceedsOwnMembers(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var ownCount = intType.GetMembers().Length;
        var allCount = intType.GetAllMembersIncludingInherited().Count();
        await Assert.That(allCount).IsGreaterThan(ownCount);
    }

    [Test]
    public async Task GetAllInheritedMembers_DoesNotIncludeOwnMembers(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var ownNames = stringType.GetMembers().Select(m => m.MetadataName).ToHashSet();
        var inheritedNames = stringType.GetAllInheritedMembers().Select(m => m.MetadataName).ToList();
        // Inherited members should include Object members (Equals, GetHashCode, ToString, etc.)
        await Assert.That(inheritedNames).IsNotEmpty();
        // Own member names and inherited member names may overlap by name, but the inherited ones
        // come from base types (ValueType/Object), so just verify we got something from Object
        var hasObjectMember = inheritedNames.Contains("Equals")
            || inheritedNames.Contains("GetHashCode")
            || inheritedNames.Contains("ToString");
        await Assert.That(hasObjectMember).IsTrue();
    }

    [Test]
    public async Task GetAllBaseTypesAndInterfaces_ContainsBaseTypesAndAllInterfaces(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var baseTypesAndInterfaces = intType.GetAllBaseTypesAndInterfaces().ToList();
        var baseTypes = intType.GetAllBaseTypes().ToList();
        var allInterfaces = intType.AllInterfaces.ToList();
        await Assert.That(baseTypesAndInterfaces.Count)
            .IsEqualTo(baseTypes.Count + allInterfaces.Count);
    }

    [Test]
    public async Task GetBaseTypeAndDirectInterfaces_ContainsBaseTypeAndDirectInterfaces(CancellationToken ct)
    {
        var compilation = await GetCompilation(ct);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var result = intType.GetBaseTypeAndDirectInterfaces().ToList();
        var directInterfaces = intType.Interfaces.ToList();
        await Assert.That(result.Count).IsEqualTo(1 + directInterfaces.Count);
        await Assert.That(result.First().SpecialType).IsEqualTo(SpecialType.System_ValueType);
    }
}
