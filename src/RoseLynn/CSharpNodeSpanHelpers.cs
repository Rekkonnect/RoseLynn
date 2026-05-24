using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn;

public static class CSharpNodeSpanHelpers
{
    public static Location GetInvocationOnlyLocation(this SyntaxNode syntax)
    {
        return syntax switch
        {
            InvocationExpressionSyntax invocation
                => GetInvocationNameAndArgumentsListLocation(invocation),

            _ => syntax.GetLocation()!,
        };
    }

    public static Location GetDeclarationSignatureOnlyLocation(
        this SyntaxNode syntax,
        bool includeModifiers)
    {
        return syntax switch
        {
            MethodDeclarationSyntax method
                => GetDeclarationSignatureOnlyLocation(method, includeModifiers),

            PropertyDeclarationSyntax property
                => GetDeclarationSignatureOnlyLocation(property, includeModifiers),

            IndexerDeclarationSyntax indexer
                => GetDeclarationSignatureOnlyLocation(indexer, includeModifiers),

            EventDeclarationSyntax @event
                => GetDeclarationSignatureOnlyLocation(@event, includeModifiers),

            _ => syntax.GetLocation()!,
        };
    }

    private static Location GetInvocationNameAndArgumentsListLocation(
        InvocationExpressionSyntax invocation)
    {
        var name = GetInvokedName(invocation);
        var argumentList = invocation.ArgumentList;
        return LocationHelpers.FromNodeBounds(name, argumentList);
    }

    private static SyntaxNodeOrToken GetInvokedName(
        InvocationExpressionSyntax invocation)
    {
        var expression = invocation.Expression;
        return expression switch
        {
            MemberAccessExpressionSyntax member => member.Name,
            AliasQualifiedNameSyntax alias => alias.Name,
            MemberBindingExpressionSyntax binding => binding.Name,
            _ => expression,
        };
    }

    private static Location GetDeclarationSignatureOnlyLocation(
        MethodDeclarationSyntax method,
        bool includeModifiers)
    {
        return LocationHelpers.GetDeclarationSignatureOnlyLocation(
            method.Modifiers,
            method.ReturnType,
            method.ParameterList,
            includeModifiers);
    }

    private static Location GetDeclarationSignatureOnlyLocation(
        PropertyDeclarationSyntax property,
        bool includeModifiers)
    {
        return LocationHelpers.GetDeclarationSignatureOnlyLocation(
            property.Modifiers,
            property.Type,
            property.Identifier,
            includeModifiers);
    }

    private static Location GetDeclarationSignatureOnlyLocation(
        IndexerDeclarationSyntax indexer,
        bool includeModifiers)
    {
        return LocationHelpers.GetDeclarationSignatureOnlyLocation(
            indexer.Modifiers,
            indexer.Type,
            indexer.ParameterList,
            includeModifiers);
    }

    private static Location GetDeclarationSignatureOnlyLocation(
        EventDeclarationSyntax @event,
        bool includeModifiers)
    {
        return LocationHelpers.GetDeclarationSignatureOnlyLocation(
            @event.Modifiers,
            @event.Type,
            @event.Identifier,
            includeModifiers);
    }
}
