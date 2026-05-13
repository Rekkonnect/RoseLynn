namespace RoseLynn.SignatureViews;

public sealed record NoOpTypeSymbolSignatureViewNullableHandler(
    ITypeSymbolSignatureView Symbol)
    : BaseTypeSymbolSignatureViewNullableHandler(Symbol)
{
    public override bool IsNullableStruct => false;

    protected override bool AcceptsNullExtra => false;

    public override ITypeSymbolSignatureView MakeNotNullable()
    {
        return Symbol;
    }

    public override ITypeSymbolSignatureView MakeNullable()
    {
        return Symbol;
    }
}