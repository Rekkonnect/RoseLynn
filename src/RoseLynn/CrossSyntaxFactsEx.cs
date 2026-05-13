#nullable enable

using Microsoft.CodeAnalysis;

namespace RoseLynn;

// TODO: Place in the .CSharp version of RoseLynn
public static class CrossSyntaxFactsEx
{
    public static string? GetArgumentName<T>(T argumentSyntax)
        where T : SyntaxNode
    {
        return argumentSyntax switch
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax csArgument
                => csArgument.NameColon?.Name?.Identifier.Text,

            Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax csArgument
                => csArgument.NameColon?.Name?.Identifier.Text,

            _ => null,
        };
    }

    public static string? GetNamedArgumentMemberName<T>(T argumentSyntax)
        where T : SyntaxNode
    {
        return argumentSyntax switch
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax csArgument
                => csArgument.NameEquals?.Name?.Identifier.Text,

            _ => null,
        };
    }
}
