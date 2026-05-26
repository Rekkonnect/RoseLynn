using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace RoseLynn.CSharp;

/// <summary>Provides C#-specific extensions for Roslyn operation types.</summary>
public static class OperationExtensions
{
    /// <summary>Gets the name syntax node representing the referenced property.</summary>
    public static SyntaxNode GetPropertySyntaxNode(this IPropertyReferenceOperation operation)
    {
        switch (operation.Syntax)
        {
            case MemberBindingExpressionSyntax memberBindingExpression:
                return memberBindingExpression.Name;

            case MemberAccessExpressionSyntax access:
                return access.Name;

            case IdentifierNameSyntax identifier:
                return identifier;

            case ElementAccessExpressionSyntax access:
                return access;

            case var syntax:
                return syntax;
        }
    }
}
