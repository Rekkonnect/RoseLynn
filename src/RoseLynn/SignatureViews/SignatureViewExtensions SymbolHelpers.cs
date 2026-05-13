using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using RoseLynn;

namespace RoseLynn.SignatureViews;

public static partial class SignatureViewExtensions
{
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

    public static string SimplerIdentifier(
        this ISymbolSignatureView view
    )
    {
        return SimplerTupleIdentifier(view)
            ?? SimplerNullableIdentifier(view)
            ?? KeywordOrFullyQualifiedName(view)
            ;
    }

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

    public static BaseTypeSymbolSignatureViewNullableHandler NullableHandler(
        this ITypeSymbolSignatureView view
    )
    {
        return BaseTypeSymbolSignatureViewNullableHandler.CreateForSymbol(view);
    }
}
