namespace RoseLynn;

public record class SyntaxWithTriviaBank<TSyntax>(
    TSyntax Syntax,
    TriviaBank TriviaBank = default)
{
    public static implicit operator SyntaxWithTriviaBank<TSyntax>(TSyntax t)
    {
        return new(t);
    }
}
