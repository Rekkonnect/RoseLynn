using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RoseLynn.SignatureViews;

public record TypeParameterSymbolSignatureViewNullableHandler(
    ITypeParameterSymbolSignatureView TypeParameterSymbol)
    : BaseTypeSymbolSignatureViewNullableHandler(TypeParameterSymbol)
{
    public override bool IsNullableStruct => false;

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
