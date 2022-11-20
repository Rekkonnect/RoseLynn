using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

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

    /// <summary>Gets the types of the method's parameters.</summary>
    /// <param name="method">The method whose parameter types to get.</param>
    /// <returns>An immutable array with the types of the parameters of the method.</returns>
    public static ImmutableArray<ITypeSymbol> GetParameterTypes(this IMethodSymbol method)
    {
        return method.Parameters
                     .Select(p => p.Type)
                     .ToImmutableArray();
    }

    /// <summary>Gets all the used type symbols for the specified method symbol's signature.</summary>
    /// <param name="methodSymbol">The method symbol whose used types to get.</param>
    /// <returns>
    /// A collection of all the types that take part in the method symbol's signature, including its parameter types
    /// and its return type.
    /// </returns>
    /// <remarks>
    /// The same types will be returned more than once if the same type is used in multiple parameters.
    /// </remarks>
    public static ImmutableArray<ITypeSymbol> GetUsedSignatureTypeSymbols(this IMethodSymbol methodSymbol)
    {
        int totalTypes = methodSymbol.Parameters.Length + 1;
        var immutableArrayBuilder = ImmutableArray.CreateBuilder<ITypeSymbol>(totalTypes);

        immutableArrayBuilder.Add(methodSymbol.ReturnType);

        var parameterTypes = methodSymbol.GetParameterTypes();

        immutableArrayBuilder.AddRange(parameterTypes);

        return immutableArrayBuilder.ToImmutable();
    }

    /// <summary>Gets all the used type symbols for the specified method symbol.</summary>
    /// <param name="methodSymbol">The method symbol whose used types to get.</param>
    /// <returns>
    /// A collection of all the types that take part in the method symbol's signature, including its parameter types,
    /// its return type and its generic type arguments, if any. If the type arguments are not substituted,
    /// its type parameters are returned instead.
    /// </returns>
    /// <remarks>
    /// The same types will be returned more than once if the same type is used in multiple parameters, or if the
    /// generic type parameters are used in the method's signature.
    /// </remarks>
    public static ImmutableArray<ITypeSymbol> GetAllUsedTypeSymbols(this IMethodSymbol methodSymbol)
    {
        var result = GetUsedSignatureTypeSymbols(methodSymbol);
        if (methodSymbol.IsGenericMethod)
        {
            result.AddRange(methodSymbol.TypeArguments);
        }
        return result;
    }
}
