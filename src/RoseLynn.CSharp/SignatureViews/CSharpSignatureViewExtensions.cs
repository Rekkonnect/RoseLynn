using Microsoft.CodeAnalysis;
using RoseLynn.Core.SignatureViews;

namespace RoseLynn.CSharp.SignatureViews;

/// <summary>Extensions for creating C#-augmented signature views from core SSV interfaces.</summary>
public static class CSharpSignatureViewExtensions
{
    // TODO: Introduce specific classes for every symbol kind

    /// <summary>Wraps <paramref name="view"/> in an <see cref="ICSharpTypeSymbolSignatureView"/>, computing C#-specific properties eagerly.</summary>
    public static ICSharpTypeSymbolSignatureView ForCSharp(this ITypeSymbolSignatureView view)
    {
        return new CSharpTypeSymbolSignatureViewWrapper(view, ComputeKeyword(view));
    }

    /// <summary>Wraps <paramref name="view"/> in an <see cref="ICSharpNamedTypeSymbolSignatureView"/>, computing C#-specific properties eagerly.</summary>
    public static ICSharpNamedTypeSymbolSignatureView ForCSharp(this INamedTypeSymbolSignatureView view)
    {
        return new CSharpNamedTypeSymbolSignatureViewWrapper(view, ComputeKeyword(view));
    }

    private static string? ComputeKeyword(ITypeSymbolSignatureView view)
    {
        if (view.TypeKind is TypeKind.Dynamic)
            return "dynamic";

        if (view is INamedTypeSymbolSignatureView named
            && named.ConstructedFromSpecialType is SpecialType.System_Nullable_T
            && named.TypeArguments.Array.Length == 1)
        {
            var underlyingKeyword = named.TypeArguments.Array[0].SpecialType
                .GetKeywordIdentifierForPredefinedType();
            return underlyingKeyword is null ? null : underlyingKeyword + "?";
        }

        var keyword = view.SpecialType.GetKeywordIdentifierForPredefinedType();
        if (keyword is null)
            return null;

        if (view.NullableAnnotation is NullableAnnotation.Annotated)
            return keyword + "?";

        return keyword;
    }
}
