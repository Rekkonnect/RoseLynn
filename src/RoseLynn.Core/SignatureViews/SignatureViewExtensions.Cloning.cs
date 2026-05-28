namespace RoseLynn.Core.SignatureViews;

/// <summary>Extension methods for cloning signature view interfaces into their concrete record implementations.</summary>
public static partial class SignatureViewExtensions
{
    /// <summary>Creates a concrete <see cref="NamedTypeSymbolSignatureView"/> by copying all properties from <paramref name="view"/>.</summary>
    /// <param name="view">The named type signature view to clone.</param>
    /// <returns>A new <see cref="NamedTypeSymbolSignatureView"/> with all properties copied from <paramref name="view"/>.</returns>
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

    /// <summary>Creates a concrete <see cref="TypeParameterSymbolSignatureView"/> by copying all properties from <paramref name="view"/>.</summary>
    /// <param name="view">The type-parameter signature view to clone.</param>
    /// <returns>A new <see cref="TypeParameterSymbolSignatureView"/> with all properties copied from <paramref name="view"/>.</returns>
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
