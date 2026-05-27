using Microsoft.CodeAnalysis;

namespace RoseLynn.CSharp.SignatureViews;

/// <summary>
/// Represents a light signature view of an <see cref="Compilation"/>,
/// implementing the <see cref="ICompilationSignatureView"/>
/// interface as a baseline.
/// </summary>
public record CompilationSignatureView : ICompilationSignatureView
{
    /// <inheritdoc/>
    public required IAssemblySymbolSignatureView Assembly { get; set; }

    /// <inheritdoc/>
    public required string Language { get; set; }

    /// <inheritdoc/>
    public required IDynamicTypeSymbolSignatureView DynamicType { get; set; }
}
