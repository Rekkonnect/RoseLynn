namespace RoseLynn.InternalGenerators;

public static class KnownSymbolNames
{
    public const string CSharpOperatorTokenAttribute = nameof(CSharpOperatorTokenAttribute);
    public const string CanBeCheckedOperatorAttribute = nameof(CanBeCheckedOperatorAttribute);
    public const string NaryOperatorAttribute = nameof(NaryOperatorAttribute);
    public const string UnaryOperatorAttribute = nameof(UnaryOperatorAttribute);
    public const string BinaryOperatorAttribute = nameof(BinaryOperatorAttribute);

    public static class Full
    {
        public static readonly string[] BaseNamespaces = new[] { nameof(RoseLynn) };

        public static readonly FullSymbolName CSharpOperatorTokenAttribute = new(nameof(CSharpOperatorTokenAttribute), BaseNamespaces);
        public static readonly FullSymbolName CanBeCheckedOperatorAttribute = new(nameof(CanBeCheckedOperatorAttribute), BaseNamespaces);
        public static readonly FullSymbolName NaryOperatorAttribute = new(nameof(NaryOperatorAttribute), BaseNamespaces);
        public static readonly FullSymbolName UnaryOperatorAttribute = new(nameof(UnaryOperatorAttribute), BaseNamespaces);
        public static readonly FullSymbolName BinaryOperatorAttribute = new(nameof(BinaryOperatorAttribute), BaseNamespaces);
    }
}
