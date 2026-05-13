#nullable enable

using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RoseLynn;

[NeedsDocs]
public static class NullableTypeExtensions
{
    extension(ITypeSymbol type)
    {
        public bool IsNullableStruct()
        {
            return type.SpecialType is SpecialType.System_Nullable_T
                || type is INamedTypeSymbol
                {
                    ConstructedFrom.SpecialType: SpecialType.System_Nullable_T
                };
        }

        public bool IsNullableStruct(
            [NotNullWhen(true)]
            out ITypeSymbol? underlying)
        {
            var isNullable = type.IsNullableStruct();
            underlying = default;
            if (isNullable)
            {
                underlying = type.GetTypeArguments().First();
            }
            return isNullable;
        }

        public bool AcceptsNull()
        {
            return type.IsNullableStruct()
                || type.IsPointerType
                || (type is ITypeParameterSymbol typeParameter
                    && typeParameter.AcceptsNull())
                || (type.IsReferenceType
                    && type.TypeKind is not TypeKind.Error
                    && type.NullableAnnotation is not NullableAnnotation.NotAnnotated)
                ;
        }

        public ITypeSymbol MakeNullable(Compilation compilation)
        {
            if (type.IsPointerType)
            {
                // Pointer types do not support nullable annotations yet
                return type;
            }

            if (type.IsValueType)
            {
                var nullable = compilation.GetSpecialType(SpecialType.System_Nullable_T);
                return nullable.Construct([type]);
            }

            return type.WithNullableAnnotation(NullableAnnotation.Annotated);
        }

        public ITypeSymbol MakeNotNullable()
        {
            if (type.IsPointerType)
            {
                // Pointer types do not support nullable annotations yet
                return type;
            }

            if (type.IsNullableStruct(out var underlyingNullableType))
            {
                return underlyingNullableType;
            }

            return type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        }

        public bool IsPointerType => type is IPointerTypeSymbol or IFunctionPointerTypeSymbol;
    }

    extension(ITypeParameterSymbol typeParameter)
    {
        public bool AcceptsNull()
        {
            if (typeParameter.HasNotNullConstraint)
            {
                return false;
            }

            if (typeParameter.HasValueTypeConstraint)
            {
                return false;
            }

            if (typeParameter.ReferenceTypeConstraintNullableAnnotation
                is NullableAnnotation.Annotated)
            {
                return true;
            }

            return true;
        }
    }
}
