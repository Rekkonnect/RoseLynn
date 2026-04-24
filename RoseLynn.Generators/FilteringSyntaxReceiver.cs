using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Provides an <seealso cref="ISyntaxReceiver"/> that filters visited syntax nodes.</summary>
public abstract class FilteringSyntaxReceiver : FilteringReceiverBase<SyntaxNode>, ISyntaxReceiver
{
    void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        OnVisitSyntaxNode(syntaxNode);
    }
}
