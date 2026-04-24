using System.Collections.Generic;
using System.Collections.Immutable;

namespace RoseLynn.Utilities;

#nullable enable annotations

public static class ImmutableArrayExtensions
{
    public static ImmutableArray<T> ToImmutableArrayOrEmpty<T>(this IEnumerable<T> source)
    {
        return source?.ToImmutableArray() ?? ImmutableArray<T>.Empty;
    }
}
