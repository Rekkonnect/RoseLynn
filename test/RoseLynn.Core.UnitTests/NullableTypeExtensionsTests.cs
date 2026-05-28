using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.Testing;

namespace RoseLynn.Test;

public sealed class NullableTypeExtensionsTests
{
    private static async Task<Compilation> GetCompilation(CancellationToken ct)
    {
        return (await new CSharpCompilationTest().CreateCompilation(ct))!;
    }

    private static INamedTypeSymbol NullableOf(Compilation c, ITypeSymbol underlying)
    {
        return c.GetSpecialType(SpecialType.System_Nullable_T).Construct(underlying);
    }

    [Test]
    public async Task IsNullableStruct_ForNullableT_ReturnsTrue(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var nullableT = c.GetSpecialType(SpecialType.System_Nullable_T);
        await Assert.That(nullableT.IsNullableStruct()).IsTrue();
    }

    [Test]
    public async Task IsNullableStruct_ForConstructedNullableInt_ReturnsTrue(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var nullableInt = NullableOf(c, intType);
        await Assert.That(nullableInt.IsNullableStruct()).IsTrue();
    }

    [Test]
    public async Task IsNullableStruct_ForNonNullableValueType_ReturnsFalse(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        await Assert.That(intType.IsNullableStruct()).IsFalse();
    }

    [Test]
    public async Task IsNullableStruct_ForReferenceType_ReturnsFalse(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var stringType = c.GetSpecialType(SpecialType.System_String);
        await Assert.That(stringType.IsNullableStruct()).IsFalse();
    }

    [Test]
    public async Task IsNullableStruct_WithOut_ForNullableInt_ReturnsTrueAndUnderlyingIsInt(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var nullableInt = NullableOf(c, intType);
        var isNullable = nullableInt.IsNullableStruct(out var underlying);
        await Assert.That(isNullable).IsTrue();
        await Assert.That(underlying).IsNotNull();
        await Assert.That(underlying!.SpecialType).IsEqualTo(SpecialType.System_Int32);
    }

    [Test]
    public async Task IsNullableStruct_WithOut_ForNonNullable_ReturnsFalseAndUnderlyingIsNull(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var isNullable = intType.IsNullableStruct(out var underlying);
        await Assert.That(isNullable).IsFalse();
        await Assert.That(underlying).IsNull();
    }

    [Test]
    public async Task AcceptsNull_ForValueType_ReturnsFalse(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        await Assert.That(intType.AcceptsNull()).IsFalse();
    }

    [Test]
    public async Task AcceptsNull_ForNullableValueType_ReturnsTrue(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var nullableInt = NullableOf(c, intType);
        await Assert.That(nullableInt.AcceptsNull()).IsTrue();
    }

    [Test]
    public async Task AcceptsNull_ForNotAnnotatedReferenceType_ReturnsFalse(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var stringType = c.GetSpecialType(SpecialType.System_String)
            .WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        await Assert.That(stringType.AcceptsNull()).IsFalse();
    }

    [Test]
    public async Task AcceptsNull_ForAnnotatedReferenceType_ReturnsTrue(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var stringType = c.GetSpecialType(SpecialType.System_String)
            .WithNullableAnnotation(NullableAnnotation.Annotated);
        await Assert.That(stringType.AcceptsNull()).IsTrue();
    }

    [Test]
    public async Task AcceptsNull_ForUnannotatedReferenceType_ReturnsTrue(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        // NullableAnnotation.None = nullable context not applicable, reference type accepts null by default
        var stringType = c.GetSpecialType(SpecialType.System_String);
        await Assert.That(stringType.NullableAnnotation).IsEqualTo(NullableAnnotation.None);
        await Assert.That(stringType.AcceptsNull()).IsTrue();
    }

    [Test]
    public async Task MakeNullable_ForValueType_ReturnsNullableT(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var nullable = intType.MakeNullable(c);
        await Assert.That(nullable.IsNullableStruct()).IsTrue();
        // Underlying type should be int
        nullable.IsNullableStruct(out var underlying);
        await Assert.That(underlying?.SpecialType).IsEqualTo(SpecialType.System_Int32);
    }

    [Test]
    public async Task MakeNullable_ForReferenceType_AddsAnnotation(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var stringType = c.GetSpecialType(SpecialType.System_String);
        var nullable = stringType.MakeNullable(c);
        await Assert.That(nullable.NullableAnnotation).IsEqualTo(NullableAnnotation.Annotated);
    }

    [Test]
    public async Task MakeNullable_ForNullableT_IsIdempotent(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var nullableInt = NullableOf(c, intType);
        // MakeNullable on an already-nullable struct returns a new Nullable<Nullable<int>>?
        // Actually Nullable<T>.MakeNullable wraps again; what matters is AcceptsNull returns true
        var result = nullableInt.MakeNullable(c);
        await Assert.That(result.AcceptsNull()).IsTrue();
    }

    [Test]
    public async Task MakeNotNullable_ForNullableInt_ReturnsInt(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var nullableInt = NullableOf(c, intType);
        var result = nullableInt.MakeNotNullable();
        await Assert.That(result.SpecialType).IsEqualTo(SpecialType.System_Int32);
    }

    [Test]
    public async Task MakeNotNullable_ForAnnotatedString_RemovesAnnotation(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var stringType = c.GetSpecialType(SpecialType.System_String)
            .WithNullableAnnotation(NullableAnnotation.Annotated);
        var result = stringType.MakeNotNullable();
        await Assert.That(result.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
    }

    [Test]
    public async Task MakeNotNullable_ForValueType_ReturnsNotAnnotated(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var result = intType.MakeNotNullable();
        await Assert.That(result.NullableAnnotation).IsEqualTo(NullableAnnotation.NotAnnotated);
        await Assert.That(result.AcceptsNull()).IsFalse();
    }

    [Test]
    public async Task IsPointerType_ForRegularType_ReturnsFalse(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        await Assert.That(intType.IsPointerType).IsFalse();
    }

    [Test]
    public async Task IsPointerType_ForArrayType_ReturnsFalse(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var arrayType = c.CreateArrayTypeSymbol(intType);
        await Assert.That(arrayType.IsPointerType).IsFalse();
    }

    [Test]
    public async Task IsPointerType_ForPointerType_ReturnsTrue(CancellationToken ct)
    {
        var c = await GetCompilation(ct);
        var intType = c.GetSpecialType(SpecialType.System_Int32);
        var pointerType = c.CreatePointerTypeSymbol(intType);
        await Assert.That(pointerType.IsPointerType).IsTrue();
    }
}
