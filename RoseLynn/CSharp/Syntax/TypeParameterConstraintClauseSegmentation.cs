using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Represents a mechanism that segments a type parameter constraint clause according to the ordering rules of the language.</summary>
public class TypeParameterConstraintClauseSegmentation
{
    private readonly SemanticModel semanticModel;

    private TypeParameterConstraintSyntax keywordOrClassConstraint;
    private TypeParameterConstraintSyntax delegateOrEnumConstraint;
    private List<TypeConstraintSyntax> otherConstraints = new();

    /// <summary>Gets or sets the keyword or class type constraint. That slot enables using either a keyword constraint (<see langword="class"/>, <see langword="struct"/>, <see langword="unmanaged"/>, <see langword="notnull"/> or <see langword="default"/>), or a class type other than <seealso cref="Delegate"/>, <seealso cref="MulticastDelegate"/> or <seealso cref="Enum"/>.</summary>
    public TypeParameterConstraintSyntax KeywordOrClassConstraint
    {
        get => keywordOrClassConstraint;
        set => keywordOrClassConstraint = value;
    }
    /// <summary>Gets or sets the <see langword="delegate"/> or <see langword="enum"/> constraint. That slot enables using one of the following class types: <seealso cref="Delegate"/>, <seealso cref="MulticastDelegate"/> or <seealso cref="Enum"/>.</summary>
    public TypeConstraintSyntax DelegateOrEnumConstraint
    {
        get => delegateOrEnumConstraint as TypeConstraintSyntax;
        set => delegateOrEnumConstraint = value;
    }
    /// <summary>Gets or sets the list of interface or type parameter constraints. Those slots enable using interfaces or type parameters as constraints.</summary>
    public List<TypeConstraintSyntax> InterfaceOrTypeParameterConstraints
    {
        get => otherConstraints;
        set
        {
            if (value is null)
                value = new();

            otherConstraints = value;
        }
    }
    /// <summary>Gets or sets the <see langword="new"/>() constraint.</summary>
    public ConstructorConstraintSyntax NewConstraint { get; set; }

    // TODO: Add ability to get the interface constraints and the type parameter constraints individually

    /// <summary>Initializes a new instance of the <seealso cref="TypeParameterConstraintClauseSegmentation"/> class from a given <see cref="TypeParameterConstraintClauseSyntax"/> and a <seealso cref="SemanticModel"/>.</summary>
    /// <param name="constraintClause">The given <seealso cref="TypeParameterConstraintClauseSyntax"/> to initialize this instance from.</param>
    /// <param name="semanticModel">The <seealso cref="SemanticModel"/> that will be taken into account when referring to types in the constraint clause.</param>
    public TypeParameterConstraintClauseSegmentation(TypeParameterConstraintClauseSyntax constraintClause, SemanticModel semanticModel)
        : this(constraintClause.Constraints, semanticModel) { }
    /// <summary>Initializes a new instance of the <seealso cref="TypeParameterConstraintClauseSegmentation"/> class from a given <see cref="TypeParameterConstraintClauseSyntax"/> and a <seealso cref="SemanticModel"/>.</summary>
    /// <param name="constraints">The given <seealso cref="TypeParameterConstraintClauseSyntax"/> to initialize this instance from.</param>
    /// <param name="model">The <seealso cref="SemanticModel"/> that will be taken into account when referring to types in the constraint clause.</param>
    public TypeParameterConstraintClauseSegmentation(IEnumerable<TypeParameterConstraintSyntax> constraints, SemanticModel model)
    {
        semanticModel = model;
        AddConstraints(constraints);
    }

    /// <summary>Adds the given constraints to the segmentation.</summary>
    /// <param name="constraints">The constraints to add.</param>
    public void AddConstraints(IEnumerable<TypeParameterConstraintSyntax> constraints)
    {
        foreach (var c in constraints)
            AddConstraint(c);
    }
    /// <summary>Adds the given constraint to the segmentation. If not an interface constraint, any existing constraint of the respective kind will be replaced.</summary>
    /// <param name="constraint">The constraint to add.</param>
    public void AddConstraint(TypeParameterConstraintSyntax constraint)
    {
        switch (constraint)
        {
            case TypeConstraintSyntax typeConstraint:
                AddTypeConstraint(typeConstraint);
                break;

            case DefaultConstraintSyntax:
            case ClassOrStructConstraintSyntax:
                keywordOrClassConstraint = constraint;
                break;

            case ConstructorConstraintSyntax constructorConstraint:
                NewConstraint = constructorConstraint;
                break;
        }
    }

    /// <summary>Adds the given type constraint to the segmentation. If the type constraint is a class type, the potentially existing class constraint will be replaced.</summary>
    /// <param name="typeConstraint">The type constraint to add.</param>
    public void AddTypeConstraint(TypeConstraintSyntax typeConstraint)
    {
        var typeSymbol = semanticModel.GetTypeInfo(typeConstraint.Type).Type;
        AddTypeConstraint(typeConstraint, typeSymbol);
    }
    /// <inheritdoc cref="AddTypeConstraint(TypeConstraintSyntax)"></inheritdoc>
    /// <param name="typeSymbol">The <seealso cref="ITypeSymbol"/> of the type that the type constraint refers to.</param>
    public void AddTypeConstraint(TypeConstraintSyntax typeConstraint, ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.TypeKind)
        {
            case TypeKind.Class:
                ref var relevantConstraint = ref keywordOrClassConstraint;
                switch (typeSymbol.SpecialType)
                {
                    case SpecialType.System_Delegate:
                    case SpecialType.System_MulticastDelegate:
                    case SpecialType.System_Enum:
                        relevantConstraint = ref delegateOrEnumConstraint;
                        break;
                }
                relevantConstraint = typeConstraint;
                return;

            case TypeKind.Interface:
            case TypeKind.TypeParameter:
                InterfaceOrTypeParameterConstraints.Add(typeConstraint);
                break;
        }
    }

    /// <summary>Constructs a new <seealso cref="TypeParameterConstraintClauseSyntax"/> for a given identifier name string.</summary>
    /// <param name="identifierName">The identifier name of the type parameter whose type constraint clause to create.</param>
    /// <returns>The resulting <seealso cref="TypeParameterConstraintClauseSyntax"/>.</returns>
    public TypeParameterConstraintClauseSyntax ToConstraintClause(string identifierName)
    {
        return ToConstraintClause(SyntaxFactory.IdentifierName(identifierName));
    }
    /// <summary>Constructs a new <seealso cref="TypeParameterConstraintClauseSyntax"/> for a given <seealso cref="IdentifierNameSyntax"/>.</summary>
    /// <param name="identifierName">The <seealso cref="IdentifierNameSyntax"/> of the type parameter whose type constraint clause to create.</param>
    /// <returns>The resulting <seealso cref="TypeParameterConstraintClauseSyntax"/>.</returns>
    public TypeParameterConstraintClauseSyntax ToConstraintClause(IdentifierNameSyntax identifierName)
    {
        return SyntaxFactory.TypeParameterConstraintClause(identifierName, ToSeparatedSyntaxList());
    }

    /// <summary>Constructs a new <seealso cref="TypeParameterConstraintClauseSyntax"/> from a given one where its constraints are replaced with this segmentation's.</summary>
    /// <param name="constraintClause">The <seealso cref="TypeParameterConstraintClauseSyntax"/> from which the result will be created.</param>
    /// <returns>The resulting <seealso cref="TypeParameterConstraintClauseSyntax"/>.</returns>
    public TypeParameterConstraintClauseSyntax WithTheseConstraints(TypeParameterConstraintClauseSyntax constraintClause)
    {
        return constraintClause.WithConstraints(ToSeparatedSyntaxList()).WithTriviaFrom(constraintClause);
    }

    /// <summary>Generates a <seealso cref="SeparatedSyntaxList{TNode}"/> containing the <seealso cref="TypeParameterConstraintSyntax"/> nodes described in this <seealso cref="TypeParameterConstraintClauseSegmentation"/> instance.</summary>
    /// <returns>The resulting <seealso cref="SeparatedSyntaxList{TNode}"/>.</returns>
    public SeparatedSyntaxList<TypeParameterConstraintSyntax> ToSeparatedSyntaxList()
    {
        var result = new List<TypeParameterConstraintSyntax>();
        if (keywordOrClassConstraint != null)
            result.Add(keywordOrClassConstraint);
        if (delegateOrEnumConstraint != null)
            result.Add(delegateOrEnumConstraint);
        if (InterfaceOrTypeParameterConstraints.Any())
            result.AddRange(InterfaceOrTypeParameterConstraints);
        if (NewConstraint != null)
            result.Add(NewConstraint);

        return SyntaxFactory.SeparatedList(result);
    }
}
