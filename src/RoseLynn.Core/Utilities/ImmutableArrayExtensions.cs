using System.Collections.Immutable;

namespace RoseLynn.Core.Utilities;

/// <summary>Provides helpers for working with immutable arrays and their equatable wrapper.</summary>
public static class ImmutableArrayExtensions
{
    /// <summary>Converts a sequence to an immutable array, returning an empty array when the source is <see langword="null"/>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>An immutable array for the source sequence, or an empty array.</returns>
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
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The array to normalize.</param>
    /// <returns>A non-default immutable array.</returns>
    public static ImmutableArray<T> Safe<T>(this ImmutableArray<T> source)
    {
        return source.IsDefault ? [] : source;
    }

    /// <summary>Converts a sequence to an <see cref="EquatableImmutableArray{T}"/>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>The resulting equatable immutable array.</returns>
    public static EquatableImmutableArray<T> ToEquatableImmutableArray<T>(
        this IEnumerable<T> source)
    {
        return source
            .ToImmutableArray()
            .ToEquatableImmutableArray();
    }

    /// <summary>Wraps an immutable array in an <see cref="EquatableImmutableArray{T}"/>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The immutable array to wrap.</param>
    /// <returns>The resulting equatable immutable array.</returns>
    public static EquatableImmutableArray<T> ToEquatableImmutableArray<T>(
        this ImmutableArray<T> source)
    {
        return new(source);
    }
}
