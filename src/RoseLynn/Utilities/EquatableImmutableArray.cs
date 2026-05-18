
using Garyon.Extensions.Comparison;
using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace RoseLynn.Utilities;

[CollectionBuilder(
    typeof(EquatableImmutableArrayBuilder),
    nameof(EquatableImmutableArrayBuilder.Create))]
/// <summary>Wraps an <see cref="ImmutableArray{T}"/> with structural equality semantics and safe access helpers.</summary>
/// <param name="Array">The underlying immutable array value.</param>
public readonly record struct EquatableImmutableArray<T>(ImmutableArray<T> Array)
    : IEquatable<EquatableImmutableArray<T>>, IEnumerable<T>
{
    /// <summary>Gets a non-default array instance for safe enumeration and indexing.</summary>
    public ImmutableArray<T> SafeArray => Array.Safe();

    /// <summary>Gets the length of the safe array value.</summary>
    public int Length => SafeArray.Length;

    /// <summary>Compares two equatable immutable arrays for ordering.</summary>
    /// <param name="other">The array to compare against.</param>
    /// <returns>A comparison result based on length and hash code.</returns>
    public int CompareTo(EquatableImmutableArray<T> other)
    {
        return SafeArray.BeginCompare(other.SafeArray)
            .By(s => s.Length)
            .ThenBy(s => s.GetHashCode())
            .Result
            ;
    }

    /// <summary>Compares the array against an arbitrary object.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>A comparison result, or <c>1</c> when the object is not a compatible array wrapper.</returns>
    public int CompareTo(object obj)
    {
        if (obj is EquatableImmutableArray<T> other)
        {
            return CompareTo(other);
        }

        return 1;
    }

    /// <summary>Determines whether two wrapped arrays contain the same sequence of values.</summary>
    /// <param name="other">The array to compare against.</param>
    /// <returns><see langword="true"/> if both arrays contain the same values in the same order.</returns>
    public bool Equals(EquatableImmutableArray<T> other)
    {
        return SafeArray.SequenceEqual(other.SafeArray);
    }

    /// <summary>Gets a hash code based on the contained elements.</summary>
    /// <returns>The computed hash code.</returns>
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

    /// <summary>Converts an <see cref="ImmutableArray{T}"/> to its equatable wrapper.</summary>
    public static implicit operator EquatableImmutableArray<T>(ImmutableArray<T> array)
        => new(array);
    /// <summary>Converts the wrapper back to a safe immutable array.</summary>
    public static implicit operator ImmutableArray<T>(EquatableImmutableArray<T> array)
        => array.SafeArray;

    /// <summary>Gets the value-type enumerator for the safe array.</summary>
    /// <returns>An enumerator over the wrapped array.</returns>
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
