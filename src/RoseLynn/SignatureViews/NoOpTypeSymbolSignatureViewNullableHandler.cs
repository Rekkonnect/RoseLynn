namespace RoseLynn.Core.SignatureViews;

/// <summary>Leaves nullable transformations unchanged for signature views that do not support them.</summary>
/// <param name="Symbol">The type signature view being adapted.</param>
public sealed record NoOpTypeSymbolSignatureViewNullableHandler(
    ITypeSymbolSignatureView Symbol)
    : BaseTypeSymbolSignatureViewNullableHandler(Symbol)
{
    /// <summary>Gets whether the handled symbol is a nullable value type.</summary>
    public override bool IsNullableStruct => false;

    /// <inheritdoc/>
    protected override bool AcceptsNullExtra => false;

    /// <summary>Returns the original symbol unchanged.</summary>
    /// <returns>The original type signature view.</returns>
    public override ITypeSymbolSignatureView MakeNotNullable()
    {
        return Symbol;
    }

    /// <summary>Returns the original symbol unchanged.</summary>
    /// <returns>The original type signature view.</returns>
    public override ITypeSymbolSignatureView MakeNullable()
    {
        return Symbol;
    }
}
