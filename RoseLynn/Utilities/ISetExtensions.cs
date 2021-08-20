using System.Collections.Generic;

namespace RoseLynn.Utilities
{
    public static class ISetExtensions
    {
        public static void AddRange<T>(this ISet<T> set, IEnumerable<T> elements)
        {
            set.UnionWith(elements);
        }
        public static void RemoveRange<T>(this ISet<T> set, IEnumerable<T> elements)
        {
            set.IntersectWith(elements);
        }
        public static bool ContainsAll<T>(this ISet<T> set, IEnumerable<T> elements)
        {
            return set.IsSupersetOf(elements);
        }    
    }
}
