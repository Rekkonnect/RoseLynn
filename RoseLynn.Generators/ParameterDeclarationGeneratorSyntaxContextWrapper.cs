using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a parameter.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
/// <typeparam name="TParameterDeclarationSyntax">The type of the parameter declaration syntax node.</typeparam>
public abstract record ParameterDeclarationGeneratorSyntaxContextWrapper<TParameterDeclarationSyntax>(GeneratorSyntaxContext Context)
    : DeclarationGeneratorSyntaxContextWrapper<TParameterDeclarationSyntax, IParameterSymbol>(Context)

    where TParameterDeclarationSyntax : SyntaxNode;

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a parameter.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
public record ParameterDeclarationGeneratorSyntaxContextWrapper(GeneratorSyntaxContext Context)
    : ParameterDeclarationGeneratorSyntaxContextWrapper<SyntaxNode>(Context);
