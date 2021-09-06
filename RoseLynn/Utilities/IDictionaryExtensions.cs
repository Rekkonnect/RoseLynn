#nullable enable

using System.Collections.Generic;

namespace RoseLynn.Utilities
{
    /// <summary>Provides useful extensions for the <seealso cref="IDictionary{TKey, TValue}"/> type.</summary>
    public static class IDictionaryExtensions
    {
        /// <summary>Gets the value mapped to the given key within the dictionary, if the key is present.</summary>
        /// <typeparam name="TKey">The type of the keys stored in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary whose mapped value to get.</param>
        /// <param name="key">The key whose mapped value to get.</param>
        /// <returns>The associated value to <paramref name="key"/>, if it exists, otherwise <see langword="default"/>.</returns>
        public static TValue? ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey? key)
        {
            if (key is null)
                return default;

            if (!dictionary.TryGetValue(key, out var value))
                return default;

            return value;
        }

        /// <summary>Adds a new entry to the dictionary. If the given key already exists, its value is overwritten in the source dictionary.</summary>
        /// <typeparam name="TKey">The type of the keys stored in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="key">The key of the entry to add or overwrite.</param>
        /// <param name="value">The value of the entry to set.</param>
        /// <returns><see langword="true"/> if the entry already existed with a different value and was overwritten, otherwise <see langword="false"/>.</returns>
        public static bool AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            var contained = source.TryGetValue(key, out var oldValue);
            
            if (!contained)
                source.Add(key, value);
            else
            {
                if (oldValue.Equals(value))
                    return false;

                source[key] = value;
            }

            return contained;
        }
        /// <summary>Adds a new entry to the dictionary. If the given key already exists, its value is preserved in the source dictionary.</summary>
        /// <typeparam name="TKey">The type of the keys stored in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="key">The key of the entry to add or overwrite.</param>
        /// <param name="value">The value of the entry to set.</param>
        /// <returns><see langword="true"/> if the entry did not exist, or existed with the same value, otherwise <see langword="false"/>.</returns>
        public static bool TryAddPreserve<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            var available = !source.TryGetValue(key, out var oldValue);

            if (available)
                source.Add(key, value);
            else
            {
                if (oldValue.Equals(value))
                    return true;
            }

            return available;
        }

        /// <summary>Adds a range of entries to a source dictionary.</summary>
        /// <typeparam name="TKey">The type of the keys stored in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="entries">The entries to add to the <paramref name="source"/> dictionary.</param>
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> entries)
        {
            foreach (var entry in entries)
                source.Add(entry.Key, entry.Value);
        }
        /// <summary>Adds a collection of new entries to the dictionary. For each of the given entries, if its key already exists, its value is overwritten in the source dictionary.</summary>
        /// <typeparam name="TKey">The type of the keys stored in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="entries">The entries to add or overwrite.</param>
        /// <returns><see langword="true"/> if at least one of the given entries already existed with a different value and was overwritten, otherwise <see langword="false"/>.</returns>
        public static bool AddOrSetRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> entries)
        {
            bool overwritten = false;
            foreach (var entry in entries)
                overwritten |= source.AddOrSet(entry.Key, entry.Value);
            return overwritten;
        }
        /// <summary>Adds a collection of new entries to the dictionary. For each of the given entries, if its key already exists, its value is preserved in the source dictionary.</summary>
        /// <typeparam name="TKey">The type of the keys stored in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="entries">The entries to add or overwrite.</param>
        /// <returns><see langword="true"/> if none of the given entries already existed with a different value, otherwise <see langword="false"/>.</returns>
        public static bool TryAddPreserveRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> entries)
        {
            bool preserved = true;
            foreach (var entry in entries)
                preserved &= source.TryAddPreserve(entry.Key, entry.Value);
            return preserved;
        }

        /// <summary>Gets a <seealso cref="KeyValuePair{TKey, TValue}"/> instance containing the specified key and its associated value.</summary>
        /// <typeparam name="TKey">The type of the keys stored in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="key">The key of the entry whose <seealso cref="KeyValuePair{TKey, TValue}"/> to get.</param>
        /// <returns>The resulting <seealso cref="KeyValuePair{TKey, TValue}"/> containing the entry that was requested.</returns>
        public static KeyValuePair<TKey, TValue> GetKeyValuePair<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return new KeyValuePair<TKey, TValue>(key, source[key]);
        }
    }
}
