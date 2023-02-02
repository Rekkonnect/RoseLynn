using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a method.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
/// <typeparam name="TMethodDeclarationSyntax">The type of the method declaration syntax node.</typeparam>
public abstract record MethodDeclarationGeneratorSyntaxContextWrapper<TMethodDeclarationSyntax>(GeneratorSyntaxContext Context)
    : DeclarationGeneratorSyntaxContextWrapper<TMethodDeclarationSyntax, IMethodSymbol>(Context)

    where TMethodDeclarationSyntax : SyntaxNode;

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a method.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
public record MethodDeclarationGeneratorSyntaxContextWrapper(GeneratorSyntaxContext Context)
    : MethodDeclarationGeneratorSyntaxContextWrapper<SyntaxNode>(Context);
