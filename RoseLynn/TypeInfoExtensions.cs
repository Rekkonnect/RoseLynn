using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Provides useful extensions for the <seealso cref="TypeInfo"/> type.</summary>
public static class TypeInfoExtensions
{
    /// <summary>Determines whether a <see cref="TypeInfo"/> result matches a given <see cref="ITypeSymbol"/>, using <seealso cref="SymbolEqualityComparer.Default"/> to perform the comparison.</summary>
    /// <param name="typeInfo">The <seealso cref="TypeInfo"/> whose <seealso cref="ITypeSymbol"/> to attempt to match.</param>
    /// <param name="symbol">The desired matching <see cref="ITypeSymbol"/>.</param>
    /// <returns><see langword="true"/> if <seealso cref="TypeInfo.Type"/> or <seealso cref="TypeInfo.ConvertedType"/> match <paramref name="symbol"/>, otherwise <see langword="false"/>.</returns>
    /// <remarks>For customized symbol equality comparison, use the <seealso cref="MatchesExplicitlyOrImplicitly(TypeInfo, ITypeSymbol, SymbolEqualityComparer)"/> overload.</remarks>
    public static bool MatchesExplicitlyOrImplicitly(this TypeInfo typeInfo, ITypeSymbol symbol)
    {
        return MatchesExplicitlyOrImplicitly(typeInfo, symbol, SymbolEqualityComparer.Default);
    }
    /// <summary>Determines whether a <see cref="TypeInfo"/> result matches a given <see cref="ITypeSymbol"/>.</summary>
    /// <param name="typeInfo">The <seealso cref="TypeInfo"/> whose <seealso cref="ITypeSymbol"/> to attempt to match.</param>
    /// <param name="symbol">The desired matching <see cref="ITypeSymbol"/>.</param>
    /// <param name="symbolEqualityComparer">The <seealso cref="SymbolEqualityComparer"/> to use when comparing the matching types.</param>
    /// <returns><see langword="true"/> if <seealso cref="TypeInfo.Type"/> or <seealso cref="TypeInfo.ConvertedType"/> match <paramref name="symbol"/>, otherwise <see langword="false"/>.</returns>
    public static bool MatchesExplicitlyOrImplicitly(this TypeInfo typeInfo, ITypeSymbol symbol, SymbolEqualityComparer symbolEqualityComparer)
    {
        return symbolEqualityComparer.Equals(typeInfo.Type, symbol)
            || symbolEqualityComparer.Equals(typeInfo.ConvertedType, symbol);
    }

    /// <summary>Determines whether a <see cref="TypeInfo"/> result matches a given <see cref="SpecialType"/>.</summary>
    /// <param name="typeInfo">The <seealso cref="TypeInfo"/> whose <seealso cref="SpecialType"/> to attempt to match.</param>
    /// <param name="specialType">The desired matching <see cref="SpecialType"/>.</param>
    /// <returns><see langword="true"/> if <seealso cref="TypeInfo.Type"/> or <seealso cref="TypeInfo.ConvertedType"/> match <paramref name="specialType"/>, otherwise <see langword="false"/>.</returns>
    public static bool MatchesExplicitlyOrImplicitly(this TypeInfo typeInfo, SpecialType specialType)
    {
        return typeInfo.Type?.SpecialType == specialType
            || typeInfo.ConvertedType?.SpecialType == specialType;
    }
}
