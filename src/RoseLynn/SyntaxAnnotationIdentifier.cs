using Microsoft.CodeAnalysis;

namespace RoseLynn;

public readonly record struct SyntaxAnnotationIdentifier(string Kind, string Data)
{
    public bool Matches(SyntaxAnnotation annotation)
    {
        return annotation.Kind == Kind
            && annotation.Data == Data
            ;
    }
}
