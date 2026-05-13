using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using System.Threading;

namespace RoseLynn;

public static class GeneralRoslynExtensions
{
    public static Compilation? GetContainingCompilation(this ISymbol symbol)
    {
        return symbol.ContainingNamespace?.ContainingCompilation;
    }

    public static object? SafeExplicitDefaultValue(this IParameterSymbol parameterSymbol)
    {
        return parameterSymbol.HasExplicitDefaultValue
            ? parameterSymbol.ExplicitDefaultValue
            : null;
    }

    public static bool HasAttributeWithClassName(this ISymbol symbol, string className)
    {
        return symbol.HasAnyAttribute(a => a.IsAttributeClassName(className));
    }

    public static bool HasAnyAttribute(
        this ISymbol symbol,
        Func<AttributeData, bool> predicate)
    {
        return symbol.GetAttributes().Any(predicate);
    }

    public static bool IsAttributeClassName(
        this AttributeData attribute,
        string className)
    {
        return attribute.AttributeClassName() == className;
    }

    public static string? AttributeClassName(
        this AttributeData attribute)
    {
        return attribute.AttributeClass?.ToDisplayString();
    }

    public static string? StringOrNull(this TypedConstant data)
    {
        if (data.Kind is not TypedConstantKind.Primitive)
        {
            return null;
        }

        return data.Value as string;
    }

    /// <summary>
    /// Finds a node within the tree that contains the given substring.
    /// </summary>
    /// <returns>
    /// The deepest node that contains the given substring, or
    /// <see langword="null"/> if the substring was not found in the tree.
    /// </returns>
    public static SyntaxNode? FindNodeContaining(
        this SyntaxTree tree,
        string substring,
        bool findInsideTrivia = false,
        CancellationToken cancellationToken = default)
    {
        var text = tree.GetText(cancellationToken).ToString();
        var span = text.SpanFor(substring);
        if (span == default)
        {
            return null;
        }

        return tree.GetRoot(cancellationToken)
            .FindNode(
                span,
                findInsideTrivia: findInsideTrivia,
                getInnermostNodeForTie: true);
    }

    /// <summary>
    /// Finds a substring within a string, and returns a <see cref="TextSpan"/>
    /// denoting the span of the first instance of the substring.
    /// </summary>
    /// <returns>
    /// A <see cref="TextSpan"/> denoting the span that matches the first
    /// instance of the substring, or <see langword="default"/> if the
    /// substring was not found.
    /// </returns>
    public static TextSpan SpanFor(this string outer, string substring)
    {
        var start = outer.IndexOf(substring);
        if (start < 0)
        {
            return default;
        }

        return new(start, substring.Length);
    }

    public static SyntaxTree WithChangedText(this SyntaxTree tree, string text)
    {
        return tree.WithChangedText(SourceText.From(text, tree.Encoding));
    }

    /// <summary>
    /// Constructs a new syntax token, preserving the kind and trivia of the
    /// given token, and adjusting the text to the target value. The value
    /// text is also set to that new text.
    /// </summary>
    public static SyntaxToken WithText(this SyntaxToken token, string text)
    {
        if (token.Kind() is SyntaxKind.IdentifierToken)
        {
            return SyntaxFactory.Identifier(
                leading: token.LeadingTrivia,
                contextualKind: SyntaxKind.None,
                text: text,
                valueText: text,
                trailing: token.TrailingTrivia);
        }

        return SyntaxFactory.Token(
            leading: token.LeadingTrivia,
            kind: token.Kind(),
            text: text,
            valueText: text,
            trailing: token.TrailingTrivia);
    }

    /// <summary>
    /// Replaces all descendant tokens, including trivia-descended tokens, that
    /// match the given old text (after trimming), with tokens containing the
    /// new text.
    /// </summary>
    /// <param name="node">
    /// The node whose descendant tokens, including trivia-descended tokens,
    /// will be replaced if they match the old text.
    /// </param>
    /// <param name="oldText">
    /// Determines the tokens to be replaced; all descendant tokens that have
    /// text that equals this old text property when trimmed, will be replaced
    /// with tokens that have their text changed with the new text.
    /// </param>
    /// <param name="newText">
    /// The new text to include in the replaced tokens.
    /// </param>
    /// <returns>
    /// The node containing the replaced tokens.
    /// </returns>
    public static SyntaxNode WithReplacedTextTokens(
        this SyntaxNode node,
        string oldText,
        string newText)
    {
        oldText = oldText.Trim();
        var replacedTokens = node
            .DescendantTokens(descendIntoTrivia: true)
            .Where(s => s.Text.AsSpan().Trim().SequenceEqual(oldText));
        return node.ReplaceTokens(replacedTokens, ReplaceToken);

        SyntaxToken ReplaceToken(SyntaxToken oldToken, SyntaxToken newerToken)
        {
            return newerToken.WithText(newText);
        }
    }
}