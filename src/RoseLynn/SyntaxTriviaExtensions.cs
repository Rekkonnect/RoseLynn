using Microsoft.CodeAnalysis;

namespace RoseLynn;

public static class SyntaxTriviaExtensions
{
    public static SyntaxTriviaList Prepend(this SyntaxTriviaList trivia, SyntaxTriviaList @new)
    {
        if (@new is [])
        {
            return trivia;
        }

        return trivia.InsertRange(0, @new);
    }
}
