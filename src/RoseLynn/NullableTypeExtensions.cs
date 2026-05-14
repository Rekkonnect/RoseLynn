#nullable enable

using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RoseLynn;

/// <summary>Provides helpers for reasoning about Roslyn nullable type metadata.</summary>
public static class NullableTypeExtensions
{
    extension(ITypeSymbol type)
    {
        /// <summary>Determines whether the type is a constructed <see cref="System.Nullable{T}"/> value type.</summary>
        /// <returns><see langword="true"/> if the type is nullable value type syntax; otherwise, <see langword="false"/>.</returns>
        public bool IsNullableStruct()
        {
            return type.SpecialType is SpecialType.System_Nullable_T
                || type is INamedTypeSymbol
                {
                    ConstructedFrom.SpecialType: SpecialType.System_Nullable_T
                };
        }

        /// <summary>Determines whether the type is nullable value type syntax and returns its underlying type.</summary>
        /// <param name="underlying">Receives the underlying non-nullable type when the type is nullable.</param>
        /// <returns><see langword="true"/> if the type is <see cref="System.Nullable{T}"/>; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>Determines whether the type can accept a <see langword="null"/> value.</summary>
        /// <returns><see langword="true"/> if the type can represent <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>Returns a nullable form of the type when the type system supports one.</summary>
        /// <param name="compilation">The compilation used to resolve <see cref="System.Nullable{T}"/>.</param>
        /// <returns>A nullable form of the type, or the original type when nullable conversion does not apply.</returns>
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

        /// <summary>Returns a non-nullable form of the type when the type system supports one.</summary>
        /// <returns>A non-nullable form of the type, or the original type when no change is needed.</returns>
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

        /// <summary>Gets whether the type is any Roslyn pointer type representation.</summary>
        public bool IsPointerType => type is IPointerTypeSymbol or IFunctionPointerTypeSymbol;
    }

    extension(ITypeParameterSymbol typeParameter)
    {
        /// <summary>Determines whether a type parameter may be substituted with a nullable type.</summary>
        /// <returns><see langword="true"/> if the type parameter can accept <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
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
