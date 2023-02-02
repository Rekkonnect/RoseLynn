using Microsoft.CodeAnalysis;

namespace RoseLynn.CSharp.Syntax;

public static partial class ISymbolRepresentationExtensions
{
    public static string GetRepresentationForParameter(this IParameterSymbol parameterSymbol)
    {
        return $"{parameterSymbol.Type.GetRepresentationForType()} {parameterSymbol.Name}";
    }
}