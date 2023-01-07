using Microsoft.CodeAnalysis.Text;

namespace RoseLynn.Utilities;

#nullable enable annotations

/// <summary>Provides useful extensions for the <seealso cref="string"/> type.</summary>
public static class StringExtensions
{
    /// <summary>Removes all instances of the given sequence from the string.</summary>
    /// <param name="origin">The original string from which to remove all instances of the given sequence.</param>
    /// <param name="sequence">The sequence which to remove from the original string.</param>
    /// <returns>The resulting string with all instances of the given sequence removed.</returns>
    public static string Remove(this string origin, string sequence) => origin.Replace(sequence, "");

    /// <summary>Returns a substring of the original string at the specified span.</summary>
    /// <param name="origin">The original string whose substring to get.</param>
    /// <param name="span">The span of the string to get.</param>
    /// <returns>The substring ranging in the span that was specified.</returns>
    public static string Substring(this string origin, TextSpan span)
    {
        return origin.Substring(span.Start, span.Length);
    }

    /// <summary>
    /// Evaluates a string such that the returned result will begin with the specified prefix,
    /// without concatenating it if it is already present.
    /// </summary>
    /// <param name="source">The source string to which to guarantee a prefix.</param>
    /// <param name="prefix">The prefix that will be contained in the resulting string.</param>
    /// <returns>
    /// The prefix itself if the source string is null or empty, or the source string if it already
    /// contains the prefix, otherwise a concatenation of the prefix and the source string.
    /// </returns>
    public static string EnsureStartsWith(this string? source, string prefix)
    {
        if (string.IsNullOrEmpty(source))
            return prefix;

        bool startsWithPrefix = source.StartsWith(prefix);
        if (!startsWithPrefix)
            source = $"{prefix}{source}";
        return source;
    }

    /// <summary>
    /// Trims the first newline of the string, if there is any.
    /// </summary>
    /// <param name="s">The string that may start with a newline.</param>
    /// <returns>
    /// The substring past the first newline, if it starts with one,
    /// or the string itself if it's null, empty or it doesn't start with a newline.
    /// </returns>
    /// <remarks>Due to the variance in the newlines that may be used, up to two characters may be evaluated.</remarks>
    public static string? TrimFirstNewLine(this string? s)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        if (s.StartsWith("\r\n"))
            return s.Substring(2);

        return s[0] switch
        {
            '\r' or
            '\n' => s.Substring(1),

            _ => s,
        };
    }
}
