using Microsoft.CodeAnalysis.CSharp;
using RoseLynn.Utilities;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Represents a target in an attribute list.</summary>
public enum AttributeListTarget
{
    /// <summary>Represents the default target, that is, the attribute list has no target identifier.</summary>
    Default,

    /// <summary>Represents the <see langword="assembly"/> target.</summary>
    Assembly,
    /// <summary>Represents the <see langword="module"/> target.</summary>
    Module,
    /// <summary>Represents the <see langword="field"/> target.</summary>
    Field,
    /// <summary>Represents the <see langword="event"/> target.</summary>
    Event,
    /// <summary>Represents the <see langword="method"/> target.</summary>
    Method,
    /// <summary>Represents the <see langword="param"/> target.</summary>
    Param,
    /// <summary>Represents the <see langword="property"/> target.</summary>
    Property,
    /// <summary>Represents the <see langword="return"/> target.</summary>
    Return,
    /// <summary>Represents the <see langword="type"/> target.</summary>
    Type,
}

internal static class AttributeListTargetCaches
{
    public static readonly InterlinkedDictionary<AttributeListTarget, SyntaxKind> SyntaxKindInterlinks = new();

    static AttributeListTargetCaches()
    {
        var values = EnumHelpers.GetValues<AttributeListTarget>();
        foreach (var targetValue in values)
        {
            if (targetValue is AttributeListTarget.Default)
                continue;

            var syntaxKindValue = EnumHelpers.Parse<SyntaxKind>($"{targetValue}Keyword");
            SyntaxKindInterlinks.Add(targetValue, syntaxKindValue);
        }
    }
}

/// <summary>Provides extension methods for the <seealso cref="AttributeListTarget"/> enum.</summary>
public static class AttributeListTargetExtensions
{
    /// <summary>Gets the <seealso cref="SyntaxKind"/> representing the provided <seealso cref="AttributeListTarget"/>.</summary>
    /// <param name="attributeListTarget">The <see cref="AttributeListTarget"/> to be represented by the respective <seealso cref="SyntaxKind"/>.</param>
    /// <returns>The appropriate <seealso cref="SyntaxKind"/> if <paramref name="attributeListTarget"/> is not <seealso cref="AttributeListTarget.Default"/>, otherwise <see langword="default"/>.</returns>
    public static SyntaxKind GetSyntaxKind(this AttributeListTarget attributeListTarget)
    {
        return AttributeListTargetCaches.SyntaxKindInterlinks.ValueOrDefault(attributeListTarget);
    }
    /// <summary>Gets the <seealso cref="AttributeListTarget"/> represented by the provided <seealso cref="SyntaxKind"/>.</summary>
    /// <param name="syntaxKind">The <see cref="SyntaxKind"/> representing an <seealso cref="AttributeListTarget"/>.</param>
    /// <returns>The appropriate <seealso cref="AttributeListTarget"/> if <paramref name="syntaxKind"/> is a valid attribute target keyword, otherwise <seealso cref="AttributeListTarget.Default"/>.</returns>
    public static AttributeListTarget GetAttributeListTarget(this SyntaxKind syntaxKind)
    {
        return AttributeListTargetCaches.SyntaxKindInterlinks.ValueOrDefault(syntaxKind);
    }
}
