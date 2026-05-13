#nullable enable

using Garyon.Extensions.Comparison;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoseLynn.Utilities;

[CollectionBuilder(
    typeof(EquatableImmutableArrayBuilder),
    nameof(EquatableImmutableArrayBuilder.Create))]
public readonly record struct EquatableImmutableArray<T>(ImmutableArray<T> Array)
    : IEquatable<EquatableImmutableArray<T>>, IEnumerable<T>
{
    public ImmutableArray<T> SafeArray => Array.Safe();

    public int Length => SafeArray.Length;

    public int CompareTo(EquatableImmutableArray<T> other)
    {
        return SafeArray.BeginCompare(other.SafeArray)
            .By(s => s.Length)
            .ThenBy(s => s.GetHashCode())
            .Result
            ;
    }

    public int CompareTo(object obj)
    {
        if (obj is EquatableImmutableArray<T> other)
        {
            return CompareTo(other);
        }

        return 1;
    }

    public bool Equals(EquatableImmutableArray<T> other)
    {
        return SafeArray.SequenceEqual(other.SafeArray);
    }

    public override int GetHashCode()
    {
        const int startCode = unchecked((int)0x9876F120);
        const int nullCode = unchecked((int)0x9021B8A4);

        int result = startCode;
        foreach (var element in SafeArray)
        {
            result ^= element?.GetHashCode() ?? nullCode;
        }
        return result;
    }

    public static implicit operator EquatableImmutableArray<T>(ImmutableArray<T> array)
        => new(array);
    public static implicit operator ImmutableArray<T>(EquatableImmutableArray<T> array)
        => array.SafeArray;

    public ImmutableArray<T>.Enumerator GetEnumerator()
    {
        return SafeArray.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)SafeArray).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)SafeArray).GetEnumerator();
    }
}
