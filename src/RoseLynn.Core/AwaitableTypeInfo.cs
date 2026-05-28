using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Contains information about a type that follows the C# awaitable pattern.</summary>
/// <param name="Type">The awaitable type symbol.</param>
/// <param name="AwaiterTypeInfo">The <seealso cref="RoseLynn.AwaiterTypeInfo"/> describing the type returned by <c>GetAwaiter()</c>.</param>
public sealed record AwaitableTypeInfo(
    INamedTypeSymbol Type,
    AwaiterTypeInfo AwaiterTypeInfo)
{
    /// <summary>Gets the type returned by <c>GetAwaiter()</c>.</summary>
    public ITypeSymbol AwaiterType => AwaiterTypeInfo.Type;
    /// <summary>Gets the result type of the awaitable, as returned by the awaiter's <c>GetResult()</c>.</summary>
    public ITypeSymbol ResultType => AwaiterTypeInfo.ResultType;
}
