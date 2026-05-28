using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Provides extension methods for <seealso cref="SyntaxTriviaList"/>.</summary>
public static class SyntaxTriviaExtensions
{
    /// <summary>Returns a new list with <paramref name="new"/> inserted at the beginning of <paramref name="trivia"/>.</summary>
    /// <param name="trivia">The list to prepend to.</param>
    /// <param name="new">The trivia to insert at position 0.</param>
    /// <returns>The combined list, or the original <paramref name="trivia"/> if <paramref name="new"/> is empty.</returns>
    public static SyntaxTriviaList Prepend(this SyntaxTriviaList trivia, SyntaxTriviaList @new)
    {
        if (@new is [])
        {
            return trivia;
        }

        return trivia.InsertRange(0, @new);
    }
}
