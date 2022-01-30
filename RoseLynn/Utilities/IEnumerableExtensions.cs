#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.Utilities;

/// <summary>Provides useful extensions for the <seealso cref="IEnumerable{T}"/> type.</summary>
public static class IEnumerableExtensions
{
    /// <summary>Gets the only element of the sequence, if it only has one element, otherwise returns <see langword="default"/>.</summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>The only element of the sequence, or <see langword="default"/>.</returns>
    public static T? OnlyOrDefault<T>(this IEnumerable<T> source)
    {
        if (source is null)
            return default;

        T? first = default;
        bool hasElements = false;

        foreach (var s in source)
        {
            if (hasElements)
                return default;

            first = s;
            hasElements = true;
        }

        return first;
    }

    /// <summary>Determines whether there are any elements contained in any sequence of the sequences that are provided.</summary>
    /// <typeparam name="T">The type of elements stored in the sequences.</typeparam>
    /// <param name="source">The sequence of sequences to analyze on whether any elements are contained.</param>
    /// <returns><see langword="true"/> if there is at least one element in any of the sequences, otherwise <see langword="false"/>.</returns>
    public static bool AnyDeep<T>(this IEnumerable<IEnumerable<T>> source)
    {
        foreach (var e in source)
            if (e.Any())
                return true;
        return false;
    }

    /// <summary>Performs an action for each element in a collection, while also performing an additional specified action for the first and the last elements.</summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="source">The source collection whose elements to perform the actions on. If the collection is empty, no action will be taken.</param>
    /// <param name="firstElementAction">The action to perform on the first element.</param>
    /// <param name="lastElementAction">The action to perform on the last element.</param>
    /// <param name="enumerationAction">The action to perform on all enuemrated elements. This is also applied to the first and the last elements.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> contained at least one element and actions were performed, otherwise <see langword="false"/>.</returns>
    public static bool EnumeratePerformAction<T>(this IEnumerable<T> source, Action<T>? firstElementAction, Action<T>? lastElementAction, Action<T> enumerationAction)
    {
        var enumerator = source.GetEnumerator();

        // If there are no elements in the source, early exist without performing actions
        if (!enumerator.MoveNext())
            return false;

        var first = enumerator.Current;
        if (firstElementAction is not null)
            firstElementAction(first);

        T last;

        do
        {
            last = enumerator.Current;
            enumerationAction(last);
        }
        while (enumerator.MoveNext());

        if (lastElementAction is not null)
            lastElementAction(last);

        return true;
    }
}
