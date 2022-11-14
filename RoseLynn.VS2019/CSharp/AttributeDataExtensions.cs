#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn.CSharp;

/// <summary>Provides useful extensions for the <seealso cref="AttributeData"/> type used for C# syntax trees.</summary>
public static class AttributeDataExtensions
{
    /// <summary>Gets the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeData"/>.</summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attribute"/>.</returns>
    public static AttributeListTarget GetTarget(this AttributeData attribute)
    {
        return attribute.GetAttributeApplicationSyntax().GetTarget();
    }
    /// <summary>Determines whether the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeData"/> is a provided one.</summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <param name="target">The desired <seealso cref="AttributeListTarget"/> to check.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attribute"/>.</returns>
    public static bool HasTarget(this AttributeData attribute, AttributeListTarget target)
    {
        return attribute.GetAttributeApplicationSyntax().HasTarget(target);
    }

    /// <summary>Gets the appropriate <seealso cref="AttributeSyntax"/> node representing the attribute whose information is contained in the provided <seealso cref="AttributeData"/> instance.</summary>
    /// <param name="attribute">The <seealso cref="AttributeData"/> whose representing <seealso cref="AttributeSyntax"/> to get.</param>
    /// <returns>The <seealso cref="AttributeSyntax"/> representing the attribute whose information is in <paramref name="attribute"/>.</returns>
    public static AttributeSyntax? GetAttributeApplicationSyntax(this AttributeData? attribute)
    {
        return attribute?.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
    }
}
