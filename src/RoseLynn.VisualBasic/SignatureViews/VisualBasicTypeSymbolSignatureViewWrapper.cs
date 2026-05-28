using Microsoft.CodeAnalysis;
using RoseLynn.Core.SignatureViews;
using RoseLynn.Core.Utilities;

namespace RoseLynn.VisualBasic.SignatureViews;

/// <summary>Wraps an <see cref="ITypeSymbolSignatureView"/> and implements <see cref="IVisualBasicTypeSymbolSignatureView"/>, storing VB-specific properties at construction.</summary>
public class VisualBasicTypeSymbolSignatureViewWrapper(ITypeSymbolSignatureView inner, string? keyword)
    : IVisualBasicTypeSymbolSignatureView
{
    /// <inheritdoc/>
    public string? Keyword { get; } = keyword;

    /// <inheritdoc/>
    public string Name => inner.Name;
    /// <inheritdoc/>
    public string MetadataName => inner.MetadataName;
    /// <inheritdoc/>
    public string FullyQualifiedIdentifier => inner.FullyQualifiedIdentifier;
    /// <inheritdoc/>
    public string FullyQualifiedName => inner.FullyQualifiedName;
    /// <inheritdoc/>
    public string FullMetadataName => inner.FullMetadataName;
    /// <inheritdoc/>
    public string? ContainedNamespaceName => inner.ContainedNamespaceName;
    /// <inheritdoc/>
    public SymbolKind Kind => inner.Kind;
    /// <inheritdoc/>
    public Accessibility DeclaredAccessibility => inner.DeclaredAccessibility;
    /// <inheritdoc/>
    public bool IsStatic => inner.IsStatic;
    /// <inheritdoc/>
    public bool IsSealed => inner.IsSealed;
    /// <inheritdoc/>
    public bool IsAbstract => inner.IsAbstract;
    /// <inheritdoc/>
    public bool IsVirtual => inner.IsVirtual;
    /// <inheritdoc/>
    public bool IsOverride => inner.IsOverride;
    /// <inheritdoc/>
    public bool IsExtern => inner.IsExtern;
    /// <inheritdoc/>
    public TypeKind TypeKind => inner.TypeKind;
    /// <inheritdoc/>
    public SpecialType SpecialType => inner.SpecialType;
    /// <inheritdoc/>
    public bool IsRecord => inner.IsRecord;
    /// <inheritdoc/>
    public bool IsValueType => inner.IsValueType;
    /// <inheritdoc/>
    public bool IsReferenceType => inner.IsReferenceType;
    /// <inheritdoc/>
    public NullableAnnotation NullableAnnotation => inner.NullableAnnotation;
}
