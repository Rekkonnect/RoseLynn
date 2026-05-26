using Garyon.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace RoseLynn;

public static class OperationExtensions
{
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

    public static TOperation? NearestAncestorOfType<TOperation>(this IOperation operation)
        where TOperation : IOperation
    {
        return operation.Ancestors()
            .OfType<TOperation>()
            .FirstOrDefault();
    }

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

    public static IOperation? Instance(
        this IConditionalAccessInstanceOperation accessInstanceOperation)
    {
        return accessInstanceOperation.NearestAncestorOfType<IConditionalAccessOperation>()?.Operation;
    }
}
