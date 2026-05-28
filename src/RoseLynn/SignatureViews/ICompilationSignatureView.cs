using Microsoft.CodeAnalysis;

namespace RoseLynn.Core.SignatureViews;

/// <summary>
/// Represents a light signature view of an <see cref="Compilation"/>.
/// </summary>
public interface ICompilationSignatureView
{
    /// <inheritdoc cref="Compilation.Assembly"/>
    public IAssemblySymbolSignatureView Assembly { get; }

    /// <inheritdoc cref="Compilation.Language"/>
    public string Language { get; }

    /// <inheritdoc cref="Compilation.DynamicType"/>
    public IDynamicTypeSymbolSignatureView DynamicType { get; }
}
