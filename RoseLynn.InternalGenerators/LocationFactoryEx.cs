using Microsoft.CodeAnalysis;

namespace RoseLynn.InternalGenerators;

internal static class LocationFactoryEx
{
    public static Location Create(SyntaxReference reference)
    {
        return Location.Create(reference.SyntaxTree, reference.Span);
    }
}
