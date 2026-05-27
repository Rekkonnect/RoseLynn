using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn.CSharp.Utilities;

/// <summary>Provides helper utilities that wrap <seealso cref="SyntaxFacts"/> with safe fallback behaviour.</summary>
public static class SyntaxFactsHelpers
{
    /// <summary>Returns <paramref name="identifier"/> if it is a valid C# identifier according to <seealso cref="SyntaxFacts.IsValidIdentifier"/>; otherwise returns <paramref name="default"/>.</summary>
    /// <param name="identifier">The string to validate.</param>
    /// <param name="default">The fallback value when the identifier is invalid.</param>
    public static string ValidIdentifierOrDefault(string identifier, string @default)
    {
        if (SyntaxFacts.IsValidIdentifier(identifier))
        {
            return identifier;
        }

        return @default;
    }
}
