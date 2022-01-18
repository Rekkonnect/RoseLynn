using Microsoft.CodeAnalysis;

namespace RoseLynn;

#nullable enable

/// <summary>Contains extensions for the <see cref="IMethodSymbol"/> interface.</summary>
public static class IMethodSymbolExtensions
{
    /// <summary>Determines whether a <seealso cref="IMethodSymbol"/> is public and parameterless.</summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    /// <remarks>This does not evaluate the arity of the method.</remarks>
    public static bool IsPublicParameterlessMethod(this IMethodSymbol symbol)
    {
        return symbol is
        {
            DeclaredAccessibility: Accessibility.Public,
            Parameters.IsEmpty: true,
        };
    }
    public static bool IsParameterlessNonGenericMethod(this IMethodSymbol symbol)
    {
        return symbol is
        {
            IsGenericMethod: false,
            Parameters.IsEmpty: true,
        };
    }
}
