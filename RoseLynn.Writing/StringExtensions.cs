using Polyfills;
using System;
using System.Linq;
using System.Text;

namespace RoseLynn.Writing;

public static class StringExtensions
{
    extension(string text)
    {
        public string NormalizeEnvironmentNewLines()
        {
            return text.NormalizeNewLines(Environment.NewLine);
        }

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

        public StringLineTokenEnumerator EnumerateLineTokens()
        {
            return new(text.AsSpan());
        }
    }
}
