using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides useful extensions for the <seealso cref="TypeParameterConstraintClauseSyntax"/> type.</summary>
public static class TypeParameterConstraintClauseSyntaxExtensions
{
    /// <summary>Adds the specified type constraint to the clause and updates it accordingly to ensure its validity.</summary>
    /// <param name="constraintClause">The constraint clause to add a new type constraint into.</param>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> that refers to the <seealso cref="SyntaxTree"/> that the given constraint clause is contained in.</param>
    /// <param name="typeConstraint">The type constraint to add to the clause.</param>
    /// <returns>The modified constraint clause.</returns>
    public static TypeParameterConstraintClauseSyntax AddUpdateTypeConstraint
    (
        this TypeParameterConstraintClauseSyntax constraintClause,
        SemanticModel semanticModel,
        TypeConstraintSyntax typeConstraint
    )
    {
        var segmentation = new TypeParameterConstraintClauseSegmentation(constraintClause, semanticModel);
        segmentation.AddTypeConstraint(typeConstraint);
        return segmentation.WithTheseConstraints(constraintClause);
    }
    /// <summary>Adds the specified type constraint to the clause and updates it accordingly to ensure its validity.</summary>
    /// <param name="constraintClause">The constraint clause to add a new type constraint into.</param>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> that refers to the <seealso cref="SyntaxTree"/> that the given constraint clause is contained in.</param>
    /// <param name="typeConstraint">The type constraint to add to the clause.</param>
    /// <param name="typeSymbol">The <seealso cref="ITypeSymbol"/> of the type that the type constraint refers to.</param>
    /// <returns>The modified constraint clause.</returns>
    public static TypeParameterConstraintClauseSyntax AddUpdateTypeConstraint
    (
        this TypeParameterConstraintClauseSyntax constraintClause,
        SemanticModel semanticModel,
        TypeConstraintSyntax typeConstraint,
        ITypeSymbol typeSymbol
    )
    {
        var segmentation = new TypeParameterConstraintClauseSegmentation(constraintClause, semanticModel);
        segmentation.AddTypeConstraint(typeConstraint, typeSymbol);
        return segmentation.WithTheseConstraints(constraintClause);
    }

    /// <summary>
    /// Adds the specified constraints to the clause and updates it accordingly to ensure its validity.
    /// This assumes that both the original constraint clause is valid and the constraints contain no invalid for the specified constraint clause combinations.
    /// <br/>
    /// The following modifications are performed:<br/>
    /// - If a keyword constraint (<see langword="class"/>, <see langword="struct"/>, <see langword="notnull"/>, <see langword="unmanaged"/>, <see langword="default"/>) is specified to be added, it will be added at the start, replacing the potentially existing keyword or class constraint.<br/>
    /// - Interface constraints are appended to the end of the interface constraint segment.<br/>
    /// - The <see langword="new"/>() constraint is added to the end, replacing the potentially existing one.<br/>
    /// </summary>
    /// <param name="constraintClause">The original constraint clause.</param>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> to use when analyzing the constraint.</param>
    /// <param name="constraints">The new constraints to add to the constraint clause.</param>
    /// <returns>The resulting constraint clause with the specified added constraints.</returns>
    public static TypeParameterConstraintClauseSyntax AddUpdateConstraints<T>
    (
        this TypeParameterConstraintClauseSyntax constraintClause,
        SemanticModel semanticModel,
        params TypeParameterConstraintSyntax[] constraints
    )
    {
        var segmentation = new TypeParameterConstraintClauseSegmentation(constraintClause, semanticModel);
        segmentation.AddConstraints(constraints);
        return segmentation.WithTheseConstraints(constraintClause);
    }
}
