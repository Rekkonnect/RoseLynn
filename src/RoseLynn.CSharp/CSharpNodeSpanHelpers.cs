using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;

namespace RoseLynn.CSharp;

/// <summary>Provides helpers for computing narrowed <seealso cref="Location"/> spans from C# syntax nodes, useful for producing tighter diagnostic locations.</summary>
public static class CSharpNodeSpanHelpers
{
    /// <summary>Gets a <seealso cref="Location"/> that covers only the invocation name and argument list, stripping any leading member-access expression.</summary>
    /// <param name="syntax">The syntax node to inspect. If it is not an <seealso cref="InvocationExpressionSyntax"/>, its full location is returned.</param>
    public static Location GetInvocationOnlyLocation(this SyntaxNode syntax)
    {
        return syntax switch
        {
            InvocationExpressionSyntax invocation
                => GetInvocationNameAndArgumentsListLocation(invocation),

            _ => syntax.GetLocation()!,
        };
    }

    /// <summary>Gets a <seealso cref="Location"/> that covers only the signature portion of a declaration, optionally including modifiers.</summary>
    /// <param name="syntax">The declaration syntax node. Handles methods, properties, indexers, and events; returns the full node location for unrecognised kinds.</param>
    /// <param name="includeModifiers">When <see langword="true"/>, the span begins at the first modifier token.</param>
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
