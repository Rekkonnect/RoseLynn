using Microsoft.CodeAnalysis;

namespace RoseLynn;

public static class OptionalExtensions
{
    extension<T>(Optional<T> optional)
    {
        public bool Equals(Optional<T> other)
        {
            return optional.HasValue == other.HasValue
                && (optional.HasValue
                    ? EqualityComparer<T>.Default.Equals(optional.Value, other.Value)
                    : true);
        }

        public static bool operator ==(Optional<T> x, Optional<T> y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Optional<T> x, Optional<T> y)
        {
            return !x.Equals(y);
        }
    }
}
