using RoseLynn.Core.SignatureViews;

namespace RoseLynn.VisualBasic.SignatureViews;

/// <summary>A VB-enriched <see cref="ITypeSymbolSignatureView"/> that exposes language-specific properties.</summary>
public interface IVisualBasicTypeSymbolSignatureView : ITypeSymbolSignatureView
{
    /// <summary>The Visual Basic keyword for this type (e.g. <c>Integer</c>, <c>String</c>, <c>Integer?</c>), or <see langword="null"/> if the type has no predefined keyword.</summary>
    public string? Keyword { get; }
}

/// <summary>A VB-enriched <see cref="INamedTypeSymbolSignatureView"/> that exposes language-specific properties.</summary>
public interface IVisualBasicNamedTypeSymbolSignatureView : INamedTypeSymbolSignatureView, IVisualBasicTypeSymbolSignatureView;
