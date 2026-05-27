using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;

namespace RoseLynn.CSharp;

/// <summary>Provides extension methods for removing a specific modifier token from C# declaration syntax nodes while preserving surrounding trivia.</summary>
public static class RemoveModifiersExtensions
{
    /// <summary>Returns a copy of the method declaration with the specified modifier removed.</summary>
    /// <param name="method">The method declaration to modify.</param>
    /// <param name="modifierKind">The <seealso cref="SyntaxKind"/> of the modifier to remove.</param>
    public static MethodDeclarationSyntax RemoveModifier(
        this MethodDeclarationSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    /// <summary>Returns a copy of the local function declaration with the specified modifier removed.</summary>
    /// <param name="method">The local function declaration to modify.</param>
    /// <param name="modifierKind">The <seealso cref="SyntaxKind"/> of the modifier to remove.</param>
    public static LocalFunctionStatementSyntax RemoveModifier(
        this LocalFunctionStatementSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    /// <summary>Returns a copy of the simple lambda expression with the specified modifier removed.</summary>
    /// <param name="method">The lambda expression to modify.</param>
    /// <param name="modifierKind">The <seealso cref="SyntaxKind"/> of the modifier to remove.</param>
    public static SimpleLambdaExpressionSyntax RemoveModifier(
        this SimpleLambdaExpressionSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    /// <summary>Returns a copy of the parenthesized lambda expression with the specified modifier removed.</summary>
    /// <param name="method">The lambda expression to modify.</param>
    /// <param name="modifierKind">The <seealso cref="SyntaxKind"/> of the modifier to remove.</param>
    public static ParenthesizedLambdaExpressionSyntax RemoveModifier(
        this ParenthesizedLambdaExpressionSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    /// <summary>Returns a copy of the anonymous method expression with the specified modifier removed.</summary>
    /// <param name="method">The anonymous method expression to modify.</param>
    /// <param name="modifierKind">The <seealso cref="SyntaxKind"/> of the modifier to remove.</param>
    public static AnonymousMethodExpressionSyntax RemoveModifier(
        this AnonymousMethodExpressionSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    /// <summary>Returns a copy of the property declaration with the specified modifier removed.</summary>
    /// <param name="method">The property declaration to modify.</param>
    /// <param name="modifierKind">The <seealso cref="SyntaxKind"/> of the modifier to remove.</param>
    public static BasePropertyDeclarationSyntax RemoveModifier(
        this BasePropertyDeclarationSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    private static T WithModifiersOrSame<T>(
        bool adjustModifiers,
        T source,
        Func<SyntaxTokenList, T> withModifiers,
        SyntaxWithTriviaBank<SyntaxTokenList> tokenList)
        where T : SyntaxNode
    {
        if (!adjustModifiers)
        {
            return source;
        }

        var node = withModifiers(tokenList.Syntax);
        return tokenList.TriviaBank.AppendLeadingOnNode(node);
    }

    private static bool TryRemoveFromList(
        SyntaxTokenList modifiers,
        SyntaxKind modifierKind,
        out SyntaxWithTriviaBank<SyntaxTokenList> withRemoved)
    {
        var modifier = modifiers.FirstOrDefault(s => s.IsKind(modifierKind));
        if (modifier == default)
        {
            withRemoved = modifiers;
            return false;
        }

        var removedModifierList = modifiers.Remove(modifier);
        withRemoved = StealTriviaFromRemoved(modifier, modifiers, removedModifierList);
        return true;
    }

    private static SyntaxWithTriviaBank<SyntaxTokenList> StealTriviaFromRemoved(
        SyntaxToken token,
        SyntaxTokenList previousList,
        SyntaxTokenList nextList)
    {
        var hasRemoved = previousList.Count != nextList.Count;
        if (!hasRemoved)
        {
            return previousList;
        }

        SyntaxTriviaList stolenLeading = default;
        SyntaxTriviaList stolenTrailing = default;
        if (token.IsFirstIn(previousList))
        {
            stolenLeading = token.LeadingTrivia;
        }
        if (token.IsLastIn(previousList))
        {
            stolenTrailing = token.TrailingTrivia;
        }

        if (nextList.Count is 0)
        {
            var bank = new TriviaBank(stolenLeading, stolenTrailing);
            return new SyntaxWithTriviaBank<SyntaxTokenList>(nextList, bank);
        }

        var firstNext = nextList.First();
        if (stolenLeading.Count > 0)
        {
            var firstWithLeading = firstNext.WithLeadingTrivia(stolenLeading);
            nextList = nextList.Replace(firstNext, firstWithLeading);
        }

        var lastNext = nextList.Last();
        if (stolenTrailing.Count > 0)
        {
            var lastWithLeading = lastNext.WithTrailingTrivia(stolenTrailing);
            nextList = nextList.Replace(lastNext, lastWithLeading);
        }

        return nextList;
    }
}
