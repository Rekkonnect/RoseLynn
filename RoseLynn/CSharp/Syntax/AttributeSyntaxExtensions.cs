#nullable enable

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides useful extensions for the <seealso cref="AttributeSyntax"/> type.</summary>
public static class AttributeSyntaxExtensions
{
    /// <summary>Gets the identifier string of the <seealso cref="AttributeSyntax"/>.</summary>
    /// <param name="attribute">The <seealso cref="AttributeSyntax"/> whose identifier string to get.</param>
    /// <returns>The text value of the string identifier of the <seealso cref="AttributeSyntax"/>.</returns>
    public static string? GetAttributeIdentifierString(this AttributeSyntax attribute) => (attribute?.Name as IdentifierNameSyntax)?.Identifier.ValueText;

    /// <summary>Gets the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeSyntax"/>.</summary>
    /// <param name="attribute">The <seealso cref="AttributeSyntax"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attribute"/>.</returns>
    public static AttributeListTarget GetTarget(this AttributeSyntax? attribute)
    {
        return (attribute?.GetParentAttributeList()).GetTarget();
    }
    /// <summary>Determines whether the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeSyntax"/> is a provided one.</summary>
    /// <param name="attribute">The <seealso cref="AttributeSyntax"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <param name="target">The desired <seealso cref="AttributeListTarget"/> to check.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attribute"/>.</returns>
    public static bool HasTarget(this AttributeSyntax? attribute, AttributeListTarget target)
    {
        return (attribute?.GetParentAttributeList()).HasTarget(target);
    }
}
