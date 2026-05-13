namespace RoseLynn.SignatureViews;

public static partial class SignatureViewExtensions
{
    public static NamedTypeSymbolSignatureView CloneIntoConcrete(
        this INamedTypeSymbolSignatureView view
    )
    {
        return new()
        {
            Name = view.Name,
            MetadataName = view.MetadataName,
            FullyQualifiedName = view.FullyQualifiedName,
            FullMetadataName = view.FullMetadataName,
            ContainedNamespaceName = view.ContainedNamespaceName,
            Keyword = view.Keyword,
            Kind = view.Kind,
            DeclaredAccessibility = view.DeclaredAccessibility,
            IsStatic = view.IsStatic,
            IsSealed = view.IsSealed,
            IsAbstract = view.IsAbstract,
            IsVirtual = view.IsVirtual,
            IsOverride = view.IsOverride,
            IsExtern = view.IsExtern,
            SpecialType = view.SpecialType,
            TypeKind = view.TypeKind,
            IsRecord = view.IsRecord,
            IsReferenceType = view.IsReferenceType,
            IsValueType = view.IsValueType,
            NullableAnnotation = view.NullableAnnotation,
            IsFileLocal = view.IsFileLocal,
            IsGenericType = view.IsGenericType,
            TypeParameters = view.TypeParameters,
            TypeArguments = view.TypeArguments,
            EnumUnderlyingSpecialType = view.EnumUnderlyingSpecialType,
            ConstructedFromSpecialType = view.ConstructedFromSpecialType,
            ContainingTypes = view.ContainingTypes,
            TupleFieldNames = view.TupleFieldNames,
        };
    }

    public static TypeParameterSymbolSignatureView CloneIntoConcrete(
        this ITypeParameterSymbolSignatureView view
    )
    {
        return new()
        {
            Name = view.Name,
            MetadataName = view.MetadataName,
            FullyQualifiedName = view.FullyQualifiedName,
            FullMetadataName = view.FullMetadataName,
            ContainedNamespaceName = view.ContainedNamespaceName,
            Keyword = view.Keyword,
            Kind = view.Kind,
            DeclaredAccessibility = view.DeclaredAccessibility,
            IsStatic = view.IsStatic,
            IsSealed = view.IsSealed,
            IsAbstract = view.IsAbstract,
            IsVirtual = view.IsVirtual,
            IsOverride = view.IsOverride,
            IsExtern = view.IsExtern,
            SpecialType = view.SpecialType,
            TypeKind = view.TypeKind,
            IsRecord = view.IsRecord,
            IsReferenceType = view.IsReferenceType,
            IsValueType = view.IsValueType,
            NullableAnnotation = view.NullableAnnotation,
            Variance = view.Variance,
            TypeParameterKind = view.TypeParameterKind,
            Ordinal = view.Ordinal,
            HasReferenceTypeConstraint = view.HasReferenceTypeConstraint,
            ReferenceTypeConstraintNullableAnnotation = view.ReferenceTypeConstraintNullableAnnotation,
            HasValueTypeConstraint = view.HasValueTypeConstraint,
            AllowsRefLikeType = view.AllowsRefLikeType,
            HasUnmanagedTypeConstraint = view.HasUnmanagedTypeConstraint,
            HasNotNullConstraint = view.HasNotNullConstraint,
            HasConstructorConstraint = view.HasConstructorConstraint,
            ConstraintTypes = view.ConstraintTypes,
        };
    }
}
