using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLynn.CSharp.Syntax
{
    /// <summary>Provides extensions for <seealso cref="MemberDeclarationSyntax"/> nodes that may be generic. All extensions only apply to <seealso cref="TypeDeclarationSyntax"/>, <seealso cref="DelegateDeclarationSyntax"/> and <seealso cref="MethodDeclarationSyntax"/>.</summary>
    public static class TypeParameterizableMemberDeclarationSyntaxExtensions
    {
        /// <summary>Determines whether the declaring member is generic.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/> related to the member.</param>
        /// <returns><see langword="true"/> if the member's arity is positive, otherwise <see langword="false"/>.</returns>
        public static bool IsGeneric(this MemberDeclarationSyntax node) => node.GetArity() > 0;

        // This is just gross

        /// <summary>Gets the arity of the declaring member.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The arity of the declaring member if it can be generic, otherwise 0.</returns>
        public static int GetArity(this MemberDeclarationSyntax node)
        {
            switch (node)
            {
                case TypeDeclarationSyntax t:
                    return t.Arity;
                case DelegateDeclarationSyntax d:
                    return d.Arity;
                case MethodDeclarationSyntax m:
                    return m.Arity;
            }
            return 0;
        }
        /// <summary>Gets the constraint clauses of the delaring member.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The constraint clauses of the declaring member if it can be generic, otherwise <see langword="default"/>.</returns>
        public static SyntaxList<TypeParameterConstraintClauseSyntax> GetConstraintClauses(this MemberDeclarationSyntax node)
        {
            switch (node)
            {
                case TypeDeclarationSyntax t:
                    return t.ConstraintClauses;
                case DelegateDeclarationSyntax d:
                    return d.ConstraintClauses;
                case MethodDeclarationSyntax m:
                    return m.ConstraintClauses;
            }
            return default;
        }
        /// <summary>Gets the type parameter list of the declaring member.</summary>
        /// <param name="node">The <seealso cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The type parameter list of the declaring member if it can be generic, otherwise <see langword="null"/>.</returns>
        public static TypeParameterListSyntax GetTypeParameterList(this MemberDeclarationSyntax node)
        {
            switch (node)
            {
                case TypeDeclarationSyntax t:
                    return t.TypeParameterList;
                case DelegateDeclarationSyntax d:
                    return d.TypeParameterList;
                case MethodDeclarationSyntax m:
                    return m.TypeParameterList;
            }
            return null;
        }

        public static MemberDeclarationSyntax WithTypeParameterList(this MemberDeclarationSyntax node, TypeParameterListSyntax typeParameterList)
        {
            switch (node)
            {
                case TypeDeclarationSyntax t:
                    return t.WithTypeParameterList(typeParameterList);
                case DelegateDeclarationSyntax d:
                    return d.WithTypeParameterList(typeParameterList);
                case MethodDeclarationSyntax m:
                    return m.WithTypeParameterList(typeParameterList);
            }
            return null;
        }
        public static MemberDeclarationSyntax WithConstraintClauses(this MemberDeclarationSyntax node, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            switch (node)
            {
                case TypeDeclarationSyntax t:
                    return t.WithConstraintClauses(constraintClauses);
                case DelegateDeclarationSyntax d:
                    return d.WithConstraintClauses(constraintClauses);
                case MethodDeclarationSyntax m:
                    return m.WithConstraintClauses(constraintClauses);
            }
            return null;
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
