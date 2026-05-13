using Microsoft.CodeAnalysis;

namespace RoseLynn.SignatureViews;

public sealed record NamedTypeSymbolSignatureViewNullableHandler(
    INamedTypeSymbolSignatureView NamedSymbol)
    : BaseTypeSymbolSignatureViewNullableHandler(NamedSymbol)
{
    protected override bool AcceptsNullExtra
    {
        get
        {
            return NamedSymbol.IsReferenceType
                && NamedSymbol.TypeKind is not TypeKind.Error
                && NamedSymbol.NullableAnnotation is not NullableAnnotation.NotAnnotated
                ;
        }
    }

    public override bool IsNullableStruct
        => Symbol is INamedTypeSymbolSignatureView
        {
            ConstructedFromSpecialType: SpecialType.System_Nullable_T,
        };

    public override ITypeSymbolSignatureView MakeNotNullable()
    {
        if (IsNullableStruct)
        {
            return NamedSymbol.TypeArguments.Array[0];
        }

        return NamedSymbol.CloneIntoConcrete() with
        {
            NullableAnnotation = NullableAnnotation.NotAnnotated,
            FullyQualifiedName = FullyQualifiedNameWithoutNullabilityMarker(),
            Keyword = RemoveNullabilityMarker(NamedSymbol.Keyword),
        };
    }

    public override ITypeSymbolSignatureView MakeNullable()
    {
        if (NamedSymbol.TypeKind is TypeKind.Struct)
        {
            if (NamedSymbol.SpecialType is SpecialType.System_Nullable_T
                || NamedSymbol.ConstructedFromSpecialType is SpecialType.System_Nullable_T)
            {
                // Do not construct Nullable<Nullable<T>>, effectively T??
                return NamedSymbol;
            }

            return WellKnownSymbolSignatureViews.System_Nullable_T
                .Construct([NamedSymbol]) with
                {
                    Keyword = AddNullabilityMarker(NamedSymbol.Keyword),
                };
        }

        return NamedSymbol.CloneIntoConcrete() with
        {
            NullableAnnotation = NullableAnnotation.Annotated,
            FullyQualifiedName = FullyQualifiedNameWithNullabilityMarker(),
            Keyword = AddNullabilityMarker(NamedSymbol.Keyword),
        };
    }
}
