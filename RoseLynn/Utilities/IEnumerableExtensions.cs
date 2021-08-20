using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.Utilities
{
    public static class IEnumerableExtensions
    {
        /// <summary>Gets the only element of the sequence, if it only has one element, otherwise returns <see langword="default"/>.</summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>The only element of the sequence, or <see langword="default"/>.</returns>
        public static T OnlyOrDefault<T>(this IEnumerable<T> source)
        {
            if (source is null)
                return default;

            T first = default;
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
    }
}
