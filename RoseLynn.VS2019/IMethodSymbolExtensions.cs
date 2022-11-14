using Microsoft.CodeAnalysis;

namespace RoseLynn;

#nullable enable

/// <summary>Contains extensions for the <see cref="IMethodSymbol"/> interface.</summary>
public static class IMethodSymbolExtensions
{
    /// <summary>Determines whether a <seealso cref="IMethodSymbol"/> is public and parameterless.</summary>
    /// <param name="symbol">The <seealso cref="IMethodSymbol"/> to determine whether it's public and parameterless.</param>
    /// <returns><see langword="true"/> if the given <seealso cref="IMethodSymbol"/> represents a public and parameterless method, otherwise <see langword="false"/>.</returns>
    /// <remarks>This does not evaluate the arity of the method.</remarks>
    public static bool IsPublicParameterlessMethod(this IMethodSymbol symbol)
    {
        return symbol is
        {
            DeclaredAccessibility: Accessibility.Public,
            Parameters.IsEmpty: true,
        };
    }
    /// <summary>Determines whether a <seealso cref="IMethodSymbol"/> is has no parameters and is not generic.</summary>
    /// <param name="symbol">The <seealso cref="IMethodSymbol"/> to determine whether it's parameterless and not generic.</param>
    /// <returns><see langword="true"/> if the given <seealso cref="IMethodSymbol"/> represents a parameterless and non-generic method, otherwise <see langword="false"/>.</returns>
    public static bool IsParameterlessNonGenericMethod(this IMethodSymbol symbol)
    {
        return symbol is
        {
            IsGenericMethod: false,
            Parameters.IsEmpty: true,
        };
    }
}
