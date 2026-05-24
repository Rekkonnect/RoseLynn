using Microsoft.CodeAnalysis;

namespace RoseLynn;

public sealed record AwaiterTypeInfo(
    INamedTypeSymbol Type,
    ITypeSymbol ResultType,
    IPropertySymbol IsCompletedProperty,
    IMethodSymbol GetResultMethod,
    IMethodSymbol? OnCompletedMethod,
    IMethodSymbol? UnsafeOnCompletedMethod);
