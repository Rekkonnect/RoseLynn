using Microsoft.CodeAnalysis;

namespace RoseLynn;

public sealed record AwaitableTypeInfo(
    INamedTypeSymbol Type,
    AwaiterTypeInfo AwaiterTypeInfo)
{
    public ITypeSymbol AwaiterType => AwaiterTypeInfo.Type;
    public ITypeSymbol ResultType => AwaiterTypeInfo.ResultType;
}
