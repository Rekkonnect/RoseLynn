using Microsoft.CodeAnalysis;

namespace RoseLynn;

public static class QualifiedNameSymbolExtensions
{
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

    public static bool HasQualifiedName(
        this INamespaceSymbol? @namespace,
        string? qualified)
    {
        return @namespace.HasQualifiedName(qualified.AsSpan());
    }

    public static bool HasQualifiedName(
        this ITypeSymbol? type,
        string? qualified)
    {
        return type.HasQualifiedName(qualified.AsSpan());
    }
}
