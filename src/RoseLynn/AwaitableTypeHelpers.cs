using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoseLynn.Core;

/// <summary>Provides helpers for inspecting whether a type follows the C# awaitable pattern.</summary>
public static class AwaitableTypeHelpers
{
    /// <summary>Gets <seealso cref="AwaitableTypeInfo"/> for the given type if it follows the awaitable pattern, otherwise <see langword="null"/>.</summary>
    /// <param name="type">The named type symbol to inspect.</param>
    /// <returns>An <seealso cref="AwaitableTypeInfo"/> instance if <paramref name="type"/> is awaitable; otherwise <see langword="null"/>.</returns>
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

    /// <summary>Gets <seealso cref="AwaiterTypeInfo"/> for the given type if it satisfies the awaiter contract, otherwise <see langword="null"/>.</summary>
    /// <param name="type">The named type symbol to inspect as a potential awaiter. May be <see langword="null"/>.</param>
    /// <returns>An <seealso cref="AwaiterTypeInfo"/> instance if <paramref name="type"/> meets the awaiter requirements; otherwise <see langword="null"/>.</returns>
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
