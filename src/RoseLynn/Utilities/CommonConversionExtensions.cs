using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace RoseLynn.Utilities;

/// <summary>Provides extension methods for <see cref="CommonConversion"/>.</summary>
public static class CommonConversionExtensions
{
    extension(CommonConversion conversion)
    {
        /// <summary>Determines whether two <see cref="CommonConversion"/> values are equal, comparing method symbols with <paramref name="comparer"/>.</summary>
        /// <param name="other">The other conversion to compare against.</param>
        /// <param name="comparer">The symbol equality comparer used to compare <see cref="CommonConversion.MethodSymbol"/>.</param>
        /// <returns><see langword="true"/> if all fields match; otherwise <see langword="false"/>.</returns>
        public bool Equals(
            CommonConversion other,
            SymbolEqualityComparer comparer)
        {
            return conversion.IsNullable == other.IsNullable
                && conversion.IsImplicit == other.IsImplicit
                && conversion.IsIdentity == other.IsIdentity
                && conversion.IsUserDefined == other.IsUserDefined
                && conversion.Exists == other.Exists
                && conversion.IsNumeric == other.IsNumeric
                && conversion.IsReference == other.IsReference
                && comparer.Equals(conversion.MethodSymbol, other.MethodSymbol)
                ;
        }
    }
}
