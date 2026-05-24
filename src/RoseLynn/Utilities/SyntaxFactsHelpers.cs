using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLynn.Utilities;

public static class SyntaxFactsHelpers
{
    public static string ValidIdentifierOrDefault(string identifier, string @default)
    {
        if (SyntaxFacts.IsValidIdentifier(identifier))
        {
            return identifier;
        }

        return @default;
    }
}
