#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace RoseLynn.Utilities;

/// <summary>Represents a <seealso cref="IReadOnlyCollection{T}"/> that contains a single element.</summary>
/// <typeparam name="T">The type of the collection</typeparam>
public struct SingleElementCollection<T> : IReadOnlyCollection<T?>, IEquatable<SingleElementCollection<T?>>
{
    private readonly T? element;

    /// <inheritdoc/>
    public int Count => 1;

    /// <summary>Initializes a new instance of the <seealso cref="SingleElementCollection{T}"/> struct, out of the single element that will be contained in the collection.</summary>
    /// <param name="singleElement">The single element that will be contained in the collection. It may be <see langword="null"/>.</param>
    public SingleElementCollection(T? singleElement)
    {
        element = singleElement;
    }
    /// <summary>Initializes a new instance of the <seealso cref="SingleElementCollection{T}"/> struct, out of another <see cref="SingleElementCollection{T}"/>.</summary>
    /// <param name="other">The other <seealso cref="SingleElementCollection{T}"/> out of which to initialize this new collection.</param>
    public SingleElementCollection(SingleElementCollection<T?> other)
    {
        element = other.element;
    }

    /// <summary>Constructs a new <seealso cref="SingleElementCollection{T}"/> that contains the provided element.</summary>
    /// <param name="newElement">The new element that will be contained</param>
    /// <returns></returns>
    public SingleElementCollection<T?> WithElement(T? newElement) => new(newElement);

    /// <inheritdoc/>
    public IEnumerator<T?> GetEnumerator() => new SingleElementEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public bool Equals(SingleElementCollection<T?> other)
    {
        return Equals(element, other.element);
    }
    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
    }
    /// <summary>Gets the hash code of the current collection, which is determined by its only contained element.</summary>
    /// <returns>The hash code of its only contained element, if not <see langword="null"/>, otherwise <see langword="default"/>.</returns>
    public override int GetHashCode() => element?.GetHashCode() ?? default;

    public static bool operator ==(SingleElementCollection<T?> left, SingleElementCollection<T?> right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(SingleElementCollection<T?> left, SingleElementCollection<T?> right)
    {
        return !(left == right);
    }

    private struct SingleElementEnumerator : IEnumerator<T?>
    {
        private readonly SingleElementCollection<T?> instance;
        private EnumerationState state;

        public SingleElementEnumerator(SingleElementCollection<T?> collection)
        {
            instance = collection;
            state = EnumerationState.Before;
        }

        object? IEnumerator.Current => Current;
        public T? Current => state switch
        {
            EnumerationState.Current => instance.element,

            // That's a lot of errors
            EnumerationState.Before => throw new InvalidOperationException("The enumeration has not yet begun."),
            EnumerationState.After => throw new InvalidOperationException("The single element has already been enumerated."),
            _ => throw new InvalidEnumArgumentException("The current state is invalid. There exists a memory corruption."),
        };

        public bool MoveNext()
        {
            if (state is EnumerationState.After)
                return false;

            state++;
            return state < EnumerationState.After;
        }
        public void Reset()
        {
            state = EnumerationState.Before;
        }

        public void Dispose() { }

        private enum EnumerationState : byte
        {
            Before,
            Current,
            After
        }
    }
}

#nullable enable
public static class SingleElementCollectionExtensions
{
    public static SingleElementCollection<T> ToSingleElementCollection<T>(this T element)
    {
        return new(element);
    }
}
