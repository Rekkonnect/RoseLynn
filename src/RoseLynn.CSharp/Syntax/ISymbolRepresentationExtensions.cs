using Microsoft.CodeAnalysis;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides helpers for formatting symbol representations in C# syntax.</summary>
public static partial class ISymbolRepresentationExtensions
{
    /// <summary>Formats a generic identifier with its closed type arguments.</summary>
    /// <param name="name">The generic type or method name.</param>
    /// <param name="typeArguments">The type arguments to include.</param>
    /// <returns>The closed generic identifier.</returns>
    public static string GetClosedGenericName(string name, IEnumerable<ITypeSymbol> typeArguments)
    {
        var typeArgumentNames = typeArguments.Select(GetRepresentationForType);
        var typeArgumentList = string.Join(", ", typeArgumentNames);
        return $"{name}<{typeArgumentList}>";
    }
    /// <summary>Formats a method signature using the provided name and parameter symbols.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The parameters to include in the signature.</param>
    /// <returns>The formatted method signature.</returns>
    public static string GetMethodSignatureRepresentation(string name, IEnumerable<IParameterSymbol> parameters)
    {
        var parameterNames = parameters.Select(GetRepresentationForParameter);
        var parameterList = string.Join(", ", parameterNames);
        return $"{name}({parameterList})";
    }
}
