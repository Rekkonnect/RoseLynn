#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides useful extensions for the <seealso cref="AttributeSyntax"/> type.</summary>
public static class AttributeSyntaxExtensions
{
    /// <summary>Gets the identifier string of the <seealso cref="AttributeSyntax"/>.</summary>
    /// <param name="attribute">The <seealso cref="AttributeSyntax"/> whose identifier string to get.</param>
    /// <returns>The text value of the string identifier of the <seealso cref="AttributeSyntax"/>.</returns>
    public static string? GetAttributeIdentifierString(this AttributeSyntax attribute) => (attribute?.Name as IdentifierNameSyntax)?.Identifier.ValueText;

    /// <summary>Gets the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeSyntax"/>.</summary>
    /// <param name="attribute">The <seealso cref="AttributeSyntax"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attribute"/>.</returns>
    public static AttributeListTarget GetTarget(this AttributeSyntax? attribute)
    {
        return (attribute?.GetParentAttributeList()).GetTarget();
    }
    /// <summary>Determines whether the <seealso cref="AttributeListTarget"/> type of the <seealso cref="AttributeSyntax"/> is a provided one.</summary>
    /// <param name="attribute">The <seealso cref="AttributeSyntax"/> whose <seealso cref="AttributeListTarget"/> to identify.</param>
    /// <param name="target">The desired <seealso cref="AttributeListTarget"/> to check.</param>
    /// <returns>The <seealso cref="AttributeListTarget"/> of <paramref name="attribute"/>.</returns>
    public static bool HasTarget(this AttributeSyntax? attribute, AttributeListTarget target)
    {
        return (attribute?.GetParentAttributeList()).HasTarget(target);
    }

    /// <inheritdoc cref="GetAttributeData(AttributeSyntax, SemanticModel, out ISymbol?)"/>
    public static AttributeData? GetAttributeData(this AttributeSyntax attributeSyntax, SemanticModel semanticModel)
    {
        return attributeSyntax.GetAttributeData(semanticModel, out _);
    }
    /// <summary>Gets the appropriate <seealso cref="AttributeData"/> the given <seealso cref="AttributeSyntax"/> represents.</summary>
    /// <param name="attributeSyntax">The <seealso cref="AttributeSyntax"/> which declares the attribute whose <seealso cref="AttributeData"/> to get.</param>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> which contains the <seealso cref="AttributeData"/> for the <see cref="AttributeSyntax"/>.</param>
    /// <param name="attributedSymbol">The <seealso cref="ISymbol"/> on which the attribute is applied.</param>
    /// <returns>
    /// The <seealso cref="AttributeData"/> representing the attribute declared in the <seealso cref="AttributeSyntax"/>.
    /// If the <seealso cref="SemanticModel"/> does not reflect the compilation on which the given <seealso cref="AttributeSyntax"/> is contained, <see langword="null"/> is returned.
    /// </returns>
    public static AttributeData? GetAttributeData(this AttributeSyntax attributeSyntax, SemanticModel semanticModel, out ISymbol? attributedSymbol)
    {
        attributedSymbol = attributeSyntax.GetAttributedSymbol(semanticModel);
        return attributedSymbol?.GetAttributes().FirstOrDefault(MatchesAttributeData);

        bool MatchesAttributeData(AttributeData attribute)
        {
            return attribute.ApplicationSyntaxReference!.GetSyntax() == attributeSyntax;
        }
    }

    /// <summary>Gets the symbol that the given <seealso cref="AttributeSyntax"/> is applied to, given the <seealso cref="SemanticModel"/>.</summary>
    /// <param name="attributeSyntax">The <seealso cref="AttributeSyntax"/> whose applied symbol will be returned.</param>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> which refers to the </param>
    /// <returns>
    /// The <seealso cref="ISymbol"/> on which the <seealso cref="AttributeSyntax"/> is applied,
    /// or <see langword="null"/> if the <seealso cref="SyntaxTree"/>s of the <seealso cref="AttributeSyntax"/> and the <seealso cref="SemanticModel"/> do not match.
    /// </returns>
    public static ISymbol? GetAttributedSymbol(this AttributeSyntax attributeSyntax, SemanticModel semanticModel)
    {
        if (attributeSyntax.SyntaxTree != semanticModel.SyntaxTree)
            return null;
        
        return attributeSyntax.GetTarget() switch
        {
            AttributeListTarget.Assembly => semanticModel.Compilation.Assembly,
            AttributeListTarget.Module => semanticModel.Compilation.SourceModule,
            _ => semanticModel.GetDeclaredOrAnonymousSymbol(attributeSyntax.GetAttributeDeclarationParent()),
        };
    }

    /// <summary>Gets the parent <seealso cref="SyntaxNode"/> on which an <seealso cref="AttributeSyntax"/> is applied.</summary>
    /// <param name="attributeSyntax">The <seealso cref="AttributeSyntax"/> denoting an attribute applied to a <seealso cref="SyntaxNode"/>.</param>
    /// <returns>
    /// The <seealso cref="SyntaxNode"/> of the parent of the applied <seealso cref="AttributeSyntax"/>.
    /// It is the parent of the <seealso cref="AttributeListSyntax"/> that contains the given <seealso cref="AttributeSyntax"/>.<br/>
    /// The containing <seealso cref="CompilationUnitSyntax"/> will be returned for the <see langword="assembly"/> and <see langword="module"/> attribute targets.
    /// </returns>
    public static SyntaxNode GetAttributeDeclarationParent(this AttributeSyntax attributeSyntax)
    {
        return attributeSyntax.GetParentAttributeList().Parent!;
    }
}
