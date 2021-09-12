using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax
{
    /// <summary>Provides extensions for <seealso cref="MemberDeclarationSyntax"/> nodes that may be generic. All extensions only apply to:
    /// <list type="bullet">
    /// <item><seealso cref="TypeDeclarationSyntax"/></item>
    /// <item><seealso cref="DelegateDeclarationSyntax"/></item>
    /// <item><seealso cref="MethodDeclarationSyntax"/></item>
    /// </list>
    /// </summary>
    public static class TypeParameterizableMemberDeclarationSyntaxExtensions
    {
        /// <remarks>See <seealso cref="IdentifiableMemberDeclarationSyntaxExtensions"/> for the supported types.</remarks>
        private static void SupportedTypeRemarks() { }

        /// <summary>Determines whether the declaring member is generic.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/> related to the member.</param>
        /// <returns><see langword="true"/> if the member's arity is positive, otherwise <see langword="false"/>.</returns>
        /// <inheritdoc cref="SupportedTypeRemarks"/>
        public static bool IsGeneric(this MemberDeclarationSyntax node) => node.GetArity() > 0;

        // This is just gross

        /// <summary>Gets the arity of the declaring member.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The arity of the declaring member if it can be generic, otherwise 0.</returns>
        public static int GetArity(this MemberDeclarationSyntax node)
        {
            return node switch
            {
                TypeDeclarationSyntax     t => t.Arity,
                DelegateDeclarationSyntax d => d.Arity,
                MethodDeclarationSyntax   m => m.Arity,
                _ => 0,
            };
        }
        /// <summary>Gets the constraint clauses of the delaring member.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The constraint clauses of the declaring member if it can be generic, otherwise <see langword="default"/>.</returns>
        /// <inheritdoc cref="SupportedTypeRemarks"/>
        public static SyntaxList<TypeParameterConstraintClauseSyntax> GetConstraintClauses(this MemberDeclarationSyntax node)
        {
            return node switch
            {
                TypeDeclarationSyntax     t => t.ConstraintClauses,
                DelegateDeclarationSyntax d => d.ConstraintClauses,
                MethodDeclarationSyntax   m => m.ConstraintClauses,
                _ => default,
            };
        }
        /// <summary>Gets the type parameter list of the declaring member.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The type parameter list of the declaring member if it can be generic, otherwise <see langword="null"/>.</returns>
        /// <inheritdoc cref="SupportedTypeRemarks"/>
        public static TypeParameterListSyntax GetTypeParameterList(this MemberDeclarationSyntax node)
        {
            return node switch
            {
                TypeDeclarationSyntax     t => t.TypeParameterList,
                DelegateDeclarationSyntax d => d.TypeParameterList,
                MethodDeclarationSyntax   m => m.TypeParameterList,
                _ => null,
            };
        }

        /// <summary>Creates a copy of the given <seealso cref="MemberDeclarationSyntax"/> with the specified <seealso cref="TypeParameterListSyntax"/>.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/> whose type parameter list to change. The original instance remains unaffected.</param>
        /// <param name="typeParameterList">The <seealso cref="TypeParameterListSyntax"/> of the new node.</param>
        /// <returns>The new node with the specified <seealso cref="TypeParameterListSyntax"/>, if it can be generic, otherwise the original unaffected node.</returns>
        public static MemberDeclarationSyntax WithTypeParameterList(this MemberDeclarationSyntax node, TypeParameterListSyntax typeParameterList)
        {
            return node switch
            {
                TypeDeclarationSyntax     t => t.WithTypeParameterList(typeParameterList),
                DelegateDeclarationSyntax d => d.WithTypeParameterList(typeParameterList),
                MethodDeclarationSyntax   m => m.WithTypeParameterList(typeParameterList),
                _ => node,
            };
        }
        /// <summary>Creates a copy of the given <seealso cref="MemberDeclarationSyntax"/> with the specified <seealso cref="TypeParameterConstraintClauseSyntax"/> nodes.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/> whose constraint clauses list to change. The original instance remains unaffected.</param>
        /// <param name="constraintClauses">The list of <seealso cref="TypeParameterConstraintClauseSyntax"/> of the new node.</param>
        /// <returns>The new node with the specified <seealso cref="TypeParameterConstraintClauseSyntax"/> nodes, if it can be generic, otherwise the original unaffected node.</returns>
        public static MemberDeclarationSyntax WithConstraintClauses(this MemberDeclarationSyntax node, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return node switch
            {
                TypeDeclarationSyntax     t => t.WithConstraintClauses(constraintClauses),
                DelegateDeclarationSyntax d => d.WithConstraintClauses(constraintClauses),
                MethodDeclarationSyntax   m => m.WithConstraintClauses(constraintClauses),
                _ => node,
            };
        }

        /// <summary>Removes the type parameter list syntax node from the given <seealso cref="MemberDeclarationSyntax"/> while also maintaining exterior trivia associated with the type parameter list node.</summary>
        /// <param name="memberDeclarationNode">The <seealso cref="MemberDeclarationSyntax"/> whose type parameter list to remove. The original node remains unaffected.</param>
        /// <returns>If the given <seealso cref="MemberDeclarationSyntax"/> is generic, returns the resulting <seealso cref="MemberDeclarationSyntax"/> without a type parameter list and with its exterior trivia being merged with that of the removed type parameter list's. The resulting node is a copy of the original one without adjusting it. Otherwise, it returns the original node without any adjustments.</returns>
        public static MemberDeclarationSyntax WithoutTypeParameterList(this MemberDeclarationSyntax memberDeclarationNode)
        {
            if (!memberDeclarationNode.IsGeneric())
                return memberDeclarationNode;

            var decalarationIdentifier = memberDeclarationNode.GetIdentifier();
            var triviaAfterTypeParameterList = memberDeclarationNode.GetTypeParameterList().GetTrailingTrivia();
            var identifierWithTrivia = decalarationIdentifier.WithTrailingTrivia(decalarationIdentifier.TrailingTrivia.AddRange(triviaAfterTypeParameterList));
            var appendedTriviaResultingNode = memberDeclarationNode.WithIdentifier(identifierWithTrivia);
            return appendedTriviaResultingNode.WithTypeParameterList(null);
        }
    }
}
