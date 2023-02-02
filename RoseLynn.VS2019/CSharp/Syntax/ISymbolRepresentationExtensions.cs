using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.CSharp.Syntax;

public static partial class ISymbolRepresentationExtensions
{
    public static string GetClosedGenericName(string name, IEnumerable<ITypeSymbol> typeArguments)
    {
        var typeArgumentNames = typeArguments.Select(GetRepresentationForType);
        var typeArgumentList = string.Join(", ", typeArgumentNames);
        return $"{name}<{typeArgumentList}>";
    }
    public static string GetMethodSignatureRepresentation(string name, IEnumerable<IParameterSymbol> parameters)
    {
        var parameterNames = parameters.Select(GetRepresentationForParameter);
        var parameterList = string.Join(", ", parameterNames);
        return $"{name}({parameterList})";
    }
}
