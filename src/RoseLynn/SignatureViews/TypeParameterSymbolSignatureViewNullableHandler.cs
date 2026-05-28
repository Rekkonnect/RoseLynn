using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RoseLynn.Core.SignatureViews;

/// <summary>Handles nullable transformations for type-parameter signature views.</summary>
/// <param name="TypeParameterSymbol">The type-parameter signature view being adapted.</param>
public record TypeParameterSymbolSignatureViewNullableHandler(
    ITypeParameterSymbolSignatureView TypeParameterSymbol)
    : BaseTypeSymbolSignatureViewNullableHandler(TypeParameterSymbol)
{
    /// <summary>Gets whether the handled symbol is a nullable value type.</summary>
    public override bool IsNullableStruct => false;

    /// <inheritdoc/>
    protected override bool AcceptsNullExtra
    {
        get
        {
            return TypeParameterSymbol
                is
                {
                    HasNotNullConstraint: false,
                    HasValueTypeConstraint: false,
                    HasUnmanagedTypeConstraint: false,
                }
                && (!TypeParameterSymbol.HasReferenceTypeConstraint
                || TypeParameterSymbol.ReferenceTypeConstraintNullableAnnotation
                    is not NullableAnnotation.NotAnnotated);
        }
    }

    /// <summary>Returns a non-nullable form of the type parameter.</summary>
    /// <returns>The non-nullable type-parameter signature view.</returns>
    public override ITypeSymbolSignatureView MakeNotNullable()
    {
        return TypeParameterSymbol.CloneIntoConcrete() with
        {
            NullableAnnotation = NullableAnnotation.NotAnnotated,
            FullyQualifiedName = FullyQualifiedNameWithoutNullabilityMarker(),
            ReferenceTypeConstraintNullableAnnotation = NullableAnnotation.NotAnnotated,
            HasNotNullConstraint = true,
            ConstraintTypes = TypeParameterSymbol.ConstraintTypes
                .Select(s => s.NullableHandler().MakeNotNullable())
                .ToImmutableArray(),
        };
    }

    /// <summary>Returns a nullable form of the type parameter.</summary>
    /// <returns>The nullable type-parameter signature view.</returns>
    public override ITypeSymbolSignatureView MakeNullable()
    {
        if (TypeParameterSymbol.HasValueTypeConstraint
            || TypeParameterSymbol.HasUnmanagedTypeConstraint)
        {
            return WellKnownSymbolSignatureViews.System_Nullable_T
                .Construct([TypeParameterSymbol]);
        }

        return TypeParameterSymbol.CloneIntoConcrete() with
        {
            NullableAnnotation = NullableAnnotation.Annotated,
            FullyQualifiedName = FullyQualifiedNameWithNullabilityMarker(),
            ReferenceTypeConstraintNullableAnnotation = NullableAnnotation.Annotated,
            HasNotNullConstraint = false,
            ConstraintTypes = TypeParameterSymbol.ConstraintTypes
                .Select(s => s.NullableHandler().MakeNullable())
                .ToImmutableArray(),
        };
    }
}
