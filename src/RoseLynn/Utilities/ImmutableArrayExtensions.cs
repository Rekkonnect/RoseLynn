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

    /// <summary>
    /// Gets an immutable array instance that will not throw when trying to
    /// access and process its elements, i.e. when
    /// <see cref="ImmutableArray{T}.IsDefault"/> is <see langword="true"/>.
    /// If the provided array is safe to access to begin with, the original
    /// instance is returned instead.
    /// </summary>
    public static ImmutableArray<T> Safe<T>(this ImmutableArray<T> source)
    {
        return source.IsDefault ? [] : source;
    }

    public static EquatableImmutableArray<T> ToEquatableImmutableArray<T>(
        this IEnumerable<T> source)
    {
        return source
            .ToImmutableArray()
            .ToEquatableImmutableArray();
    }

    public static EquatableImmutableArray<T> ToEquatableImmutableArray<T>(
        this ImmutableArray<T> source)
    {
        return new(source);
    }
}
