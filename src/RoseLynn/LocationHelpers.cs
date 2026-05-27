using Garyon.Functions;
using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Provides factory helpers for constructing <seealso cref="Location"/> instances from ranges and nodes.</summary>
public static class LocationHelpers
{
    /// <summary>Creates a <seealso cref="Location"/> spanning from <paramref name="left"/> to <paramref name="right"/> within the same source tree.</summary>
    /// <param name="left">The start location; its source tree must match <paramref name="right"/>.</param>
    /// <param name="right">The end location.</param>
    /// <returns>A location spanning both inputs.</returns>
    public static Location FromBounds(Location left, Location right)
    {
        Asserts.NotNull(left.SourceTree);
        Asserts.NotNull(right.SourceTree);
        Asserts.True(left.SourceTree == right.SourceTree);
        Asserts.True(left.SourceSpan.Start <= right.SourceSpan.Start);

        var span = TextSpanHelpers.FromBounds(left.SourceSpan, right.SourceSpan);
        return Location.Create(left.SourceTree, span);
    }

    /// <summary>Creates a <seealso cref="Location"/> spanning from the location of <paramref name="left"/> to the location of <paramref name="right"/>.</summary>
    /// <param name="left">The start node or token.</param>
    /// <param name="right">The end node or token.</param>
    /// <returns>A location spanning both nodes.</returns>
    public static Location FromNodeBounds(SyntaxNodeOrToken left, SyntaxNodeOrToken right)
    {
        var leftLocation = left.GetLocation()!;
        var rightLocation = right.GetLocation()!;
        return FromBounds(leftLocation, rightLocation);
    }

    /// <summary>Returns a location covering the declaration signature from optional modifiers to <paramref name="endNode"/>.</summary>
    /// <param name="modifiers">The modifier token list; used as the start if <paramref name="includeModifiers"/> is <see langword="true"/> and non-empty.</param>
    /// <param name="alternativeStart">The start node to use when modifiers are excluded or empty.</param>
    /// <param name="endNode">The node marking the end of the signature location.</param>
    /// <param name="includeModifiers">Whether to include the modifiers in the location range.</param>
    /// <returns>A location spanning the declaration signature.</returns>
    public static Location GetDeclarationSignatureOnlyLocation(
        SyntaxTokenList modifiers,
        SyntaxNodeOrToken alternativeStart,
        SyntaxNodeOrToken endNode,
        bool includeModifiers)
    {
        var modifiersLocation = ConditionallyGetTokenListLocation(
            modifiers, includeModifiers);

        var startLocation = modifiersLocation ?? alternativeStart.GetLocation()!;
        var endLocation = endNode.GetLocation()!;
        return LocationHelpers.FromBounds(startLocation, endLocation);
    }

    /// <summary>Returns the location spanning <paramref name="list"/> if <paramref name="include"/> is <see langword="true"/>; otherwise <see langword="null"/>.</summary>
    /// <param name="list">The token list to span.</param>
    /// <param name="include">Whether to return the location.</param>
    /// <returns>The token list location, or <see langword="null"/>.</returns>
    public static Location? ConditionallyGetTokenListLocation(
        SyntaxTokenList list,
        bool include)
    {
        return include
            ? GetTokenListLocation(list)
            : null;
    }

    /// <summary>Returns a location spanning the first to the last token in <paramref name="list"/>, or <see langword="null"/> if the list is empty.</summary>
    /// <param name="list">The token list to span.</param>
    /// <returns>The spanning location, or <see langword="null"/> for empty lists.</returns>
    public static Location? GetTokenListLocation(
        SyntaxTokenList list)
    {
        if (list.Count is 0)
        {
            return null;
        }

        var first = list.First();
        var last = list.Last();
        return LocationHelpers.FromNodeBounds(first, last);
    }
}
