#nullable enable

using System;
using System.Collections.Immutable;

namespace RoseLynn.Utilities;

/// <summary>Provides the collection-builder entry point for <see cref="EquatableImmutableArray{T}"/>.</summary>
public static class  EquatableImmutableArrayBuilder
{
    /// <summary>Creates an equatable immutable array from a span of values.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="values">The values to copy into the array.</param>
    /// <returns>The resulting equatable immutable array.</returns>
    public static EquatableImmutableArray<T> Create<T>(ReadOnlySpan<T> values) => values.ToImmutableArray();
}
