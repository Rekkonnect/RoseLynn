using System;
using System.Linq;

namespace RoseLynn.Utilities;

/// <summary>Provides helper functions for enums.</summary>
public static class EnumHelpers
{
    /// <summary>Gets the values of an enum type.</summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <returns>An array of type <typeparamref name="T"/>[] which contains all the values contained in the <typeparamref name="T"/> enum.</returns>
    public static T[] GetValues<T>()
        where T : struct, Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
    }
    
    /// <summary>Parses the value of an enum type.</summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="name">The name of the field in the enum.</param>
    /// <returns>The parsed value cast to the enum type that was specified.</returns>
    public static T Parse<T>(string name)
        where T : struct, Enum
    {
        return (T)Enum.Parse(typeof(T), name);
    }
}
