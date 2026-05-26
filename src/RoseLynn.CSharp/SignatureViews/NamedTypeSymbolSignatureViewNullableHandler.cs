using Microsoft.CodeAnalysis;

namespace RoseLynn.CSharp.SignatureViews;

/// <summary>Handles nullable conversions for named type signature views.</summary>
/// <param name="NamedSymbol">The named type signature view being adapted.</param>
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

    /// <summary>Gets whether the symbol represents a constructed <see cref="System.Nullable{T}"/> type.</summary>
    public override bool IsNullableStruct
        => Symbol is INamedTypeSymbolSignatureView
        {
            ConstructedFromSpecialType: SpecialType.System_Nullable_T,
        };

    /// <summary>Returns a non-nullable form of the named type.</summary>
    /// <returns>The non-nullable named type signature view.</returns>
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

    /// <summary>Returns a nullable form of the named type.</summary>
    /// <returns>The nullable named type signature view.</returns>
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
