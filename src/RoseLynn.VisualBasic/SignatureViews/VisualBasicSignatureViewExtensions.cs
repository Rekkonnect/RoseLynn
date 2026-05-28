using Microsoft.CodeAnalysis;
using RoseLynn.Core.SignatureViews;

namespace RoseLynn.VisualBasic.SignatureViews;

/// <summary>Extensions for creating VB-augmented signature views from core SSV interfaces.</summary>
public static class VisualBasicSignatureViewExtensions
{
    /// <summary>Wraps <paramref name="view"/> in an <see cref="IVisualBasicTypeSymbolSignatureView"/>, computing VB-specific properties eagerly.</summary>
    public static IVisualBasicTypeSymbolSignatureView ForVisualBasic(this ITypeSymbolSignatureView view)
    {
        return new VisualBasicTypeSymbolSignatureViewWrapper(view, ComputeKeyword(view));
    }

    /// <summary>Wraps <paramref name="view"/> in an <see cref="IVisualBasicNamedTypeSymbolSignatureView"/>, computing VB-specific properties eagerly.</summary>
    public static IVisualBasicNamedTypeSymbolSignatureView ForVisualBasic(this INamedTypeSymbolSignatureView view)
    {
        return new VisualBasicNamedTypeSymbolSignatureViewWrapper(view, ComputeKeyword(view));
    }

    private static string? ComputeKeyword(ITypeSymbolSignatureView view)
    {
        if (view is INamedTypeSymbolSignatureView named
            && named.ConstructedFromSpecialType is SpecialType.System_Nullable_T
            && named.TypeArguments.Array.Length == 1)
        {
            var underlyingKeyword = named.TypeArguments.Array[0].SpecialType
                .GetKeywordIdentifierForPredefinedType();
            return underlyingKeyword is null ? null : underlyingKeyword + "?";
        }

        return view.SpecialType.GetKeywordIdentifierForPredefinedType();
    }
}
