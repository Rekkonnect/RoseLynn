#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltimateSymbolTest;

using IntArray = int[];
using IntListArray = List<int>[];
using FunctionPointer = delegate* unmanaged<string, void>;
using FunctionPointerWithCustomCallConventions
    = delegate* unmanaged[Cdecl, Thiscall]<string, void>;

// ===================== CLASSES =====================
/// <summary>Basic public class</summary>
public class PublicClass { }

/// <summary>Abstract class</summary>
public abstract class AbstractClass
{
    /// <summary>Abstract method</summary>
    public abstract void AbstractMethod();

    /// <summary>Virtual method</summary>
    public virtual void VirtualMethod() { }
}

/// <summary>Sealed class</summary>
public sealed class SealedClass { }

/// <summary>Static class</summary>
public static class StaticClass
{
    /// <summary>Static method</summary>
    public static void StaticMethod() { }

    /// <summary>Static property</summary>
    public static string? StaticProperty { get; set; }
}

/// <summary>Partial class part 1</summary>
public partial class PartialClass { }

/// <summary>Partial class part 2</summary>
public partial class PartialClass { }

/// <summary>Generic class</summary>
public class GenericClass<T> { }

/// <summary>Multi-parameter generic</summary>
public class MultiGeneric<T, U, V> { }

/// <summary>Constrained generic</summary>
public class ConstrainedGeneric<T> where T : class { }

/// <summary>Multiple constraint generic</summary>
public class MultiConstrainedGeneric<T> where T : class, new() { }

// ===================== STRUCTS =====================
/// <summary>Value type struct</summary>
public struct ValueStruct { }

/// <summary>Record struct</summary>
public record struct RecordStruct { }

/// <summary>Readonly struct</summary>
public readonly struct ReadonlyStruct { }

/// <summary>Ref struct</summary>
public ref struct RefStruct { }

/// <summary>Readonly ref struct</summary>
public readonly ref struct ReadOnlyRefStruct { }

[StructLayout(LayoutKind.Sequential, Size = 8, Pack = 8)]
public struct StructLayoutExample;

// ===================== INTERFACES =====================
/// <summary>Basic interface</summary>
public interface IBasicInterface { }

/// <summary>Generic interface</summary>
public interface IGenericInterface<T> { }

/// <summary>Covariant interface</summary>
public interface ICovariant<out T> { }

/// <summary>Contravariant interface</summary>
public interface IContravariant<in T> { }

// ===================== RECORDS =====================
/// <summary>Basic record</summary>
public record BasicRecord { }

/// <summary>Positional record</summary>
public record PositionalRecord(string Name, int Age);

/// <summary>Record with properties</summary>
public record RecordWithProps
{
    /// <summary>Property</summary>
    public string? Name { get; init; }
}

// ===================== ENUMS =====================
/// <summary>Basic enum</summary>
public enum BasicEnum
{
    /// <summary>First value</summary>
    First,
    /// <summary>Second value</summary>
    Second
}

/// <summary>Flag enum</summary>
[Flags]
public enum FlagEnum
{
    /// <summary>None</summary>
    None = 0,
    /// <summary>Flag1</summary>
    Flag1 = 1,
    /// <summary>Flag2</summary>
    Flag2 = 2
}

// ===================== DELEGATES =====================
/// <summary>Action delegate</summary>
public delegate void SimpleDelegate();

/// <summary>Generic delegate</summary>
public delegate T GenericDelegate<T>();

/// <summary>Delegate with parameters</summary>
public delegate bool PredicateDelegate<T>(T item);

// ===================== PROPERTIES =====================
public abstract class PropertyExamples
{
    /// <summary>Auto-property</summary>
    public string? AutoProperty { get; set; }

    /// <summary>Read-only auto-property</summary>
    public string? ReadOnlyAutoProperty { get; }

    /// <summary>Init-only property</summary>
    public string? InitProperty { get; init; }

    /// <summary>Computed property</summary>
    public int ComputedProperty => 42;

    /// <summary>Property with backing field</summary>
    private string? _backingField;
    public string? PropertyWithBacking
    {
        get => _backingField;
        set => _backingField = value;
    }

    /// <summary>Virtual property</summary>
    public virtual string? VirtualProperty { get; set; }

    /// <summary>Abstract property</summary>
    public abstract string? AbstractProperty { get; set; }

    /// <summary>Required property</summary>
    public required string RequiredProperty { get; init; }

    /// <summary>Nullable reference type property</summary>
    public string? NullableProperty { get; set; }

    /// <summary>Nullable value type property</summary>
    public int? NullableIntProperty { get; set; }

    /// <summary>Non-nullable property</summary>
    public string NonNullableProperty { get; set; } = string.Empty;
}

// ===================== METHODS =====================
public partial class MethodExamples
{
    /// <summary>Simple method</summary>
    public void SimpleMethod() { }

    /// <summary>Method with parameters</summary>
    public void MethodWithParams(string a, int b, bool c) { }

    /// <summary>Method with return type</summary>
    public string? MethodWithReturn() => default;

    /// <summary>Generic method</summary>
    public T GenericMethod<T>() => default!;

    /// <summary>Generic method with constraints</summary>
    public T ConstrainedMethod<T>() where T : class, new() => new();

    /// <summary>Method with ref parameter</summary>
    public void RefParameter(ref string value) { }

    /// <summary>Method with out parameter</summary>
    public void OutParameter(out string value) { value = string.Empty; }

    /// <summary>Method with in parameter</summary>
    public void InParameter(in string value) { }

    /// <summary>Method with scoped parameter</summary>
    public void ScopedParameter(scoped string value) { }

    /// <summary>Method with scoped ref parameter</summary>
    public void ScopedRefParameter(scoped ref string value) { }

    /// <summary>Method with params</summary>
    public void ParamsMethod(params string[] values) { }

    /// <summary>Async method</summary>
    public async Task AsyncMethod() { }

    /// <summary>Async method with return</summary>
    public async Task<int> AsyncMethodWithReturn() => 42;

    /// <summary>Yieldable method</summary>
    public IEnumerable<int> Yieldable()
    {
        yield return 1;
        yield return 2;
        yield return 3;
    }

    /// <summary>Discard symbol</summary>
    public void TestDiscard()
    {
        _ = string.TryParse("123", out _);
    }

    /// <summary>Range variable symbol</summary>
    public void TestRangeVariable()
    {
        var result =
            from x in new[] { 1, 2 }
            where x > 1
            select x;
    }

    /// <summary>Static method</summary>
    public static void StaticMethod() { }

    /// <summary>Virtual method</summary>
    public virtual void VirtualMethod() { }

    /// <summary>Override method</summary>
    public override string ToString() => base.ToString()!;

    /// <summary>Method with optional parameters</summary>
    public void OptionalParams(string a, int b = 42, bool c = true) { }

    /// <summary>Method with named parameters</summary>
    public void NamedParamMethod(string name, int age) { }

    /// <summary>Partial method declaration</summary>
    public partial void PartialMethod();

    /// <summary>Partial method implementation</summary>
    public partial void PartialMethod()
    {
    }

    /// <summary>Local function</summary>
    public void MethodWithLocal()
    {
        void LocalFunction() { }
        LocalFunction();
    }
}

public static partial class StaticMethodExamples
{
    /// <summary>Extension method</summary>
    public static void ExtensionMethod(this string value) { }
}

// ===================== EVENTS =====================
public class EventExamples
{
    /// <summary>Simple event</summary>
    public event EventHandler? SimpleEvent;

    /// <summary>Event with custom delegate</summary>
    public event SimpleDelegate? CustomDelegateEvent;

    /// <summary>Event with add/remove</summary>
    private EventHandler? _customEvent;
    public event EventHandler? CustomEvent
    {
        add => _customEvent += value;
        remove => _customEvent -= value;
    }

    /// <summary>Static event</summary>
    public static event EventHandler? StaticEvent;
}

// ===================== INDEXERS =====================
public abstract class IndexerExamples
{
    /// <summary>Integer indexer</summary>
    public abstract string? this[int index] { get; set; }

    /// <summary>String indexer</summary>
    public abstract string? this[string key] { get; set; }

    /// <summary>Multi-parameter indexer</summary>
    public abstract string? this[int x, int y] { get; set; }
}

// ===================== OPERATORS =====================
public class OperatorExamples
{
    /// <summary>Unary plus</summary>
    public static OperatorExamples operator +(OperatorExamples a) => a;

    /// <summary>Unary minus</summary>
    public static OperatorExamples operator -(OperatorExamples a) => a;

    /// <summary>Binary addition</summary>
    public static OperatorExamples operator +(OperatorExamples a, OperatorExamples b) => a;

    /// <summary>Binary subtraction</summary>
    public static OperatorExamples operator -(OperatorExamples a, OperatorExamples b) => a;

    /// <summary>Multiplication</summary>
    public static OperatorExamples operator *(OperatorExamples a, int b) => a;

    /// <summary>Division</summary>
    public static OperatorExamples operator /(OperatorExamples a, int b) => a;

    /// <summary>Modulo</summary>
    public static OperatorExamples operator %(OperatorExamples a, int b) => a;

    /// <summary>Equality</summary>
    public static bool operator ==(OperatorExamples? a, OperatorExamples? b) => ReferenceEquals(a, b);

    /// <summary>Inequality</summary>
    public static bool operator !=(OperatorExamples? a, OperatorExamples? b) => !ReferenceEquals(a, b);

    /// <summary>Less than</summary>
    public static bool operator <(OperatorExamples? a, OperatorExamples? b) => false;

    /// <summary>Greater than</summary>
    public static bool operator >(OperatorExamples? a, OperatorExamples? b) => false;

    /// <summary>Less than or equal</summary>
    public static bool operator <=(OperatorExamples? a, OperatorExamples? b) => true;

    /// <summary>Greater than or equal</summary>
    public static bool operator >=(OperatorExamples? a, OperatorExamples? b) => true;

    /// <summary>Bitwise AND</summary>
    public static OperatorExamples operator &(OperatorExamples a, OperatorExamples b) => a;

    /// <summary>Bitwise OR</summary>
    public static OperatorExamples operator |(OperatorExamples a, OperatorExamples b) => a;

    /// <summary>Bitwise XOR</summary>
    public static OperatorExamples operator ^(OperatorExamples a, OperatorExamples b) => a;

    /// <summary>Bitwise NOT</summary>
    public static OperatorExamples operator ~(OperatorExamples a) => a;

    /// <summary>Left shift</summary>
    public static OperatorExamples operator <<(OperatorExamples a, int b) => a;

    /// <summary>Right shift</summary>
    public static OperatorExamples operator >>(OperatorExamples a, int b) => a;

    /// <summary>Logical NOT</summary>
    public static bool operator !(OperatorExamples a) => false;

    /// <summary>Implicit conversion</summary>
    public static implicit operator string(OperatorExamples a) => string.Empty;

    /// <summary>Explicit conversion</summary>
    public static explicit operator int(OperatorExamples a) => 0;

    /// <summary>Checked increment</summary>
    public static OperatorExamples operator checked ++(OperatorExamples a) => a;

    /// <summary>Checked decrement</summary>
    public static OperatorExamples operator checked --(OperatorExamples a) => a;

    /// <summary>Unchecked increment</summary>
    public static OperatorExamples operator ++(OperatorExamples a) => a;

    /// <summary>Unchecked decrement</summary>
    public static OperatorExamples operator --(OperatorExamples a) => a;
}

// ===================== FIELDS =====================
public class FieldExamples
{
    /// <summary>Public field</summary>
    public string? PublicField;

    /// <summary>Private field</summary>
    private string? _privateField;

    /// <summary>Protected field</summary>
    protected string? ProtectedField;

    /// <summary>Internal field</summary>
    internal string? InternalField;

    /// <summary>Protected internal field</summary>
    protected internal string? ProtectedInternalField;

    /// <summary>Static field</summary>
    public static string? StaticField;

    /// <summary>Readonly field</summary>
    public readonly string? ReadonlyField;

    /// <summary>Static readonly field</summary>
    public static readonly string? StaticReadonlyField;

    /// <summary>Volatile field</summary>
    public volatile int VolatileField;

    /// <summary>Const field</summary>
    public const int ConstField = 42;
}

// ===================== NESTED TYPES =====================
public class NestedContainer
{
    /// <summary>Nested class</summary>
    public class NestedClass { }

    /// <summary>Nested struct</summary>
    public struct NestedStruct { }

    /// <summary>Nested interface</summary>
    public interface INestedInterface { }

    /// <summary>Nested enum</summary>
    public enum NestedEnum { Value }

    /// <summary>Nested delegate</summary>
    public delegate void NestedDelegate();

    /// <summary>Nested record</summary>
    public record NestedRecord { }

    /// <summary>Generic nested class</summary>
    public class GenericNested<T> { }
}

// ===================== TYPE VARIATIONS =====================

/// <summary>With nullable reference types</summary>
public class NullableRefTypes
{
    /// <summary>Nullable string</summary>
    public string? NullableString { get; set; }

    /// <summary>Non-nullable string</summary>
    public string NonNullableString { get; set; } = string.Empty;

    /// <summary>Method with nullable parameters</summary>
    public void Method(string? nullable, string nonNullable) { }

    /// <summary>Nullable return type</summary>
    public string? GetNullableString() => default;

    /// <summary>Collection of nullables</summary>
    public List<string?>? NullableCollection { get; set; }
}

/// <summary>With toggled nullability</summary>
public class NullableToggling
{
#nullable disable

    public string? ObliviouslyNullableString { get; set; }
    public string ObliviouslyNonNullableString { get; set; }

#nullable enable

    public string? NullableString { get; set; }
    public string NonNullableString { get; set; }
}

/// <summary>Tuple examples</summary>
public class TupleExamples
{
    /// <summary>Named tuple property</summary>
    public (string Name, int Age) NamedTuple { get; set; }

    /// <summary>Tuple method return</summary>
    public (int X, int Y, int Z) GetTuple() => (1, 2, 3);

    /// <summary>Tuple parameters</summary>
    public void ProcessTuple((string Key, object? Value) data) { }
}

/// <summary>Pattern matching</summary>
public class PatternMatching
{
    /// <summary>Type pattern method</summary>
    public void TypePattern(object o)
    {
        if (o is string s) { }
    }

    /// <summary>Property pattern method</summary>
    public void PropertyPattern(object o)
    {
        if (o is PatternMatching { IsSuccess: true }) { }
        if (o is not null) { }
    }

    /// <summary>List pattern method</summary>
    public void ListPattern(int[] arr)
    {
        if (arr is [1, 2, 3]) { }
    }

    public bool IsSuccess { get; set; }
}

/// <summary>Collection types</summary>
public class CollectionTypes
{
    /// <summary>List property</summary>
    public List<string>? StringList { get; set; }

    /// <summary>Dictionary property</summary>
    public Dictionary<string, int>? StringIntDict { get; set; }

    /// <summary>Span property</summary>
    public Span<int> IntSpan => MemoryMarshal.CreateSpan(ref Unsafe.NullRef<int>(), 0);

    /// <summary>ReadOnlySpan property</summary>
    public ReadOnlySpan<int> ReadOnlyIntSpan => [1];

    /// <summary>Array property</summary>
    public string?[]? StringArray { get; set; }

    /// <summary>Jagged array property</summary>
    public int[][]? JaggedArray { get; set; }

    /// <summary>Multidimensional array property</summary>
    public string?[,]? MultiArray { get; set; }
}

/// <summary>Attributes test</summary>
[Obsolete("This is obsolete")]
public class AttributedClass
{
    /// <summary>Method with multiple attributes</summary>
    [Obsolete]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AttributedMethod() { }

    /// <summary>Property with attributes</summary>
    [System.ComponentModel.DefaultValue(null)]
    public string? AttributedProperty { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class CustomAttribute : Attribute;

/// <summary>Access modifiers variety</summary>
public class AccessModifiers
{
    /// <summary>Public member</summary>
    public void PublicMethod() { }

    /// <summary>Private member</summary>
    private void PrivateMethod() { }

    /// <summary>Protected member</summary>
    protected void ProtectedMethod() { }

    /// <summary>Internal member</summary>
    internal void InternalMethod() { }

    /// <summary>Protected internal member</summary>
    protected internal void ProtectedInternalMethod() { }

    /// <summary>Private protected member</summary>
    private protected void PrivateProtectedMethod() { }
}

/// <summary>Using declarations and statements</summary>
public class UsingExamples
{
    /// <summary>Method using IDisposable</summary>
    public void DisposableExample()
    {
        using (var disposable = new StringWriter()) { }
    }

    /// <summary>IAsyncDisposable example</summary>
    public async Task AsyncDisposableExample()
    {
        await using (var asyncDisposable = (IAsyncDisposable?)null!) { }
    }
}

/// <summary>Foreach statements</summary>
public class ForeachExamples
{
    public void ForeachSingle()
    {
        foreach (int a in [1, 2]) { }
    }

    public void ForeachTuple()
    {
        foreach (var (a, b) in [[1, 2], [3, 4]]) { }
    }
}

/// <summary>Exception handling</summary>
public class ExceptionHandling
{
    /// <summary>Method with throws documentation</summary>
    /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
    /// <exception cref="InvalidOperationException">Thrown on invalid state</exception>
    public void ThrowingMethod(string? value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
    }
}

/// <summary>Default implementations on interface</summary>
public interface IDefaultImplementations
{
    /// <summary>Default method</summary>
    sealed void DefaultMethod() { }

    /// <summary>Default property</summary>
    sealed int DefaultProperty => 42;
}

/// <summary>Static abstract/virtual members</summary>
public interface IStaticAbstract
{
    /// <summary>Static abstract method</summary>
    static abstract void StaticAbstractMethod();

    /// <summary>Static virtual method</summary>
    static virtual void StaticVirtualMethod() { }

    /// <summary>Static abstract property</summary>
    static abstract string StaticAbstractProperty { get; }

    /// <summary>Static virtual property</summary>
    static virtual string StaticVirtualProperty => string.Empty;
}

/// <summary>Constraints and bounds</summary>
public class ConstraintExamples<
    T1,
    T2> where T1 : struct
    where T2 : class
{
    /// <summary>Method with multiple constraints</summary>
    public void ConstrainedMethod<U1, U2>(U1 item1, U2 item2)
        where U1 : notnull
        where U2 : IDisposable
    {
    }

    /// <summary>Generic method with unmanaged constraint</summary>
    public void UnmanagedMethod<U>() where U : unmanaged { }

    /// <summary>Generic method with Delegate constraint</summary>
    public void DelegateConstrainedMethod<U>() where U : Delegate { }

    /// <summary>Generic method with Enum constraint</summary>
    public void EnumConstrainedMethod<U>() where U : Enum { }
}

/// <summary>Fixed buffer field</summary>
public unsafe class FixedBufferExamples
{
    /// <summary>Fixed buffer field</summary>
    private fixed int _fields[3];
}

/// <summary>Regular pointers</summary>
public unsafe class PointerExamples
{
    /// <summary>Pointer field</summary>
    public unsafe int** DoublePointer;

    /// <summary>Method returning pointer</summary>
    [return: NotNull]
    public unsafe int** GetHandler() => null;
}

/// <summary>Function pointers</summary>
public unsafe class FunctionPointerExamples
{
    /// <summary>Function pointer field</summary>
    public unsafe delegate*<int, int, int> FunctionPointer;

    /// <summary>Method returning function pointer</summary>
    public unsafe FunctionPointer GetStringHandler() => null;
}

/// <summary>Records with inheritance</summary>
public record BaseRecord { }

/// <summary>Derived record</summary>
public record DerivedRecord(string Name) : BaseRecord { }

/// <summary>Method overloading</summary>
public class Overloading
{
    /// <summary>Method overload 1</summary>
    public void Overload() { }

    /// <summary>Method overload 2</summary>
    public void Overload(string s) { }

    /// <summary>Method overload 3</summary>
    public void Overload(int i) { }

    /// <summary>Method overload 4</summary>
    public void Overload(string s, int i) { }

    /// <summary>Generic overload</summary>
    public void Overload<T>(T item) { }

    /// <summary>Generic overload with constraint</summary>
    public void Overload<T>(T item, int distinct)
        where T : class
    { }
}

/// <summary>Type parameter variance in complex scenarios</summary>
public interface IComplexVariance<out TOut, in TIn>
{
    /// <summary>Covariant usage</summary>
    TOut GetValue();

    /// <summary>Contravariant usage</summary>
    void SetValue(TIn value);
}

/// <summary>Named and optional parameters variety</summary>
public class ParameterVariety
{
    /// <summary>All optional parameters</summary>
    public void AllOptional(int a = 1, string b = "default", bool c = true) { }

    /// <summary>Mixed required and optional</summary>
    public void MixedParams(string required, int optional1 = 0, bool optional2 = false) { }

    /// <summary>With default expressions</summary>
    public void DefaultExpressions(int[]? arr = null, List<string>? list = null) { }
}

/// <summary>Init-only setter scenarios</summary>
public record InitOnlyRecord
{
    /// <summary>Init-only property</summary>
    public string Property { get; init; } = string.Empty;

    /// <summary>Required init property</summary>
    public required string RequiredProperty { get; init; }
}

/// <summary>Compiler-generated members</summary>
#pragma warning disable CS0169
public class CompilerGenerated
{
    /// <summary>Private backing field</summary>
    private string _field = string.Empty;

    /// <summary>Auto-property generates backing field</summary>
    public string Property { get; set; } = string.Empty;

    /// <summary>Init-only property</summary>
    public string InitProperty { get; init; } = string.Empty;
}
#pragma warning restore CS0169

// ===================== PREPROCESSING =====================

#if SOME_SYMBOL
#elif ANOTHER_SYMBOL
#elif LAST_SYMBOL && SOMETHING_ELSE || DEFAULT
#else
#endif
