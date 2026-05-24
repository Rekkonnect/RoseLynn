using Garyon.Functions;
using Microsoft.CodeAnalysis;

namespace RoseLynn;

public static class LocationHelpers
{
    public static Location FromBounds(Location left, Location right)
    {
        Asserts.NotNull(left.SourceTree);
        Asserts.NotNull(right.SourceTree);
        Asserts.True(left.SourceTree == right.SourceTree);
        Asserts.True(left.SourceSpan.Start <= right.SourceSpan.Start);

        var span = TextSpanHelpers.FromBounds(left.SourceSpan, right.SourceSpan);
        return Location.Create(left.SourceTree, span);
    }

    public static Location FromNodeBounds(SyntaxNodeOrToken left, SyntaxNodeOrToken right)
    {
        var leftLocation = left.GetLocation()!;
        var rightLocation = right.GetLocation()!;
        return FromBounds(leftLocation, rightLocation);
    }

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

    public static Location? ConditionallyGetTokenListLocation(
        SyntaxTokenList list,
        bool include)
    {
        return include
            ? GetTokenListLocation(list)
            : null;
    }

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
