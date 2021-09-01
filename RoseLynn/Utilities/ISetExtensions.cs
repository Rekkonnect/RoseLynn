using System.Collections.Generic;

namespace RoseLynn.Utilities
{
    /// <summary>Provides useful extensions for the <seealso cref="ISet{T}"/> type.</summary>
    public static class ISetExtensions
    {
        /// <inheritdoc cref="ISet{T}.UnionWith(IEnumerable{T})"/>
        /// <remarks>This extension acts as a friendlier alias for <seealso cref="ISet{T}.UnionWith(IEnumerable{T})"/>.</remarks>
        public static void AddRange<T>(this ISet<T> set, IEnumerable<T> other)
        {
            set.UnionWith(other);
        }
        /// <inheritdoc cref="ISet{T}.IntersectWith(IEnumerable{T})"/>
        /// <remarks>This extension acts as a friendlier alias for <seealso cref="ISet{T}.IntersectWith(IEnumerable{T})"/>.</remarks>
        public static void RemoveRange<T>(this ISet<T> set, IEnumerable<T> other)
        {
            set.IntersectWith(other);
        }
        /// <inheritdoc cref="ISet{T}.IsSupersetOf(IEnumerable{T})"/>
        /// <remarks>This extension acts as a friendlier alias for <seealso cref="ISet{T}.IsSupersetOf(IEnumerable{T})"/>.</remarks>
        public static bool ContainsAll<T>(this ISet<T> set, IEnumerable<T> other)
        {
            return set.IsSupersetOf(other);
        }    
    }
}
