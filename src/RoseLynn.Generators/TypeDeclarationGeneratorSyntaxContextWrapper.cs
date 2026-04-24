using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a type.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
/// <typeparam name="TTypeDeclarationSyntax">The type of the type declaration syntax node.</typeparam>
public abstract record TypeDeclarationGeneratorSyntaxContextWrapper<TTypeDeclarationSyntax>(GeneratorSyntaxContext Context)
    : DeclarationGeneratorSyntaxContextWrapper<TTypeDeclarationSyntax, ITypeSymbol>(Context)

    where TTypeDeclarationSyntax : SyntaxNode;

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a type.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
public record TypeDeclarationGeneratorSyntaxContextWrapper(GeneratorSyntaxContext Context)
    : TypeDeclarationGeneratorSyntaxContextWrapper<SyntaxNode>(Context);
