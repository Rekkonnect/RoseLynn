using Polyfills;
using System.Text;

namespace RoseLynn.Writing;

/// <summary>Provides helpers for normalizing and tokenizing string newline content.</summary>
public static class StringExtensions
{
    extension(string text)
    {
        /// <summary>Normalizes all newlines in the text to the current environment newline sequence.</summary>
        /// <returns>The normalized string.</returns>
        public string NormalizeEnvironmentNewLines()
        {
            return text.NormalizeNewLines(Environment.NewLine);
        }

        /// <summary>Normalizes all newlines in the text to a specified newline sequence.</summary>
        /// <param name="newLine">The newline sequence to substitute.</param>
        /// <returns>The normalized string.</returns>
        public string NormalizeNewLines(string newLine)
        {
            var builder = new StringBuilder((int)(text.Length * 1.2));

            foreach (var token in text.EnumerateLineTokens())
            {
                if (token.Kind is LineToken.TokenKind.NewLine)
                {
                    builder.Append(newLine);
                    continue;
                }

                builder.Append(token.Span);
            }

            return builder.ToString();
        }

        /// <summary>Enumerates the text as alternating content and newline tokens.</summary>
        /// <returns>An enumerator over line-content and newline tokens.</returns>
        public StringLineTokenEnumerator EnumerateLineTokens()
        {
            return new(text.AsSpan());
        }
    }
}
