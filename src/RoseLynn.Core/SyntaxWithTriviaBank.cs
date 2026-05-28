namespace RoseLynn.Core;

/// <summary>Pairs a syntax node with an associated <seealso cref="TriviaBank"/> for deferred trivia attachment.</summary>
/// <typeparam name="TSyntax">The syntax node type.</typeparam>
/// <param name="Syntax">The syntax node.</param>
/// <param name="TriviaBank">The trivia bank associated with the syntax node.</param>
public record class SyntaxWithTriviaBank<TSyntax>(
    TSyntax Syntax,
    TriviaBank TriviaBank = default)
{
    /// <summary>Implicitly converts a syntax node to a <seealso cref="SyntaxWithTriviaBank{TSyntax}"/> with a default trivia bank.</summary>
    /// <param name="t">The syntax node to wrap.</param>
    /// <returns>A new instance wrapping <paramref name="t"/>.</returns>
    public static implicit operator SyntaxWithTriviaBank<TSyntax>(TSyntax t)
    {
        return new(t);
    }
}
