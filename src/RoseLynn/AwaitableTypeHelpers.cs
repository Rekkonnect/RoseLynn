using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoseLynn;

public static class AwaitableTypeHelpers
{
    public static AwaitableTypeInfo? GetAwaitableTypeInfo(INamedTypeSymbol type)
    {
        var getAwaiterMethod = type
            .GetMembers(nameof(ExampleAwaitableType.GetAwaiter))
            .OfType<IMethodSymbol>()
            .Where(IMethodSymbolExtensions.IsParameterlessAndNonGeneric)
            .FirstOrDefault();

        if (getAwaiterMethod is null)
        {
            return null;
        }

        var awaiterType = getAwaiterMethod.ReturnType as INamedTypeSymbol;
        var awaiterTypeInfo = GetAwaiterTypeInfo(awaiterType);
        if (awaiterTypeInfo is null)
        {
            return null;
        }

        return new(type, awaiterTypeInfo);
    }

    public static AwaiterTypeInfo? GetAwaiterTypeInfo(INamedTypeSymbol? type)
    {
        if (type is null)
        {
            return null;
        }

        var isCompletedProperty = type
            .GetMembers(nameof(ExampleAwaiterType.IsCompleted))
            .OfType<IPropertySymbol>()
            .FirstOrDefault();

        if (isCompletedProperty is null)
        {
            return null;
        }

        var getResultMethod = type
            .GetMembers(nameof(ExampleAwaiterType.GetResult))
            .OfType<IMethodSymbol>()
            .Where(IMethodSymbolExtensions.IsParameterlessAndNonGeneric)
            .FirstOrDefault();

        if (getResultMethod is null)
        {
            return null;
        }

        var notifyCompletionInterface = KnownSymbolNameMatches
            .SystemRuntimeCompilerServicesINotifyCompletion
            .FirstMatchingOrDefault(type.AllInterfaces)
            as INamedTypeSymbol;
        var criticalNotifyCompletionInterface = KnownSymbolNameMatches
            .SystemRuntimeCompilerServicesICriticalNotifyCompletion
            .FirstMatchingOrDefault(type.AllInterfaces)
            as INamedTypeSymbol;

        if (notifyCompletionInterface is null && criticalNotifyCompletionInterface is null)
        {
            return null;
        }

        var onCompletedImplementation = type.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(s => s.ImplementsInterface(notifyCompletionInterface))
            .FirstOrDefault();

        var unsafeOnCompletedImplementation = type.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(s => s.ImplementsInterface(criticalNotifyCompletionInterface))
            .FirstOrDefault();

        return new(
            Type: type,
            ResultType: getResultMethod.ReturnType,
            IsCompletedProperty: isCompletedProperty,
            GetResultMethod: getResultMethod,
            OnCompletedMethod: onCompletedImplementation,
            UnsafeOnCompletedMethod: unsafeOnCompletedImplementation);
    }

    // Examples to show that the types are awaitable

    private sealed class ExampleAwaitableType
    {
        public async Task AwaitThis()
        {
            await this;
        }

        public ExampleAwaiterType GetAwaiter() => new();
    }

    private sealed class ExampleAwaiterType : ICriticalNotifyCompletion
    {
        public bool IsCompleted => false;
        public int GetResult() => 1;
        public void OnCompleted(Action continuation) { }
        public void UnsafeOnCompleted(Action continuation) { }
    }
}
