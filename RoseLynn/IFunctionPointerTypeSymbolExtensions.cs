using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RoseLynn;

#nullable enable

/// <summary>Contains extensions for the <see cref="IFunctionPointerTypeSymbol"/> interface.</summary>
public static class IFunctionPointerTypeSymbolExtensions
{
    /// <summary>Gets the types of the function pointer's parameters.</summary>
    /// <param name="functionPointer">The function pointer whose parameter types to get.</param>
    /// <returns>An immutable array with the types of the parameters of the function pointer.</returns>
    public static ImmutableArray<ITypeSymbol> GetParameterTypes(this IFunctionPointerTypeSymbol functionPointer)
    {
        return functionPointer.Signature.GetParameterTypes();
    }

    /// <summary>Gets all the types used in the signature of the function pointer.</summary>
    /// <param name="functionPointerSymbol">The function pointer symbol whose signature's used type symbols to get.</param>
    /// <returns>
    /// A collection of all the types that take part in the function pointer's signature, including its parameter types
    /// and its return type.
    /// </returns>
    /// <remarks>
    /// The same types will be returned more than once if the same type is used in multiple parameters.
    /// </remarks>
    public static ImmutableArray<ITypeSymbol> GetUsedSignatureTypeSymbols(this IFunctionPointerTypeSymbol functionPointerSymbol)
    {
        return functionPointerSymbol.Signature.GetUsedSignatureTypeSymbols();
    }
}
