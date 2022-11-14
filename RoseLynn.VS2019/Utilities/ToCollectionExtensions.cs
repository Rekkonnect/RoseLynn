using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace RoseLynn.Utilities;

/// <summary>Contains extensions regarding converting an <seealso cref="IEnumerable{T}"/> into a collection.</summary>
public static class ToCollectionExtensions
{
    public static T[] ToArrayOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source?.ToArray() ?? Array.Empty<T>();
    }
    public static List<T> ToListOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source?.ToList() ?? new List<T>();
    }
}
