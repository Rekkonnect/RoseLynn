using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace RoseLynn.Utilities;

/// <summary>Provides useful extensions for the <seealso cref="StringBuilder"/> type.</summary>
public static class StringBuilderExtensions
{
    /// <summary>Appends a string to the <seealso cref="StringBuilder"/> instance, from its start index up until its end.</summary>
    /// <param name="stringBuilder">The <seealso cref="StringBuilder"/> instance on which to append the string.</param>
    /// <param name="value">The string to append to the <seealso cref="StringBuilder"/> from the specified index onwards.</param>
    /// <param name="startIndex">The index of the first character to append to the <seealso cref="StringBuilder"/>.</param>
    /// <returns>The <seealso cref="StringBuilder"/> instance that the string was appended to.</returns>
    public static StringBuilder Append(this StringBuilder stringBuilder, string value, int startIndex)
    {
        return stringBuilder.Append(value, startIndex, value.Length - startIndex);
    }
    /// <summary>Appends a span of a string to the <seealso cref="StringBuilder"/> instance.</summary>
    /// <param name="stringBuilder">The <seealso cref="StringBuilder"/> instance on which to append the string.</param>
    /// <param name="value">The string whose specified span to append to the <seealso cref="StringBuilder"/>.</param>
    /// <param name="span">The span of the string to append to the <seealso cref="StringBuilder"/>.</param>
    /// <returns>The <seealso cref="StringBuilder"/> instance that the string was appended to.</returns>
    public static StringBuilder Append(this StringBuilder stringBuilder, string value, TextSpan span)
    {
        return stringBuilder.Append(value, span.Start, span.Length);
    }
}
