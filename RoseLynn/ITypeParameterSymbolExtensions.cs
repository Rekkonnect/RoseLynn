using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn;

/// <summary>Provides useful extensions for the <seealso cref="ITypeParameterSymbol"/> type.</summary>
public static class ITypeParameterSymbolExtensions
{
    /// <summary>Gets the declaring symbol of the <seealso cref="ITypeParameterSymbol"/>.</summary>
    /// <param name="symbol">The <seealso cref="ITypeParameterSymbol"/> whose </param>
    /// <returns>The declaring symbol of the <seealso cref="ITypeParameterSymbol"/>. It could either be a <seealso cref="INamedTypeSymbol"/> or a <seealso cref="IMethodSymbol"/>.</returns>
    public static ISymbol GetDeclaringSymbol(this ITypeParameterSymbol symbol)
    {
        return (ISymbol)symbol.DeclaringType ?? symbol.DeclaringMethod;
    }

    /// <summary>Evaluates whether a given type arugment can substitute a parameter symbol, given its constraints. If the constraints also include other type parameters, this function will be recursively called for each type parameter type constraint, while only taking the declared constraints into account.</summary>
    /// <param name="typeParameter">The type parameter that is evaluted as being substituted.</param>
    /// <param name="typeArgument">The type argument that is evaluted as substituting the type parameter.</param>
    /// <param name="evaluateAsBase">Determines whether the evaluated type argument will not be considered a direct substitution. If <see langword="true"/>, if the type argument is an interface, <see langword="true"/> is returned immediately.</param>
    /// <returns><see langword="true"/> if the given type argument is a valid direct or indirect substitution for the given type parameter symbol, otherwise <see langword="false"/>.</returns>
    public static bool IsValidTypeArgumentSubstitution(this ITypeParameterSymbol typeParameter, ITypeSymbol typeArgument, bool evaluateAsBase)
    {
        if (evaluateAsBase && typeArgument.TypeKind is TypeKind.Interface)
            return true;

        if (typeParameter.HasConstructorConstraint)
        {
            if (typeArgument is not INamedTypeSymbol namedTypeArgument)
                return false;

            if (!namedTypeArgument.InstanceConstructors.Any(c => c.Parameters.IsEmpty))
                return false;
        }

        if (typeParameter.HasReferenceTypeConstraint)
            if (!typeArgument.IsReferenceType)
                return false;

        if (typeParameter.HasUnmanagedTypeConstraint)
            if (!typeArgument.IsUnmanagedType)
                return false;

        if (typeParameter.HasValueTypeConstraint)
            if (!typeArgument.IsValueType)
                return false;

        if (typeParameter.HasNotNullConstraint)
            if (!typeArgument.IsNotNull())
                return false;

#pragma warning disable RS1024 // Compare symbols correctly
        var baseTypeSet = new HashSet<ITypeSymbol>(typeArgument.GetAllBaseTypesAndInterfaces(), SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
        foreach (var constraintType in typeParameter.ConstraintTypes)
        {
            if (constraintType is ITypeParameterSymbol typeParameterConstraint)
            {
                /*\
                 * This can only check for whether the provided type meets the other type parameter's constraint;
                 * but cannot check whether it can be valid in context. The check is atomic to each type parameter
                 * per call of this function, each type argument substitution should mind the context within itself
                 * for type argument validity.
                \*/
                if (!IsValidTypeArgumentSubstitution(typeParameterConstraint, typeArgument, evaluateAsBase))
                    return false;

                continue;
            }

            if (!baseTypeSet.Contains(constraintType))
                return false;
        }

        return true;
    }
}
