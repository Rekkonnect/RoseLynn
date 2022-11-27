using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace RoseLynn.CSharp.Syntax;

/// <summary>Provides useful extensions for the <seealso cref="CSharpSyntaxNode"/> type.</summary>
public static class CSharpSyntaxNodeExtensions
{
    /// <summary>
    /// Gets the default <seealso cref="AttributeListTarget"/> for the given <seealso cref="CSharpSyntaxNode"/>.
    /// </summary>
    /// <param name="attributedMember">The member that is attributed.</param>
    /// <returns>
    /// The <seealso cref="AttributeListTarget"/> representing the default target
    /// for the given <seealso cref="CSharpSyntaxNode"/> kind. If the specified node
    /// does not reflect an attributable member, <seealso cref="AttributeListTarget.None"/>
    /// is returned.
    /// </returns>
    public static AttributeListTarget ResolveDefaultAttributeListTarget(this CSharpSyntaxNode attributedMember)
    {
        return attributedMember switch
        {
            FieldDeclarationSyntax => AttributeListTarget.Field,
            PropertyDeclarationSyntax => AttributeListTarget.Property,
            MethodDeclarationSyntax => AttributeListTarget.Method,
            ParameterSyntax => AttributeListTarget.Param,
            TypeParameterSyntax => AttributeListTarget.TypeVar,

            _ when attributedMember.IsNamedTypeDeclarationSyntax() => AttributeListTarget.Type,
            _ when attributedMember.IsEventDeclarationSyntax() => AttributeListTarget.Event,

            _ => AttributeListTarget.None,
        };
    }
    /// <summary>
    /// Gets the explicit interface specifier of the declared member.
    /// </summary>
    /// <param name="node">
    /// The <seealso cref="MemberDeclarationSyntax"/> representing an invokable symbol.
    /// </param>
    /// <returns>
    /// The <seealso cref="ExplicitInterfaceSpecifierSyntax"/> of the declared member,
    /// or <see langword="null"/> if the <seealso cref="MemberDeclarationSyntax"/>
    /// does not reflect the declaration of a member that can be declared in an interface.
    /// </returns>
    public static SyntaxList<AttributeListSyntax> GetAttributeLists(this CSharpSyntaxNode node)
    {
        return node switch
        {
            MemberDeclarationSyntax member => member.AttributeLists,
            LambdaExpressionSyntax lambda => lambda.AttributeLists,
            TypeParameterSyntax typeParameter => typeParameter.AttributeLists,
            AccessorDeclarationSyntax accessorDeclaration => accessorDeclaration.AttributeLists,
            StatementSyntax statement => statement.AttributeLists,
            BaseParameterSyntax parameter => parameter.AttributeLists,
            _ => default,
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

    /// <summary>
    /// Determines whether the <seealso cref="CSharpSyntaxNode"/> represents a class type declaration syntax,
    /// including a record class.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to determine if it's a class type declaration syntax.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="syntaxNode"/> is <seealso cref="ClassDeclarationSyntax"/>
    /// or <seealso cref="RecordDeclarationSyntax"/> with the <see langword="class"/> kind,
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsClassDeclarationSyntax(this CSharpSyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax
            || (syntaxNode as RecordDeclarationSyntax)?.GetRecordTypeKind() is TypeKind.Class;
    }
    /// <summary>
    /// Determines whether the <seealso cref="CSharpSyntaxNode"/> represents a struct type declaration syntax,
    /// including a record struct.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to determine if it's a struct type declaration syntax.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="syntaxNode"/> is <seealso cref="StructDeclarationSyntax"/>
    /// or <seealso cref="RecordDeclarationSyntax"/> with the <see langword="struct"/> kind,
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsStructDeclarationSyntax(this CSharpSyntaxNode syntaxNode)
    {
        return syntaxNode is StructDeclarationSyntax
            || (syntaxNode as RecordDeclarationSyntax)?.GetRecordTypeKind() is TypeKind.Struct;
    }

    /// <summary>
    /// Gets the type of the declared symbol.
    /// </summary>
    /// <param name="node">
    /// The <seealso cref="CSharpSyntaxNode"/> representing the declaration of a symbol.
    /// </param>
    /// <returns>
    /// The <seealso cref="TypeSyntax"/> for the declared member's type, or <see langword="null"/>
    /// if the <seealso cref="CSharpSyntaxNode"/> does not reflect the declaration of a symbol
    /// that can have a type, or the declared symbol is a type itself.
    /// </returns>
    public static TypeSyntax? GetSymbolDeclarationType(this CSharpSyntaxNode node)
    {
        return node switch
        {
            BasePropertyDeclarationSyntax property => property.Type,
            BaseParameterSyntax parameter => parameter.Type,
            BaseFieldDeclarationSyntax field => field.Declaration.Type,
            VariableDeclarationSyntax variable => variable.Type,
            TupleElementSyntax tupleElement => tupleElement.Type,

            _ => node.GetReturnType(),
        };
    }

    /// <summary>
    /// Gets the return type of the declared invokable symbol.
    /// </summary>
    /// <param name="node">
    /// The <seealso cref="CSharpSyntaxNode"/> representing an invokable symbol.
    /// </param>
    /// <returns>
    /// The <seealso cref="TypeSyntax"/> representing the return type declared on the
    /// invokable symbol, or <see langword="null"/> if the syntax node does not reflect
    /// the declaration of an invokable symbol, or if the invokable symbol does not
    /// support declaring a return type (anonymous methods or lambda expressions).
    /// </returns>
    /// <remarks>
    /// This method will return the value of the respective return type property on the
    /// following types:
    /// <list type="bullet">
    /// <item><seealso cref="DelegateDeclarationSyntax"/></item>
    /// <item><seealso cref="MethodDeclarationSyntax"/></item>
    /// <item><seealso cref="OperatorDeclarationSyntax"/></item>
    /// <item>
    /// <seealso cref="ConversionOperatorDeclarationSyntax"/>
    /// (returning <seealso cref="ConversionOperatorDeclarationSyntax.Type"/>)
    /// </item>
    /// <item><seealso cref="LocalFunctionStatementSyntax"/></item>
    /// </list>
    /// If the type is not any of the above, <see langword="null"/> is returned.
    /// </remarks>
    public static TypeSyntax? GetReturnType(this CSharpSyntaxNode node)
    {
        return node switch
        {
            DelegateDeclarationSyntax @delegate => @delegate.ReturnType,
            MethodDeclarationSyntax method => method.ReturnType,
            OperatorDeclarationSyntax method => method.ReturnType,
            ConversionOperatorDeclarationSyntax method => method.Type,
            LocalFunctionStatementSyntax localFunction => localFunction.ReturnType,
            _ => null,
        };
    }
    /// <summary>
    /// Gets the parameter list of the declared invokable symbol.
    /// </summary>
    /// <param name="node">
    /// The <seealso cref="CSharpSyntaxNode"/> representing the declaration of an
    /// invokable symbol.
    /// </param>
    /// <returns>
    /// This method will return the value of the respective return type property
    /// on the following types:
    /// <list type="bullet">
    /// <item><seealso cref="DelegateDeclarationSyntax"/></item>
    /// <item><seealso cref="BaseMethodDeclarationSyntax"/></item>
    /// <item><seealso cref="LocalFunctionStatementSyntax"/></item>
    /// <item><seealso cref="AnonymousMethodExpressionSyntax"/></item>
    /// <item><seealso cref="ParenthesizedLambdaExpressionSyntax"/></item>
    /// </list>
    /// If the type is not any of the above, <see langword="null"/> is returned.
    /// </returns>
    public static ParameterListSyntax? GetParameterList(this CSharpSyntaxNode node)
    {
        return node switch
        {
            DelegateDeclarationSyntax @delegate => @delegate.ParameterList,
            BaseMethodDeclarationSyntax method => method.ParameterList,
            LocalFunctionStatementSyntax localFunction => localFunction.ParameterList,
            AnonymousMethodExpressionSyntax anonymousFunction => anonymousFunction.ParameterList,
            ParenthesizedLambdaExpressionSyntax parenthesizedLambda => parenthesizedLambda.ParameterList,
            _ => null,
        };
    }
    /// <summary>
    /// Gets the modifier of the declared symbol.
    /// </summary>
    /// <param name="node">
    /// The <seealso cref="CSharpSyntaxNode"/> representing the declaration of a symbol.
    /// </param>
    /// <returns>
    /// This method will return the value of the respective modifier property on the
    /// following types:
    /// <list type="bullet">
    /// <item><seealso cref="MemberDeclarationSyntax"/></item>
    /// <item><seealso cref="BaseParameterSyntax"/></item>
    /// <item><seealso cref="LocalDeclarationStatementSyntax"/></item>
    /// <item><seealso cref="LocalFunctionStatementSyntax"/></item>
    /// <item><seealso cref="AccessorDeclarationSyntax"/></item>
    /// <item><seealso cref="AnonymousFunctionExpressionSyntax"/></item>
    /// </list>
    /// If the type is not any of the above, <see langword="default"/> is returned.
    /// </returns>
    public static SyntaxTokenList GetModifiers(this CSharpSyntaxNode node)
    {
        return node switch
        {
            MemberDeclarationSyntax method => method.Modifiers,
            BaseParameterSyntax parameter => parameter.Modifiers,
            LocalDeclarationStatementSyntax local => local.Modifiers,
            LocalFunctionStatementSyntax localFunction => localFunction.Modifiers,
            AccessorDeclarationSyntax accessor => accessor.Modifiers,
            AnonymousFunctionExpressionSyntax anonymousFunction => anonymousFunction.Modifiers,
            _ => default,
        };
    }

    /// <summary>Gets the identifier of the declaring member.</summary>
    /// <param name="node">The <seealso cref="CSharpSyntaxNode"/> representing the declaration of an identifiable symbol.</param>
    /// <returns>The identifier of the declaring member if it has an identifier, otherwise <see langword="default"/>.</returns>
    public static SyntaxToken GetIdentifier(this CSharpSyntaxNode node)
    {
        return node switch
        {
            BaseTypeDeclarationSyntax n => n.Identifier,
            DelegateDeclarationSyntax n => n.Identifier,
            EnumMemberDeclarationSyntax n => n.Identifier,
            MethodDeclarationSyntax n => n.Identifier,
            ConstructorDeclarationSyntax n => n.Identifier,
            PropertyDeclarationSyntax n => n.Identifier,
            EventDeclarationSyntax n => n.Identifier,
            LocalFunctionStatementSyntax n => n.Identifier,
            VariableDeclaratorSyntax n => n.Identifier,
            SingleVariableDesignationSyntax n => n.Identifier,
            TupleElementSyntax n => n.Identifier,

            // LINQ
            FromClauseSyntax n => n.Identifier,
            LetClauseSyntax n => n.Identifier,
            JoinClauseSyntax n => n.Identifier,
            JoinIntoClauseSyntax n => n.Identifier,

            // Statements
            LabeledStatementSyntax n => n.Identifier,
            ForEachStatementSyntax n => n.Identifier,
            CatchDeclarationSyntax n => n.Identifier,
            ExternAliasDirectiveSyntax n => n.Identifier,
            AttributeTargetSpecifierSyntax n => n.Identifier,
            TypeParameterSyntax n => n.Identifier,

            _ => default,
        };
    }

    /// <summary>Creates a new <seealso cref="CSharpSyntaxNode"/> with the specified identifier.</summary>
    /// <typeparam name="TSyntax">The type of the <seealso cref="CSharpSyntaxNode"/> to create.</typeparam>
    /// <param name="node">The <seealso cref="CSharpSyntaxNode"/> from which to create the new node.</param>
    /// <param name="identifier">The identifier of the created node.</param>
    /// <returns>The resulting node with the specified identifier, if it can have one, otherwise <see langword="null"/>.</returns>
    public static TSyntax? WithIdentifier<TSyntax>(this TSyntax node, SyntaxToken identifier)
        where TSyntax : CSharpSyntaxNode
    {
        CSharpSyntaxNode? withIdentifier = node switch
        {
            // Since the underlying WithIdentifier always returns the peak type
            // we needn't support each type individually, provided the WithIdentifier
            // method in the base type

            BaseTypeDeclarationSyntax n => n.WithIdentifier(identifier),
            DelegateDeclarationSyntax n => n.WithIdentifier(identifier),
            EnumMemberDeclarationSyntax n => n.WithIdentifier(identifier),
            MethodDeclarationSyntax n => n.WithIdentifier(identifier),
            ConstructorDeclarationSyntax n => n.WithIdentifier(identifier),
            PropertyDeclarationSyntax n => n.WithIdentifier(identifier),
            EventDeclarationSyntax n => n.WithIdentifier(identifier),
            LocalFunctionStatementSyntax n => n.WithIdentifier(identifier),
            VariableDeclaratorSyntax n => n.WithIdentifier(identifier),
            SingleVariableDesignationSyntax n => n.WithIdentifier(identifier),
            TupleElementSyntax n => n.WithIdentifier(identifier),

            // LINQ
            FromClauseSyntax n => n.WithIdentifier(identifier),
            LetClauseSyntax n => n.WithIdentifier(identifier),
            JoinClauseSyntax n => n.WithIdentifier(identifier),
            JoinIntoClauseSyntax n => n.WithIdentifier(identifier),

            // Statements
            LabeledStatementSyntax n => n.WithIdentifier(identifier),
            ForEachStatementSyntax n => n.WithIdentifier(identifier),
            CatchDeclarationSyntax n => n.WithIdentifier(identifier),
            ExternAliasDirectiveSyntax n => n.WithIdentifier(identifier),
            AttributeTargetSpecifierSyntax n => n.WithIdentifier(identifier),
            TypeParameterSyntax n => n.WithIdentifier(identifier),

            _ => null,
        };
        return withIdentifier as TSyntax;
    }
}
