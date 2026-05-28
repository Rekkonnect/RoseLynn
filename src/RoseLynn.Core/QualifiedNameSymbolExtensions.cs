using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Provides extension methods for checking qualified names of Roslyn symbols.</summary>
public static class QualifiedNameSymbolExtensions
{
    /// <summary>Returns <see langword="true"/> if the namespace's fully qualified name equals <paramref name="qualified"/>.</summary>
    /// <param name="namespace">The namespace symbol to check.</param>
    /// <param name="qualified">The expected fully qualified name (e.g. <c>System.Collections.Generic</c>).</param>
    /// <returns><see langword="true"/> when the qualified names match; otherwise <see langword="false"/>.</returns>
    public static bool HasQualifiedName(
        this INamespaceSymbol? @namespace,
        ReadOnlySpan<char> qualified)
    {
        if (@namespace is null)
        {
            return qualified.Length is 0;
        }

        var current = @namespace;
        var remaining = qualified;
        while (remaining.Length > 0)
        {
            if (current is null)
            {
                return false;
            }

            _ = remaining.SplitOnceRight('.', out var next, out var currentName);

            if (!current.Name.AsSpan().SequenceEqual(currentName))
            {
                return false;
            }

            remaining = next;
            current = current.ContainingNamespace;
        }

        return current.IsGlobalNamespace;
    }

    /// <summary>Returns <see langword="true"/> if the type's fully qualified name equals <paramref name="qualified"/>.</summary>
    /// <param name="type">The type symbol to check.</param>
    /// <param name="qualified">The expected fully qualified name (e.g. <c>System.Collections.Generic.List</c>).</param>
    /// <returns><see langword="true"/> when the qualified names match; otherwise <see langword="false"/>.</returns>
    public static bool HasQualifiedName(
        this ITypeSymbol? type,
        ReadOnlySpan<char> qualified)
    {
        if (type is null)
        {
            return qualified.Length is 0;
        }

        var current = type;
        var remaining = qualified;
        while (remaining.Length > 0)
        {
            if (current is null)
            {
                return false;
            }

            _ = remaining.SplitOnceRight('.', out var next, out var currentName);

            if (!current.Name.AsSpan().SequenceEqual(currentName))
            {
                return false;
            }

            remaining = next;
            current = current.ContainingType;
        }

        return current is not null;
    }

    /// <summary>Returns <see langword="true"/> if the namespace's fully qualified name equals <paramref name="qualified"/>.</summary>
    /// <param name="namespace">The namespace symbol to check.</param>
    /// <param name="qualified">The expected fully qualified name (e.g. <c>System.Collections.Generic</c>).</param>
    /// <returns><see langword="true"/> when the qualified names match; otherwise <see langword="false"/>.</returns>
    public static bool HasQualifiedName(
        this INamespaceSymbol? @namespace,
        string? qualified)
    {
        return @namespace.HasQualifiedName(qualified.AsSpan());
    }

    /// <summary>Returns <see langword="true"/> if the type's fully qualified name equals <paramref name="qualified"/>.</summary>
    /// <param name="type">The type symbol to check.</param>
    /// <param name="qualified">The expected fully qualified name (e.g. <c>System.Collections.Generic.List</c>).</param>
    /// <returns><see langword="true"/> when the qualified names match; otherwise <see langword="false"/>.</returns>
    public static bool HasQualifiedName(
        this ITypeSymbol? type,
        string? qualified)
    {
        return type.HasQualifiedName(qualified.AsSpan());
    }
}
