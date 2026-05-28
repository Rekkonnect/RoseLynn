using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Contains information about a type that satisfies the C# awaiter contract.</summary>
/// <param name="Type">The awaiter type symbol.</param>
/// <param name="ResultType">The type returned by <c>GetResult()</c>.</param>
/// <param name="IsCompletedProperty">The <c>IsCompleted</c> property symbol on the awaiter.</param>
/// <param name="GetResultMethod">The parameterless <c>GetResult()</c> method symbol.</param>
/// <param name="OnCompletedMethod">The <c>OnCompleted</c> implementation from <c>INotifyCompletion</c>, or <see langword="null"/> if absent.</param>
/// <param name="UnsafeOnCompletedMethod">The <c>UnsafeOnCompleted</c> implementation from <c>ICriticalNotifyCompletion</c>, or <see langword="null"/> if absent.</param>
public sealed record AwaiterTypeInfo(
    INamedTypeSymbol Type,
    ITypeSymbol ResultType,
    IPropertySymbol IsCompletedProperty,
    IMethodSymbol GetResultMethod,
    IMethodSymbol? OnCompletedMethod,
    IMethodSymbol? UnsafeOnCompletedMethod);
