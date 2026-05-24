using Microsoft.CodeAnalysis;
using RoseLynn.CSharp;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RoseLynn;

public static partial class TypeMemoryHelpers
{
    public static int SizeOf(ITypeSymbol type)
    {
        var calculator = new SizeOfCalculator();
        return calculator.SizeOf(type).TotalSize;
    }

    private sealed class SizeOfCalculator
    {
        private readonly Dictionary<ITypeSymbol, SizeOfInfo> _sizes = new(SymbolEqualityComparer.Default);

        public SizeOfInfo SizeOf(ITypeSymbol type)
        {
            var primitiveSize = PrimitiveSize(type);
            if (primitiveSize is not null)
            {
                var size = primitiveSize.Value;
                return new(size, size);
            }

            // This is THE fucking hack of all the fucking hacks
            // Gotta figure this out somehow, decimal doesn't have any fields
            // but it gets treated as though it does
            if (type is { SpecialType: SpecialType.System_Decimal })
            {
                return new(sizeof(decimal), 8);
            }

            if (type.IsReferenceType || type is IPointerTypeSymbol)
            {
                return SizeOfPointer();
            }

            return GetOrCalculateSizeOf(type);
        }

        private static unsafe SizeOfInfo SizeOfPointer()
        {
            var size = sizeof(nint);
            return new(size, size);
        }

        private SizeOfInfo SizeOfField(IFieldSymbol field)
        {
            if (field.IsFixedSizeBuffer)
            {
                var fixedPointerType = field.Type as IPointerTypeSymbol;
                var bufferType = fixedPointerType!.PointedAtType;
                var typeSize = SizeOf(bufferType);
                var maxFieldSize = typeSize.MaxPrimitiveFieldSize;
                var totalSize = typeSize.TotalSize * field.FixedSize;
                return new(totalSize, maxFieldSize);
            }

            if (field.RefKind is not RefKind.None)
            {
                return SizeOfPointer();
            }

            return SizeOf(field.Type);
        }

        private SizeOfInfo GetOrCalculateSizeOf(ITypeSymbol type)
        {
            if (_sizes.TryGetValue(type, out var size))
            {
                return size;
            }

            var result = CalculateSizeOf(type);
            _sizes[type] = result;
            return result;
        }

        private SizeOfInfo CalculateSizeOf(ITypeSymbol type)
        {
            if (type is INamedTypeSymbol { TypeKind: TypeKind.Enum } enumType)
            {
                return SizeOf(enumType.EnumUnderlyingType!);
            }

            var inlineArrayAttribute = GetInlineArrayAttribute(type);
            var inlineArrayInfo = ParseInlineArrayInfo(inlineArrayAttribute);

            var fields = GetFields(type);
            if (inlineArrayInfo is not null)
            {
                var inlineArrayLength = inlineArrayInfo.Value.Size;
                if (fields is [var singleField])
                {
                    var singleFieldSize = SizeOfField(singleField);
                    var inlineArraySize = inlineArrayLength * singleFieldSize.TotalSize;
                    return new(inlineArraySize, singleFieldSize.TotalSize);
                }
            }

            var layoutAttribute = GetStructLayoutAttribute(type);
            var structLayoutInfo = ParseStructLayoutInfo(layoutAttribute);
            var minSize = structLayoutInfo.Size;
            var fieldSize = MinSizeForFields(
                structLayoutInfo,
                fields);
            var totalTypeSize = Math.Max(minSize, fieldSize);

            var maxFieldSize = MaxFieldSizeForFields(fields);

            return new(totalTypeSize, maxFieldSize);
        }

        private static IReadOnlyList<IFieldSymbol> GetFields(ITypeSymbol type)
        {
            if (type.IsTupleType)
            {
                return (type as INamedTypeSymbol)!.TupleElements;
            }

            return type.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(static s => !s.IsStatic)
                .Where(static s => !s.IsConst)
                .ToList();
        }

        private int MaxFieldSizeForFields(IReadOnlyList<IFieldSymbol> fields)
        {
            int maxSize = 0;

            foreach (var field in fields)
            {
                var fieldType = field.Type;
                var primitiveSize = PrimitiveSize(fieldType);
                if (primitiveSize is not null)
                {
                    var fieldSize = primitiveSize.Value;
                    maxSize = Math.Max(fieldSize, maxSize);
                    continue;
                }

                var sizeOfField = SizeOfField(field);
                maxSize = Math.Max(sizeOfField.MaxPrimitiveFieldSize, maxSize);
            }

            return maxSize;
        }

        private int MinSizeForFields(
            StructLayoutInfo layoutInfo,
            IReadOnlyList<IFieldSymbol> fields)
        {
            if (fields is [])
            {
                return 1;
            }

            int maxSize = 0;
            var maxFieldSize = fields.Select(s => SizeOfField(s).MaxPrimitiveFieldSize).Max();
            var pack = layoutInfo.Pack;
            if (layoutInfo.Pack is 0)
            {
                pack = maxFieldSize;
            }
            var alignment = Math.Min(maxFieldSize, pack);
            alignment = Math.Max(alignment, 1);

            foreach (var field in fields)
            {
                var fieldSize = SizeOfField(field).TotalSize;

                if (layoutInfo.LayoutKind is LayoutKind.Sequential or LayoutKind.Auto)
                {
                    var fieldOffset = maxSize;
                    var perFieldAlignment = Math.Min(fieldSize, alignment);
                    var alignmentOffset = fieldOffset % perFieldAlignment;
                    var alignmentRemainder = perFieldAlignment - alignmentOffset;
                    var shouldAddPadding =
                        alignmentOffset > 0 && fieldSize > alignmentRemainder;
                    if (shouldAddPadding)
                    {
                        maxSize += alignmentRemainder;
                    }
                    maxSize += fieldSize;
                }
                else
                {
                    var fieldOffsetAttribute = GetFieldOffsetAttribute(field);
                    var fieldOffset = ExtractFieldOffset(fieldOffsetAttribute);

                    var requiredSize = fieldOffset + fieldSize;
                    maxSize = Math.Max(maxSize, requiredSize);
                }
            }

            if (layoutInfo.LayoutKind is LayoutKind.Sequential or LayoutKind.Auto)
            {
                var fieldOffset = maxSize;
                var alignmentOffset = fieldOffset % alignment;
                // Add padding
                if (alignmentOffset > 0)
                {
                    maxSize += alignment - alignmentOffset;
                }
            }

            return maxSize;
        }

        private static int ExtractFieldOffset(AttributeData? fieldOffsetAttribute)
        {
            if (fieldOffsetAttribute?.ConstructorArguments.FirstOrDefault()
                is TypedConstant fieldOffsetArgument
                && fieldOffsetArgument is
                {
                    Kind: TypedConstantKind.Primitive,
                    Value: int fieldOffset
                })
            {
                return fieldOffset;
            }

            return 0;
        }

        private static StructLayoutInfo ParseStructLayoutInfo(AttributeData? layoutAttribute)
        {
            if (layoutAttribute is null)
            {
                return new(LayoutKind.Sequential, 0, 0);
            }

            var layoutKind = ExtractLayoutKind(layoutAttribute);
            var size = ParseArgument(nameof(StructLayoutAttribute.Size));
            var pack = ParseArgument(nameof(StructLayoutAttribute.Pack));

            return new(layoutKind, size, pack);

            int ParseArgument(string name)
            {
                var argument = layoutAttribute.NamedArguments
                    .FirstOrDefault(s => s.Key == name);
                if (argument.Value is not { Kind: TypedConstantKind.Primitive })
                {
                    return 0;
                }

                return argument.Value.Value as int? ?? 0;
            }
        }

        private static InlineArrayInfo? ParseInlineArrayInfo(AttributeData? inlineArray)
        {
            if (inlineArray is null)
            {
                return null;
            }

            var length = ParseLengthArgument();
            if (length is null)
            {
                return null;
            }

            return new(length.Value);

            int? ParseLengthArgument()
            {
                var argument = inlineArray.ConstructorArguments
                    .FirstOrDefault();
                if (argument is not { Kind: TypedConstantKind.Primitive })
                {
                    return null;
                }

                return argument.Value as int?;
            }
        }

        private static LayoutKind ExtractLayoutKind(AttributeData? layoutAttribute)
        {
            if (layoutAttribute?.ConstructorArguments.FirstOrDefault()
                is TypedConstant layoutKindArgument
                && layoutKindArgument is
                {
                    Kind: TypedConstantKind.Enum,
                    Value: int layoutKindInt
                })
            {
                return (LayoutKind)layoutKindInt;
            }

            return default;
        }

        private static AttributeData? GetInlineArrayAttribute(ITypeSymbol type)
        {
            return type.GetAttributes()
                .FirstOrDefault(static s
                    => s.AttributeClass is
                    {
                        Name: "InlineArrayAttribute",
                        ContainingNamespace:
                        {
                            Name: nameof(System.Runtime.CompilerServices),
                            ContainingNamespace:
                            {
                                Name: nameof(System.Runtime),
                                ContainingNamespace:
                                {
                                    Name: nameof(System)
                                }
                            }
                        }
                    });
        }

        private static AttributeData? GetStructLayoutAttribute(ITypeSymbol type)
        {
            return type.GetAttributes()
                .FirstOrDefault(static s
                    => s.AttributeClass is
                    {
                        Name: nameof(StructLayoutAttribute),
                        ContainingNamespace:
                        {
                            Name: nameof(System.Runtime.InteropServices),
                            ContainingNamespace:
                            {
                                Name: nameof(System.Runtime),
                                ContainingNamespace:
                                {
                                    Name: nameof(System)
                                }
                            }
                        }
                    });
        }

        private static AttributeData? GetFieldOffsetAttribute(IFieldSymbol type)
        {
            return type.GetAttributes()
                .FirstOrDefault(static s
                    => s.AttributeClass is
                    {
                        Name: nameof(FieldOffsetAttribute),
                        ContainingNamespace:
                        {
                            Name: nameof(System.Runtime.InteropServices),
                            ContainingNamespace:
                            {
                                Name: nameof(System.Runtime),
                                ContainingNamespace:
                                {
                                    Name: nameof(System)
                                }
                            }
                        }
                    });
        }

        private static unsafe int? PrimitiveSize(ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_Byte => sizeof(byte),
                SpecialType.System_Int16 => sizeof(short),
                SpecialType.System_Int32 => sizeof(int),
                SpecialType.System_Int64 => sizeof(long),

                SpecialType.System_SByte => sizeof(sbyte),
                SpecialType.System_UInt16 => sizeof(ushort),
                SpecialType.System_UInt32 => sizeof(uint),
                SpecialType.System_UInt64 => sizeof(ulong),

                SpecialType.System_Single => sizeof(float),
                SpecialType.System_Double => sizeof(double),

                SpecialType.System_Char => sizeof(char),
                SpecialType.System_Boolean => sizeof(bool),

                SpecialType.System_DateTime => sizeof(DateTime),

                _ => null,
            };
        }

        private readonly record struct StructLayoutInfo(
            LayoutKind LayoutKind,
            int Size,
            int Pack);

        private readonly record struct InlineArrayInfo(
            int Size);

        public readonly record struct SizeOfInfo(
            int TotalSize, int MaxPrimitiveFieldSize);
    }
}
