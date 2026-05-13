using System.Diagnostics.CodeAnalysis;
using Garyon.Extensions;

namespace RoseLynn.SignatureViews;

public abstract record BaseTypeSymbolSignatureViewNullableHandler(
    ITypeSymbolSignatureView Symbol)
{
    public bool AcceptsNull
    {
        get
        {
            return IsNullableStruct
                || AcceptsNullExtra;
        }
    }

    protected virtual bool AcceptsNullExtra => false;

    public abstract bool IsNullableStruct { get; }

    public abstract ITypeSymbolSignatureView MakeNullable();

    public abstract ITypeSymbolSignatureView MakeNotNullable();

    public static BaseTypeSymbolSignatureViewNullableHandler CreateForSymbol(
        ITypeSymbolSignatureView view
    )
    {
        return view switch
        {
            INamedTypeSymbolSignatureView named
                => new NamedTypeSymbolSignatureViewNullableHandler(named),
            ITypeParameterSymbolSignatureView typeParameter
                => new TypeParameterSymbolSignatureViewNullableHandler(typeParameter),
            _ => new NoOpTypeSymbolSignatureViewNullableHandler(view),
        };
    }

    [return: NotNullIfNotNull(nameof(source))]
    protected static string? RemoveNullabilityMarker(string? source)
    {
        return source?.TrimEnd("?").ToString();
    }

    [return: NotNullIfNotNull(nameof(source))]
    protected static string? AddNullabilityMarker(string? source)
    {
        return source?.EnsureEndsWith("?");
    }

    protected string FullyQualifiedNameWithoutNullabilityMarker()
    {
        return RemoveNullabilityMarker(Symbol.FullyQualifiedName);
    }

    protected string FullyQualifiedNameWithNullabilityMarker()
    {
        return AddNullabilityMarker(Symbol.FullyQualifiedName);
    }
}
