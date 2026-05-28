using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.Testing;
using System.Diagnostics.CodeAnalysis;

namespace RoseLynn.Test;

public class TypeMemoryHelpers_SizeOf_Tests
{
    private static readonly IReadOnlyList<MetadataReference> _metadataReferences
        = LoadMetadataReferences();

    [Test]
    public async Task StructWithAutoLayout(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Auto, Size = 10)]
            public struct S
            {
                public long A, B, C;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithExplicitLayout(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Explicit, Size = 10)]
            public struct S
            {
                [FieldOffset(5)]
                public long A, B, C;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithExplicitLayoutAndOffsetsOverManyLongsPacked(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Explicit, Size = 10, Pack = 16)]
            public struct S
            {
                [FieldOffset(0)]
                public long A, B, C;
                [FieldOffset(5)]
                public long A1, B1, C1;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithSequentialLayoutAndPackIntBeforeLongFields(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Size = 0, Pack = 128)]
            public struct S
            {
                public int I;
                public long A, B, C;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithSequentialLayoutAndPackLongBeforeIntFields(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Size = 0, Pack = 128)]
            public struct S
            {
                public long A, B, C;
                public int I;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithAutoLayoutWithTwoBytesAndInt(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            public struct S
            {
                public byte b1;
                public byte b2;
                public int i3;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithSequentialLayoutWithTwoBytesAndIntPack2(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Pack = 2)]
            public struct S
            {
                public byte b1;
                public byte b2;
                public int i3;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithSequentialLayoutWithTwoBytesAndIntPack4(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            public struct S
            {
                public byte b1;
                public byte b2;
                public int i3;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithSequentialLayoutAndFixedByte1DecimalPack16(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Pack = 16)]
            public unsafe struct S
            {
                public byte b1;
                public byte b2;
                public int i3;
                public fixed byte a4[1];
                public decimal d5;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithSequentialLayoutAndFixedByte2DecimalPack16(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Pack = 16)]
            public unsafe struct S
            {
                public byte b1;
                public byte b2;
                public int i3;
                public fixed byte a4[2];
                public decimal d5;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task StructWithSequentialLayoutAndFixedByte12DecimalPack16(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Pack = 16)]
            public unsafe struct S
            {
                public byte b1;
                public byte b2;
                public int i3;
                public fixed byte a4[12];
                public decimal d5;
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task EmptySequentialStruct(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Pack = 16)]
            public unsafe struct S
            {
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task EmptySequentialStructWithSize1(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential, Size = 1)]
            public unsafe struct S
            {
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task SequentialWithNestedOfIntsAtFirst(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct S
            {
                private T _t;
                private int _extra0;
                private byte _extra1;
                private byte _extra2;
                private int _extra3;

                private record struct T(int A, int B);
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task SequentialWithNestedOfLongIntAtFirst(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct S
            {
                private T _t;
                private int _extra0;
                private byte _extra1;
                private byte _extra2;
                private int _extra3;

                private record struct T(long A, int B);
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task SequentialWithFixedBufferAndNestedFixedBufferStruct(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct S
            {
                private T t;
                private fixed byte a[3];
                private byte b;

                private unsafe struct T
                {
                    public fixed byte s[3];
                }
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task SequentialWithInlineArrayStruct(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct S
            {
                private T t;
                private fixed byte a[3];
                private byte b;

                [InlineArray(3)]
                private unsafe struct T
                {
                    private byte _element0;
                }
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task SequentialWithInlineArrayOfIntsStruct(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct S
            {
                private T t;
                private fixed byte a[3];
                private byte b;

                [InlineArray(3)]
                private unsafe struct T
                {
                    private int _element0;
                }
            }
            """,
            "S",
            cancellationToken);
    }

    [Test]
    public async Task RecordStructsWithNestedContainers(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            public readonly record struct R1(R2 R2, R3 R3, string? Augment);
            public readonly record struct R2(R3 R3);
            public readonly record struct R3(string Key, int Value);
            """,
            "R1",
            cancellationToken);
    }

    [Test]
    public async Task NestedStructsWithTuples(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            public readonly record struct R1(R2 R2, (R3 R3, string? Augment) NestedExample);
            public readonly record struct R2(R3 R3);
            public readonly record struct R3((string Key, int Value) Tuple, (int, int, int, int) Point4D);
            """,
            "R1",
            cancellationToken);
    }

    [Test]
    public async Task StructWithRefFields(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            public ref struct R(ref byte r, ref byte s)
            {
                private readonly ref byte r = ref r;
                private readonly ref byte s = ref s;
            }
            """,
            "R",
            cancellationToken);
    }

    [Test]
    public async Task StructWithRefFieldsOfHeavyTypes(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            public ref struct R(ref R.Large l, ref R.Large ll)
            {
                private readonly ref Large l = ref l;
                private readonly ref Large ll = ref ll;

                public record struct Large(double A, double B, double C, double D);
            }
            """,
            "R",
            cancellationToken);
    }

    [Test]
    public async Task EnumDefault(
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            """
            public enum E
            {
                None,
                Something,
            }
            """,
            "E",
            cancellationToken);
    }

    [Test]
    [Arguments("byte")]
    [Arguments("short")]
    [Arguments("int")]
    [Arguments("long")]
    [Arguments("sbyte")]
    [Arguments("ushort")]
    [Arguments("uint")]
    [Arguments("ulong")]
    public async Task EnumWithSpecifiedType(
        string underlyingType,
        CancellationToken cancellationToken)
    {
        await EvaluateDeclaredType(
            $$"""
            public enum E : {{underlyingType}}
            {
                None,
                Something,
            }
            """,
            "E",
            cancellationToken);
    }

    private static async Task EvaluateDeclaredType(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string declarations,
        string testedTypeName,
        CancellationToken cancellationToken)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);

        var source = $$"""
            using System;
            using System.Runtime.CompilerServices;
            using System.Runtime.InteropServices;

            unsafe
            {
                return sizeof({{testedTypeName}});
            }

            {{declarations}}
            """;

        var result = await CreateRunCompilationRetrieveResult(parseOptions, source, cancellationToken);
        if (result.RunResult is not int runtimeSizeOf)
        {
            return;
        }

        var declaration = result.Compilation
            .GetSymbolsWithName(testedTypeName, cancellationToken: cancellationToken)
            .FirstOrDefault()
                as INamedTypeSymbol;
        await Assert.That(declaration).IsNotNull();
        var estimatedSizeOf = TypeMemoryHelpers.SizeOf(declaration!);

        await Assert.That(estimatedSizeOf).IsEqualTo(runtimeSizeOf);
    }

    private static async Task<AssemblyCompileRunResult> CreateRunCompilationRetrieveResult(
        CSharpParseOptions parseOptions,
        string source,
        CancellationToken cancellationToken)
    {
        var tree = CSharpSyntaxTree.ParseText(
            source,
            options: parseOptions,
            cancellationToken: cancellationToken);
        var options = new CSharpCompilationOptions(
            OutputKind.ConsoleApplication,
            allowUnsafe: true);
        var compilation = CSharpCompilation.Create(
            null,
            [tree],
            _metadataReferences,
            options);

        var peStream = new MemoryStream();
        var emitResult = compilation.Emit(peStream, cancellationToken: cancellationToken);
        await Assert.That(emitResult.Success).IsTrue();

        peStream.Seek(0, SeekOrigin.Begin);

        using var _ = AssemblyLoadContextEx.CreateDisposableContext(out var context);
        var assembly = context.LoadFromStream(peStream);
        var runtimeSizeOfObject = assembly.EntryPoint!.Invoke(null, [(string[])[]]);

        return new(compilation, runtimeSizeOfObject);
    }

    private static IReadOnlyList<MetadataReference> LoadMetadataReferences()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();
    }

    private sealed record AssemblyCompileRunResult(
        Compilation Compilation,
        object? RunResult);
}
