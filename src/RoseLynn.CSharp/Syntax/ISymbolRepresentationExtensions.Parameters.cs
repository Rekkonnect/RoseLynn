using Microsoft.CodeAnalysis;

namespace RoseLynn.CSharp.Syntax;

public static partial class ISymbolRepresentationExtensions
{
    /// <summary>Gets the C# representation of a parameter, including its type and name.</summary>
    /// <param name="parameterSymbol">The parameter symbol whose representation to get.</param>
    /// <returns>A string in the form <c>type name</c> for the given parameter.</returns>
    public static string GetRepresentationForParameter(this IParameterSymbol parameterSymbol)
    {
        return $"{parameterSymbol.Type.GetRepresentationForType()} {parameterSymbol.Name}";
    }
}
