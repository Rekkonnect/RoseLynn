using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace RoseLynn.SignatureViews;

/// <summary>Provides helper projections and transforms for symbol signature views.</summary>
public static partial class SignatureViewExtensions
{
    /// <summary>Gets the identifiable symbol kind flags for a type signature view.</summary>
    /// <param name="typeSymbol">The type signature view to inspect.</param>
    /// <returns>The identifiable symbol kind flags for the type.</returns>
    public static IdentifiableSymbolKind GetIdentifiableSymbolKind(
        this ITypeSymbolSignatureView typeSymbol)
    {
        var identifiableSymbolKind = typeSymbol.TypeKind.GetIdentifiableSymbolKindFlag();
        if (typeSymbol.IsRecord)
        {
            identifiableSymbolKind |= IdentifiableSymbolKind.Record;
        }

        return identifiableSymbolKind;
    }

    /// <summary>Gets the keyword form of a symbol when available, otherwise its fully qualified name.</summary>
    /// <param name="view">The symbol signature view to format.</param>
    /// <returns>A keyword or fully qualified identifier for the symbol.</returns>
    public static string KeywordOrFullyQualifiedName(
        this ISymbolSignatureView view
    )
    {
        if (view.Keyword is not null and var keyword)
        {
            return keyword;
        }

        return view.FullyQualifiedName;
    }

    /// <summary>Formats a nullable named type using shorthand nullable syntax when possible.</summary>
    /// <param name="view">The symbol signature view to format.</param>
    /// <returns>A nullable shorthand identifier, or <see langword="null"/> if not applicable.</returns>
    public static string? SimplerNullableIdentifier(
        this ISymbolSignatureView view
    )
    {
        if (view is INamedTypeSymbolSignatureView
            { ConstructedFromSpecialType: SpecialType.System_Nullable_T } named)
        {
            var underlying = named.TypeArguments.Array[0];
            return $"{underlying.SimplerIdentifier()}?";
        }

        return null;
    }

    /// <summary>Formats a tuple type using tuple syntax when possible.</summary>
    /// <param name="view">The symbol signature view to format.</param>
    /// <returns>A tuple identifier, or <see langword="null"/> if tuple syntax is not applicable.</returns>
    public static string? SimplerTupleIdentifier(
        this ISymbolSignatureView view
    )
    {
        if (view is INamedTypeSymbolSignatureView
            { TupleFieldNames.Length: not 0 } named)
        {
            var types = named.TypeArguments.Array;
            var fieldNames = named.TupleFieldNames.Array;
            if (types.Length != fieldNames.Length)
            {
                return null;
            }

            return CreateTupleFieldSyntax();

            string CreateTupleFieldSyntax()
            {
                var builder = new StringBuilder();
                builder.Append('(');
                for (int i = 0; i < types.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }

                    var type = types[i];
                    var typeString = type.SimplerIdentifier();
                    builder.Append(typeString);

                    builder.Append(' ');
                    var fieldName = fieldNames[i];
                    builder.Append(fieldName);
                }
                builder.Append(')');
                return builder.ToString();
            }
        }

        return null;
    }

    /// <summary>Gets the simplest identifier form available for a symbol signature view.</summary>
    /// <param name="view">The symbol signature view to format.</param>
    /// <returns>A simplified identifier for the symbol.</returns>
    public static string SimplerIdentifier(
        this ISymbolSignatureView view
    )
    {
        return SimplerTupleIdentifier(view)
            ?? SimplerNullableIdentifier(view)
            ?? KeywordOrFullyQualifiedName(view)
            ;
    }

    /// <summary>Constructs a named type signature view with a new set of type arguments.</summary>
    /// <param name="view">The generic named type to construct.</param>
    /// <param name="typeArguments">The type arguments to apply.</param>
    /// <returns>A constructed named type signature view.</returns>
    public static NamedTypeSymbolSignatureView Construct(
        this INamedTypeSymbolSignatureView view,
        ImmutableArray<ITypeSymbolSignatureView> typeArguments
    )
    {
        return view.CloneIntoConcrete() with
        {
            TypeArguments = typeArguments,
            ConstructedFromSpecialType = view.SpecialType,
            FullyQualifiedName = ConstructFullyQualifiedName(
                view.FullyQualifiedIdentifier,
                typeArguments
            ),
        };
    }

    private static string ConstructFullyQualifiedName(
        string identifier,
        ImmutableArray<ITypeSymbolSignatureView> typeArguments
    )
    {
        var typeArgumentList = string.Join(
            ", ",
            typeArguments.Select(s => s.FullyQualifiedName));
        return $"{identifier}<{typeArgumentList}>";
    }

    /// <summary>Gets the nullable handler for a type signature view.</summary>
    /// <param name="view">The type signature view to adapt.</param>
    /// <returns>The nullable handler for the supplied view.</returns>
    public static BaseTypeSymbolSignatureViewNullableHandler NullableHandler(
        this ITypeSymbolSignatureView view
    )
    {
        return BaseTypeSymbolSignatureViewNullableHandler.CreateForSymbol(view);
    }
}
