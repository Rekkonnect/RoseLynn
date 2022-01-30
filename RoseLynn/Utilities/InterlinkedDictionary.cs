#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RoseLynn.Utilities;

/// <summary>Represents a collection of interlinked key-value pairs, where both the key and the value are mapped to each other. This means that the ability to get the mapping key from the mapped value is enabled.</summary>
/// <typeparam name="T1">The type of the first component of the pairs.</typeparam>
/// <typeparam name="T2">The type of the second component of the pairs.</typeparam>
public class InterlinkedDictionary<T1, T2>
{
    private readonly Dictionary<T1, T2> t1Dictionary;
    private readonly Dictionary<T2, T1> t2Dictionary;

    /// <summary>Gets the count of pairs that are present in the dictionary.</summary>
    public int Count => t1Dictionary.Count;

    /// <summary>Gets all the currently stored values of type <typeparamref name="T1"/>.</summary>
    public IEnumerable<T1> Values1 => t1Dictionary.Keys;
    /// <summary>Gets all the currently stored values of type <typeparamref name="T2"/>.</summary>
    public IEnumerable<T2> Values2 => t2Dictionary.Keys;
    /// <summary>Gets all the currently stored pairs of values.</summary>
    [ExcludeFromCodeCoverage]
    public IEnumerable<(T1, T2)> ValuePairs => t1Dictionary.Keys.Zip(t2Dictionary.Keys, Selectors.MakeTuple);

    /// <summary>Initializes a new empty <seealso cref="InterlinkedDictionary{T1, T2}"/>.</summary>
    public InterlinkedDictionary()
    {
        t1Dictionary = new();
        t2Dictionary = new();
    }
    /// <summary>Initializes a new <seealso cref="InterlinkedDictionary{T1, T2}"/> out of another, copying the pairs to this new instance.</summary>
    /// <param name="other">The other <see cref="InterlinkedDictionary{T1, T2}"/> from which to copy the pairs. That instance remains unaffected upon performing operations on this new one.</param>
    public InterlinkedDictionary(InterlinkedDictionary<T1, T2> other)
    {
        t1Dictionary = new(other.t1Dictionary);
        t2Dictionary = new(other.t2Dictionary);
    }

    /// <summary>Determines whether a value is contained.</summary>
    /// <param name="value">The value of type <typeparamref name="T1"/> that may be contained.</param>
    /// <returns>A value determining whether the value is contained.</returns>
    public bool Contains(T1 value) => t1Dictionary.ContainsKey(value);
    /// <summary>Determines whether a value is contained.</summary>
    /// <param name="value">The value of type <typeparamref name="T2"/> that may be contained.</param>
    /// <returns>A value determining whether the value is contained.</returns>
    public bool Contains(T2 value) => t2Dictionary.ContainsKey(value);

    /// <summary>Adds a pair of values to the dictionary. If any of the values already exists, </summary>
    /// <param name="t1Value">The value of type <typeparamref name="T1"/> to add to the dictionary.</param>
    /// <param name="t2Value">The value of type <typeparamref name="T2"/> to add to the dictionary.</param>
    /// <exception cref="ArgumentException">Thrown if one of the values already exists in the dictionary.</exception>
    /// <exception cref="ArgumentNullException">Thrown if any of the values is <see langword="null"/>.</exception>
    public void Add(T1 t1Value, T2 t2Value)
    {
        if (t1Value is null || t2Value is null)
            throw new ArgumentNullException($"One of the provided values was null (passed values: {t1Value}, {t2Value})");

        AssertAlreadyExisting(t1Dictionary, t1Value);
        AssertAlreadyExisting(t2Dictionary, t2Value);

        t1Dictionary.Add(t1Value, t2Value);
        t2Dictionary.Add(t2Value, t1Value);

        static void AssertAlreadyExisting<A, B>(Dictionary<A, B> dictionary, A key)
        {
            if (dictionary.ContainsKey(key))
                throw new ArgumentException($"The value {key} already exists.");
        }
    }

    /// <summary>Removes a from the dictionary, if it exists, otherwise nothing happens.</summary>
    /// <param name="t1Value">The value of type <typeparamref name="T1"/> that will be removed.</param>
    /// <returns><see langword="true"/> if the value was found and successfully removed from the dictionary, otherwise <see langword="false"/>.</returns>
    public bool Remove(T1 t1Value)
    {
        if (!t1Dictionary.TryGetValue(t1Value, out var t2Value))
            return false;

        return Remove(t1Value, t2Value);
    }
    /// <summary>Removes a from the dictionary, if it exists, otherwise nothing happens.</summary>
    /// <param name="t2Value">The value of type <typeparamref name="T2"/> that will be removed.</param>
    /// <returns><see langword="true"/> if the value was found and successfully removed from the dictionary, otherwise <see langword="false"/>.</returns>
    public bool Remove(T2 t2Value)
    {
        if (!t2Dictionary.TryGetValue(t2Value, out var t1Value))
            return false;

        return Remove(t1Value, t2Value);
    }

    private bool Remove(T1 t1Value, T2 t2Value)
    {
        t1Dictionary.Remove(t1Value);
        t2Dictionary.Remove(t2Value);
        return true;
    }

    /// <summary>Clears the dictionary, removing all pairs of values that are currently stored.</summary>
    public void Clear()
    {
        t1Dictionary.Clear();
        t2Dictionary.Clear();
    }

    /// <summary>Attempts to get the paired value of a value of type <typeparamref name="T1"/>.</summary>
    /// <param name="t1Key">The value of type <typeparamref name="T1"/> whose paired value to get.</param>
    /// <param name="t2Value">The paired value of type <typeparamref name="T2"/>, if <paramref name="t1Key"/> exists in the dictionary, otherwise <see langword="default"/>.</param>
    /// <returns><see langword="true"/> if the value was present in the dictionary, otherwise <see langword="false"/>.</returns>
    public bool TryGetValue(T1 t1Key, out T2 t2Value)
    {
        return t1Dictionary.TryGetValue(t1Key, out t2Value);
    }
    /// <summary>Attempts to get the paired value of a value of type <typeparamref name="T2"/>.</summary>
    /// <param name="t2Key">The value of type <typeparamref name="T2"/> whose paired value to get.</param>
    /// <param name="t1Value">The paired value of type <typeparamref name="T1"/>, if <paramref name="t2Key"/> exists in the dictionary, otherwise <see langword="default"/>.</param>
    /// <returns><see langword="true"/> if the value was present in the dictionary, otherwise <see langword="false"/>.</returns>
    public bool TryGetValue(T2 t2Key, out T1 t1Value)
    {
        return t2Dictionary.TryGetValue(t2Key, out t1Value);
    }

    /// <summary>Attempts to get the paired value of a value of type <typeparamref name="T1"/>.</summary>
    /// <param name="key">The value of type <typeparamref name="T1"/> whose paired value to get.</param>
    /// <returns>The paired value of type <typeparamref name="T2"/>, if the value of type <typeparamref name="T1"/> exists, otherwise <see langword="default"/>.</returns>
    public T2? ValueOrDefault(T1 key)
    {
        TryGetValue(key, out var value);
        return value;
    }
    /// <summary>Attempts to get the paired value of a value of type <typeparamref name="T2"/>.</summary>
    /// <param name="key">The value of type <typeparamref name="T2"/> whose paired value to get.</param>
    /// <returns>The paired value of type <typeparamref name="T1"/>, if the value of type <typeparamref name="T2"/> exists, otherwise <see langword="default"/>.</returns>
    public T1? ValueOrDefault(T2 key)
    {
        TryGetValue(key, out var value);
        return value;
    }

    /// <summary>Gets or sets the paired value of a value of type <typeparamref name="T2"/>.</summary>
    /// <param name="t1Key">The value of type <typeparamref name="T1"/> whose paired value to get or set.</param>
    /// <returns>The paired value of type <typeparamref name="T2"/>, if the value of type <typeparamref name="T1"/> exists.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the given value of type <typeparamref name="T1"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if the given value of type <typeparamref name="T1"/> does not exist in the dictionary.</exception>
    public T2 this[T1 t1Key]
    {
        get => t1Dictionary[t1Key];
        set => ChangeValue(t1Dictionary, t2Dictionary, t1Key, value);
    }
    /// <summary>Gets or sets the paired value of a value of type <typeparamref name="T1"/>.</summary>
    /// <param name="t2Key">The value of type <typeparamref name="T2"/> whose paired value to get or set.</param>
    /// <returns>The paired value of type <typeparamref name="T1"/>, if the value of type <typeparamref name="T2"/> exists.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the given value of type <typeparamref name="T2"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if the given value of type <typeparamref name="T2"/> does not exist in the dictionary.</exception>
    public T1 this[T2 t2Key]
    {
        get => t2Dictionary[t2Key];
        set => ChangeValue(t2Dictionary, t1Dictionary, t2Key, value);
    }

    private static void ChangeValue<TFocused, TOther>(Dictionary<TFocused, TOther> focusedDictionary, Dictionary<TOther, TFocused> otherDictionary, TFocused key, TOther value)
    {
        var oldValue = focusedDictionary[key];
        focusedDictionary[key] = value;
        otherDictionary.Remove(oldValue);
        otherDictionary.Add(value, key);
    }
}
