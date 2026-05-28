using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Identifies a <seealso cref="SyntaxAnnotation"/> by its kind and data strings.</summary>
/// <param name="Kind">The annotation kind.</param>
/// <param name="Data">The annotation data.</param>
public readonly record struct SyntaxAnnotationIdentifier(string Kind, string Data)
{
    /// <summary>Returns <see langword="true"/> if <paramref name="annotation"/> has the same kind and data as this identifier.</summary>
    /// <param name="annotation">The annotation to test.</param>
    /// <returns><see langword="true"/> when kind and data match; otherwise <see langword="false"/>.</returns>
    public bool Matches(SyntaxAnnotation annotation)
    {
        return annotation.Kind == Kind
            && annotation.Data == Data
            ;
    }
}
