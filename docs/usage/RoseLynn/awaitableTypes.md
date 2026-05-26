# Awaitable Type Helpers

`AwaitableTypeHelpers` inspects Roslyn symbols to determine whether a type follows the C# awaitable pattern.

## Main entry points

- `GetAwaitableTypeInfo(INamedTypeSymbol)`
- `GetAwaiterTypeInfo(INamedTypeSymbol?)`

## What is checked

For an awaitable type, the helper looks for:

- a parameterless `GetAwaiter()`
- an awaiter type with `IsCompleted`
- a parameterless `GetResult()`
- `INotifyCompletion` or `ICriticalNotifyCompletion`

If the pattern is complete, RoseLynn returns:

- `AwaitableTypeInfo`
- `AwaiterTypeInfo`

## Example

```csharp
var awaitableInfo = AwaitableTypeHelpers.GetAwaitableTypeInfo(namedTypeSymbol);
if (awaitableInfo is not null)
{
    var resultType = awaitableInfo.Awaiter.ResultType;
}
```

## When it is useful

- analyzer rules around custom awaitables
- source generators that need to special-case awaitable members
- semantic inspections where the Roslyn symbol alone is too low-level
