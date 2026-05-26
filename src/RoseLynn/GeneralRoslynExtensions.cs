using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoseLynn;

/// <summary>Provides general-purpose Roslyn helper extensions used across the library.</summary>
public static class GeneralRoslynExtensions
{
    /// <summary>Gets the compilation that contains a symbol through its containing namespace.</summary>
    /// <param name="symbol">The symbol whose containing compilation will be retrieved.</param>
    /// <returns>The containing compilation, or <see langword="null"/> if it cannot be resolved.</returns>
    public static Compilation? GetContainingCompilation(this ISymbol symbol)
    {
        return symbol.ContainingNamespace?.ContainingCompilation;
    }

    /// <summary>Gets the explicit default value of a parameter when one is declared.</summary>
    /// <param name="parameterSymbol">The parameter to inspect.</param>
    /// <returns>The explicit default value, or <see langword="null"/> when the parameter has none.</returns>
    public static object? SafeExplicitDefaultValue(this IParameterSymbol parameterSymbol)
    {
        return parameterSymbol.HasExplicitDefaultValue
            ? parameterSymbol.ExplicitDefaultValue
            : null;
    }

    /// <summary>Determines whether a symbol has an attribute with the specified fully qualified class name.</summary>
    /// <param name="symbol">The symbol whose attributes will be inspected.</param>
    /// <param name="className">The fully qualified attribute class name to match.</param>
    /// <returns><see langword="true"/> if a matching attribute is found; otherwise, <see langword="false"/>.</returns>
    public static bool HasAttributeWithClassName(this ISymbol symbol, string className)
    {
        return symbol.HasAnyAttribute(a => a.IsAttributeClassName(className));
    }

    /// <summary>Determines whether any attribute on a symbol matches a predicate.</summary>
    /// <param name="symbol">The symbol whose attributes will be evaluated.</param>
    /// <param name="predicate">The predicate used to test each attribute.</param>
    /// <returns><see langword="true"/> if any attribute matches the predicate; otherwise, <see langword="false"/>.</returns>
    public static bool HasAnyAttribute(
        this ISymbol symbol,
        Func<AttributeData, bool> predicate)
    {
        return symbol.GetAttributes().Any(predicate);
    }

    /// <summary>Determines whether an attribute resolves to the specified fully qualified class name.</summary>
    /// <param name="attribute">The attribute to inspect.</param>
    /// <param name="className">The fully qualified class name to compare against.</param>
    /// <returns><see langword="true"/> if the attribute class name matches; otherwise, <see langword="false"/>.</returns>
    public static bool IsAttributeClassName(
        this AttributeData attribute,
        string className)
    {
        return attribute.AttributeClassName() == className;
    }

    /// <summary>Gets the fully qualified display name of an attribute's class.</summary>
    /// <param name="attribute">The attribute whose class name will be returned.</param>
    /// <returns>The fully qualified class name, or <see langword="null"/> if the class cannot be resolved.</returns>
    public static string? AttributeClassName(
        this AttributeData attribute)
    {
        return attribute.AttributeClass?.ToDisplayString();
    }

    /// <summary>Returns the string value of a primitive typed constant when available.</summary>
    /// <param name="data">The typed constant to inspect.</param>
    /// <returns>The string value if the constant is a primitive string; otherwise, <see langword="null"/>.</returns>
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

    /// <summary>Creates a syntax tree with its text replaced while preserving the original encoding.</summary>
    /// <param name="tree">The syntax tree to update.</param>
    /// <param name="text">The new text content for the syntax tree.</param>
    /// <returns>A syntax tree based on <paramref name="tree"/> with the provided text.</returns>
    public static SyntaxTree WithChangedText(this SyntaxTree tree, string text)
    {
        return tree.WithChangedText(SourceText.From(text, tree.Encoding));
    }
}