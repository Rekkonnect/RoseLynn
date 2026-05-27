using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace RoseLynn;

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
        return method
            .Parameters
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

    extension(IMethodSymbol method)
    {
        /// <summary>Gets the first parameter of the extension method, accounting for reduced extension methods.</summary>
        /// <returns>The <see cref="IParameterSymbol"/> representing the receiver parameter, or <see langword="null"/> if the method is not an extension method.</returns>
        public IParameterSymbol? GetExtensionMethodParameter()
        {
            if (method.MethodKind is MethodKind.ReducedExtension)
            {
                return GetExtensionMethodParameter(method.OriginalDefinition);
            }

            if (method.IsExtensionMethod)
            {
                return method.Parameters.FirstOrDefault();
            }

            return null;
        }

        /// <summary>Gets the actual receiver type of the method: the extension parameter's type for extension methods, the declared receiver type for instance methods, or <see langword="null"/> for non-extension static methods.</summary>
        /// <returns>The receiver type, or <see langword="null"/> for non-extension static methods.</returns>
        public ITypeSymbol? GetActualReceiverType()
        {
            var extensionParameter = method.GetExtensionMethodParameter();
            if (extensionParameter is not null)
            {
                return extensionParameter.Type;
            }

            if (method.IsStatic)
            {
                return null;
            }

            return method.ReceiverType;
        }

        /// <summary>Returns <see langword="true"/> if this method explicitly implements or is the implementation for any member of <paramref name="interface"/>.</summary>
        /// <param name="interface">The interface to check against; <see langword="null"/> is treated as no match.</param>
        /// <returns><see langword="true"/> when the method implements the interface; otherwise <see langword="false"/>.</returns>
        public bool ImplementsInterface(
            INamedTypeSymbol? @interface)
        {
            if (method.ExplicitInterfaceImplementations
                .Any(s => s.ContainingType.Equals(@interface, SymbolEqualityComparer.Default)))
            {
                return true;
            }

            foreach (var interfaceMethod in @interface?.GetMembers().OfType<IMethodSymbol>() ?? [])
            {
                if (method.Equals(
                        method.ContainingType.FindImplementationForInterfaceMember(interfaceMethod),
                        SymbolEqualityComparer.Default))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Returns <see langword="true"/> if the method has no parameters.</summary>
        /// <returns><see langword="true"/> when <see cref="IMethodSymbol.Parameters"/> is empty.</returns>
        public bool IsParameterless()
        {
            return method.Parameters.Length is 0;
        }

        /// <summary>Returns <see langword="true"/> if the method has no parameters and is not generic.</summary>
        /// <returns><see langword="true"/> when the method is both parameterless and non-generic.</returns>
        public bool IsParameterlessAndNonGeneric()
        {
            return method.IsParameterless()
                && !method.IsGenericMethod;
        }
    }
}
