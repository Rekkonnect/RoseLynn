using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn;

public static class RemoveModifiersExtensions
{
    public static MethodDeclarationSyntax RemoveModifier(
        this MethodDeclarationSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    public static LocalFunctionStatementSyntax RemoveModifier(
        this LocalFunctionStatementSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    public static SimpleLambdaExpressionSyntax RemoveModifier(
        this SimpleLambdaExpressionSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    public static ParenthesizedLambdaExpressionSyntax RemoveModifier(
        this ParenthesizedLambdaExpressionSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

    public static AnonymousMethodExpressionSyntax RemoveModifier(
        this AnonymousMethodExpressionSyntax method, SyntaxKind modifierKind)
    {
        var removed = TryRemoveFromList(method.Modifiers, modifierKind, out var modifiers);
        return WithModifiersOrSame(removed, method, method.WithModifiers, modifiers);
    }

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
