namespace RoseLynn.InternalGenerators;

public static class KnownSymbolNames
{
    public const string CSharpOperatorTokenAttribute = nameof(CSharpOperatorTokenAttribute);
    public const string CanBeCheckedOperatorAttribute = nameof(CanBeCheckedOperatorAttribute);
    public const string OperatorArityAttribute = nameof(OperatorArityAttribute);
    public const string UnaryOperatorAttribute = nameof(UnaryOperatorAttribute);
    public const string BinaryOperatorAttribute = nameof(BinaryOperatorAttribute);

    public static class Full
    {
        public static readonly string[] BaseNamespaces = new[] { nameof(RoseLynn) };

        public static readonly FullSymbolName
            CSharpOperatorTokenAttribute = new(KnownSymbolNames.CSharpOperatorTokenAttribute, BaseNamespaces),
            CanBeCheckedOperatorAttribute = new(KnownSymbolNames.CanBeCheckedOperatorAttribute, BaseNamespaces),
            OperatorArityAttribute = new(KnownSymbolNames.OperatorArityAttribute, BaseNamespaces),
            UnaryOperatorAttribute = new(KnownSymbolNames.UnaryOperatorAttribute, BaseNamespaces),
            BinaryOperatorAttribute = new(KnownSymbolNames.BinaryOperatorAttribute, BaseNamespaces);
    }
}
