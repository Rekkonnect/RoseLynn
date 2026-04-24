using Microsoft.CodeAnalysis;

namespace RoseLynn.CSharp.Syntax;

public static partial class ISymbolRepresentationExtensions
{
    /// <summary>Gets the full representation of the method symbol to be used as a method group.</summary>
    /// <param name="method">The method symbol whose representation to get.</param>
    /// <returns>The C# representation of the provided method. It does not include any information about the invocation.</returns>
    public static string GetRepresentationForMethod(this IMethodSymbol method)
    {
        if (method.IsGenericMethod)
        {
            return GetClosedGenericName(method.Name, method.TypeArguments);
        }

        return method.Name;
    }

    /// <summary>
    /// Gets the full representation of the method symbol representing its
    /// signature, including its parameters and type parameters.
    /// </summary>
    /// <param name="method">The method symbol whose representation to get.</param>
    /// <returns>The C# representation of the provided method. It does not include the return type.</returns>
    /// <remarks>
    /// The method symbol that is used derives from the
    /// <seealso cref="IMethodSymbol.OriginalDefinition"/> property.
    /// </remarks>
    public static string GetRepresentationForMethodSignature(this IMethodSymbol method)
    {
        method = method.OriginalDefinition;

        var baseRepresentation = GetRepresentationForMethod(method);
        return GetMethodSignatureRepresentation(baseRepresentation, method.Parameters);
    }

    /// <summary>
    /// Gets the full representation of the method symbol representing its
    /// signature, including its parameters and type parameters.
    /// </summary>
    /// <param name="delegateSymbol">The method symbol whose representation to get.</param>
    /// <returns>The C# representation of the provided method.</returns>
    /// <remarks>
    /// The method symbol that is used derives from the
    /// <seealso cref="IMethodSymbol.OriginalDefinition"/> property.
    /// </remarks>
    public static string GetRepresentationForDelegateSignature(this INamedTypeSymbol delegateSymbol)
    {
        delegateSymbol = delegateSymbol.OriginalDefinition;

        var delegateInvokeMethod = delegateSymbol.DelegateInvokeMethod;
        var delegateNameRepresentation = GetRepresentationForType(delegateSymbol);
        return GetMethodSignatureRepresentation(delegateNameRepresentation, delegateInvokeMethod.Parameters);
    }
}
