using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace RoseLynn.Core;

/// <summary>Provides extension methods for Roslyn <seealso cref="IOperation"/> types.</summary>
public static class OperationExtensions
{
    /// <summary>Gets the operation representing the receiver/target of an invocation, handling both instance and extension methods.</summary>
    /// <param name="invocation">The invocation to inspect.</param>
    /// <returns>The receiver operation for instance calls, the extension parameter argument for extension calls, or <see langword="null"/>.</returns>
    public static IOperation? TargetForInvocation(
        this IInvocationOperation? invocation)
    {
        if (invocation?.Instance is not null)
        {
            return invocation.Instance;
        }

        var targetMethod = invocation?.TargetMethod;
        if (targetMethod is null)
        {
            return null;
        }

        // Extremely redundant not null check
        Asserts.NotNull(invocation);

        var extensionParameter = targetMethod.GetExtensionMethodParameter();
        var argument = invocation.Arguments.FirstOrDefault(
            s => s.Parameter?.Equals(extensionParameter, SymbolEqualityComparer.Default)
                ?? false);

        return argument?.Value;
    }

    /// <summary>Enumerates all ancestor operations of <paramref name="operation"/>, from direct parent up to the root.</summary>
    /// <param name="operation">The operation whose ancestors to enumerate.</param>
    /// <returns>The ancestor operations in order from parent to root.</returns>
    public static IEnumerable<IOperation> Ancestors(this IOperation operation)
    {
        var ancestor = operation;
        while (true)
        {
            ancestor = ancestor.Parent;
            if (ancestor is null)
            {
                yield break;
            }

            yield return ancestor;
        }
    }

    /// <summary>Returns the nearest ancestor of <paramref name="operation"/> that is of type <typeparamref name="TOperation"/>.</summary>
    /// <typeparam name="TOperation">The ancestor operation type to find.</typeparam>
    /// <param name="operation">The operation to start from.</param>
    /// <returns>The nearest matching ancestor, or <see langword="null"/> if not found.</returns>
    public static TOperation? NearestAncestorOfType<TOperation>(this IOperation operation)
        where TOperation : IOperation
    {
        return operation.Ancestors()
            .OfType<TOperation>()
            .FirstOrDefault();
    }

    /// <summary>Returns the furthest (root-closest) ancestor of <paramref name="operation"/> that is of type <typeparamref name="TOperation"/>.</summary>
    /// <typeparam name="TOperation">The ancestor operation type to find.</typeparam>
    /// <param name="operation">The operation to start from.</param>
    /// <returns>The furthest matching ancestor, or <see langword="null"/> if not found.</returns>
    public static TOperation? FurthestAncestorOfType<TOperation>(this IOperation operation)
        where TOperation : IOperation
    {
        return operation.Ancestors()
            .OfType<TOperation>()
            .LastOrDefault();
    }

    /// <summary>
    /// Returns the target symbol that the operation involves, if it's a simple symbol
    /// like a parameter, local, or member (not an invocation). If the operation doesn't
    /// represent a simple reference to a symbol, this returns <see langword="null"/>.
    /// </summary>
    public static ISymbol? TargetSymbol(this IOperation operation)
    {
        return operation switch
        {
            IParameterReferenceOperation parameterReference => parameterReference.Parameter,
            ILocalReferenceOperation localReference => localReference.Local,
            IMemberReferenceOperation memberReference => memberReference.Member,

            IAssignmentOperation assignment => assignment.Target.TargetSymbol(),

            // What the fuck is this API
            IParameterInitializerOperation initializer => initializer.Parameter,
            IMemberInitializerOperation initializer => initializer.InitializedMember.TargetSymbol(),
            IVariableDeclaratorOperation declarator => declarator.Symbol,

            _ => null,
        };
    }

    /// <summary>Gets the value being assigned or initialized by the operation, if applicable.</summary>
    /// <param name="operation">The operation to inspect.</param>
    /// <returns>The assigned or initialized value operation, or <see langword="null"/> if the operation has no applicable value.</returns>
    public static IOperation? Value(this IOperation operation)
    {
        return operation switch
        {
            ISymbolInitializerOperation initializer => initializer.Value,
            IAssignmentOperation initializer => initializer.Value,
            IVariableDeclaratorOperation declarator => declarator.Initializer?.Value,

            _ => null,
        };
    }

    /// <summary>Gets the operation representing the receiver of the conditional access expression containing <paramref name="accessInstanceOperation"/>.</summary>
    /// <param name="accessInstanceOperation">The conditional access instance operation.</param>
    /// <returns>The receiver operation, or <see langword="null"/> if no enclosing conditional access is found.</returns>
    public static IOperation? Instance(
        this IConditionalAccessInstanceOperation accessInstanceOperation)
    {
        return accessInstanceOperation.NearestAncestorOfType<IConditionalAccessOperation>()?.Operation;
    }
}
