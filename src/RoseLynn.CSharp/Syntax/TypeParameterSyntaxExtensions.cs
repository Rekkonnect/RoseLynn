using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides useful extensions for the <seealso cref="TypeParameterSyntax"/> type.</summary>
public static class TypeParameterSyntaxExtensions
{
    /// <summary>Gets the constraint clause associated with the given type parameter, if any.</summary>
    /// <param name="typeParameter">The type parameter whose constraint clause to find.</param>
    /// <returns>The matching <seealso cref="TypeParameterConstraintClauseSyntax"/>, or <see langword="null"/> if there is no constraint clause for this type parameter.</returns>
    public static TypeParameterConstraintClauseSyntax? GetTypeParameterConstraintClause(this TypeParameterSyntax typeParameter)
    {
        var declaringMemberSyntax = typeParameter.GetDeclaringMemberParent();
        var constraintClauses = declaringMemberSyntax.GetConstraintClauses();
        return constraintClauses.FirstOrDefault(c => c.Name.Identifier.Text == typeParameter.Identifier.Text);
    }

    /// <summary>Gets the nearest ancestor <seealso cref="MemberDeclarationSyntax"/> that declares the given type parameter.</summary>
    /// <param name="typeParameter">The type parameter whose declaring member to find.</param>
    /// <returns>The nearest ancestor <seealso cref="MemberDeclarationSyntax"/> in the syntax tree.</returns>
    public static MemberDeclarationSyntax GetDeclaringMemberParent(this TypeParameterSyntax typeParameter)
    {
        return typeParameter.GetNearestParentOfType<MemberDeclarationSyntax>()!;
    }

}
