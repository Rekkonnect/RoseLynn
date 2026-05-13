#nullable enable

using System;
using System.Collections.Immutable;

namespace RoseLynn.Utilities;

public static class  EquatableImmutableArrayBuilder
{
    public static EquatableImmutableArray<T> Create<T>(ReadOnlySpan<T> values) => values.ToImmutableArray();
}
