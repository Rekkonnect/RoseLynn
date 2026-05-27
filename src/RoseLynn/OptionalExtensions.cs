using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Provides equality operations for <seealso cref="Optional{T}"/> values.</summary>
public static class OptionalExtensions
{
    extension<T>(Optional<T> optional)
    {
        /// <summary>Determines whether this optional equals <paramref name="other"/> by comparing presence and value.</summary>
        /// <param name="other">The optional to compare against.</param>
        /// <returns><see langword="true"/> when both optionals have the same presence state and equal values.</returns>
        public bool Equals(Optional<T> other)
        {
            return optional.HasValue == other.HasValue
                && (optional.HasValue
                    ? EqualityComparer<T>.Default.Equals(optional.Value, other.Value)
                    : true);
        }

        /// <summary>Returns <see langword="true"/> if <paramref name="x"/> and <paramref name="y"/> are equal.</summary>
        public static bool operator ==(Optional<T> x, Optional<T> y)
        {
            return x.Equals(y);
        }

        /// <summary>Returns <see langword="true"/> if <paramref name="x"/> and <paramref name="y"/> are not equal.</summary>
        public static bool operator !=(Optional<T> x, Optional<T> y)
        {
            return !x.Equals(y);
        }
    }
}
