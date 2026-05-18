using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides useful extensions for the <seealso cref="TypeParameterSyntax"/> type.</summary>
public static class TypeParameterSyntaxExtensions
{
    public static TypeParameterConstraintClauseSyntax? GetTypeParameterConstraintClause(this TypeParameterSyntax typeParameter)
    {
        var declaringMemberSyntax = typeParameter.GetDeclaringMemberParent();
        var constraintClauses = declaringMemberSyntax.GetConstraintClauses();
        return constraintClauses.FirstOrDefault(c => c.Name.Identifier.Text == typeParameter.Identifier.Text);
    }

    public static MemberDeclarationSyntax GetDeclaringMemberParent(this TypeParameterSyntax typeParameter)
    {
        return typeParameter.GetNearestParentOfType<MemberDeclarationSyntax>()!;
    }

}
