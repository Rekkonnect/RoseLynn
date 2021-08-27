using Microsoft.CodeAnalysis.Text;

namespace RoseLynn.Utilities
{
    public static class StringExtensions
    {
        /// <summary>Removes all instances of the given sequence from the string.</summary>
        /// <param name="origin">The original string from which to remove all instances of the given sequence.</param>
        /// <param name="sequence">The sequence which to remove from the original string.</param>
        /// <returns>The resulting string with all instances of the given sequence removed.</returns>
        public static string Remove(this string origin, string sequence) => origin.Replace(sequence, "");

        // Missing crucial overloads for a more complete API
        /// <summary>Gets the index of the first character after the first occurrence of the given sequence in the original string.</summary>
        /// <param name="origin">The original string from which to get the first occurrence.</param>
        /// <param name="sequence">The sequence to find the first occurrence of.</param>
        /// <param name="startIndex">The index of the first character to include to the search, from which onwards the search will be performed.</param>
        /// <returns>The index of the first character after the first occurrence of <paramref name="sequence"/> in <paramref name="origin"/>, if found, otherwise -1.</returns>
        public static int IndexOfAfter(this string origin, string sequence, int startIndex)
        {
            int index = origin.IndexOf(sequence, startIndex);
            return index is -1 ? -1 : index + sequence.Length;
        }

        /// <summary>Returns a substring of the original string at the specified span.</summary>
        /// <param name="origin">The original string whose substring to get.</param>
        /// <param name="span">The span of the string to get.</param>
        /// <returns>The substring ranging in the span that was specified.</returns>
        public static string Substring(this string origin, TextSpan span)
        {
            return origin.Substring(span.Start, span.Length);
        }
    }
}
