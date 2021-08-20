using System;
using System.Linq;

namespace RoseLynn.Utilities
{
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
    }
}
