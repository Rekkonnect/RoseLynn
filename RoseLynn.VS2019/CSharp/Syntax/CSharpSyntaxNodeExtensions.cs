using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides useful extensions for the <seealso cref="CSharpSyntaxNode"/> type.</summary>
public static class CSharpSyntaxNodeExtensions
{
    /// <summary>Gets the default <seealso cref="AttributeListTarget"/> for the given <seealso cref="CSharpSyntaxNode"/>.</summary>
    /// <param name="attributedMember">The member that is attributed.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> representing the default target for the given <seealso cref="CSharpSyntaxNode"/> kind.</returns>
    public static AttributeListTarget ResolveDefaultAttributeListTarget(this CSharpSyntaxNode attributedMember)
    {
        return attributedMember switch
        {
            FieldDeclarationSyntax => AttributeListTarget.Field,
            PropertyDeclarationSyntax => AttributeListTarget.Property,
            MethodDeclarationSyntax => AttributeListTarget.Method,
            ParameterSyntax => AttributeListTarget.Param,

            _ when attributedMember.IsNamedTypeDeclarationSyntax() => AttributeListTarget.Type,
            _ when attributedMember.IsEventDeclarationSyntax() => AttributeListTarget.Event,

            // avoid throwing on non-attributable members - assume intending to attribute the assembly
            _ => AttributeListTarget.Assembly,
        };
    }

    /// <summary>Determines whether the <seealso cref="CSharpSyntaxNode"/> represents a named type declaration syntax.</summary>
    /// <param name="syntaxNode">The syntax node to determine if it's a named type declaration syntax.</param>
    /// <returns><see langword="true"/> if <paramref name="syntaxNode"/> is <seealso cref="BaseTypeDeclarationSyntax"/> or <seealso cref="DelegateDeclarationSyntax"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsNamedTypeDeclarationSyntax(this CSharpSyntaxNode syntaxNode)
    {
        return syntaxNode is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax;
    }
    /// <summary>Determines whether the <seealso cref="CSharpSyntaxNode"/> represents an event declaration syntax.</summary>
    /// <param name="syntaxNode">The syntax node to determine if it's an event declaration syntax.</param>
    /// <returns><see langword="true"/> if <paramref name="syntaxNode"/> is <seealso cref="EventDeclarationSyntax"/> or <seealso cref="EventFieldDeclarationSyntax"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsEventDeclarationSyntax(this CSharpSyntaxNode syntaxNode)
    {
        return syntaxNode is EventDeclarationSyntax or EventFieldDeclarationSyntax;
    }
}
