using System.Diagnostics.CodeAnalysis;
using Garyon.Extensions;

namespace RoseLynn.Core.SignatureViews;

/// <summary>Provides nullable-shape transformations for type signature views.</summary>
/// <param name="Symbol">The type signature view being adapted.</param>
public abstract record BaseTypeSymbolSignatureViewNullableHandler(
    ITypeSymbolSignatureView Symbol)
{
    /// <summary>Gets whether the handled symbol can accept <see langword="null"/>.</summary>
    public bool AcceptsNull
    {
        get
        {
            return IsNullableStruct
                || AcceptsNullExtra;
        }
    }

    /// <summary>Provides additional null-acceptance logic for derived handlers.</summary>
    protected virtual bool AcceptsNullExtra => false;

    /// <summary>Gets whether the handled symbol represents a nullable value type.</summary>
    public abstract bool IsNullableStruct { get; }

    /// <summary>Returns a nullable form of the handled symbol.</summary>
    /// <returns>A nullable type signature view.</returns>
    public abstract ITypeSymbolSignatureView MakeNullable();

    /// <summary>Returns a non-nullable form of the handled symbol.</summary>
    /// <returns>A non-nullable type signature view.</returns>
    public abstract ITypeSymbolSignatureView MakeNotNullable();

    /// <summary>Creates the appropriate nullable handler for a type signature view.</summary>
    /// <param name="view">The type signature view to adapt.</param>
    /// <returns>The nullable handler for the supplied view.</returns>
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

    /// <summary>Removes a trailing nullability marker from an identifier when present.</summary>
    [return: NotNullIfNotNull(nameof(source))]
    protected static string? RemoveNullabilityMarker(string? source)
    {
        return source?.TrimEnd("?").ToString();
    }

    /// <summary>Adds a trailing nullability marker to an identifier.</summary>
    [return: NotNullIfNotNull(nameof(source))]
    protected static string? AddNullabilityMarker(string? source)
    {
        return source?.EnsureEndsWith("?");
    }

    /// <summary>Returns the handled symbol's fully qualified name with any trailing nullability marker removed.</summary>
    /// <returns>The fully qualified name without a <c>?</c> suffix.</returns>
    protected string FullyQualifiedNameWithoutNullabilityMarker()
    {
        return RemoveNullabilityMarker(Symbol.FullyQualifiedName);
    }

    /// <summary>Returns the handled symbol's fully qualified name with a trailing nullability marker appended.</summary>
    /// <returns>The fully qualified name with a <c>?</c> suffix.</returns>
    protected string FullyQualifiedNameWithNullabilityMarker()
    {
        return AddNullabilityMarker(Symbol.FullyQualifiedName);
    }
}
