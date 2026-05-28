using RoseLynn.Core.SignatureViews;

namespace RoseLynn.CSharp.SignatureViews;

/// <summary>A C#-enriched <see cref="ITypeSymbolSignatureView"/> that exposes language-specific properties.</summary>
public interface ICSharpTypeSymbolSignatureView : ITypeSymbolSignatureView
{
    /// <summary>The C# keyword for this type (e.g. <c>int</c>, <c>string?</c>, <c>dynamic</c>), or <see langword="null"/> if the type has no predefined keyword.</summary>
    public string? Keyword { get; }
}

/// <summary>A C#-enriched <see cref="INamedTypeSymbolSignatureView"/> that exposes language-specific properties.</summary>
public interface ICSharpNamedTypeSymbolSignatureView : INamedTypeSymbolSignatureView, ICSharpTypeSymbolSignatureView;
