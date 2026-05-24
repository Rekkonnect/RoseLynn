using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace RoseLynn.Utilities;

public static class CommonConversionExtensions
{
    extension(CommonConversion conversion)
    {
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
