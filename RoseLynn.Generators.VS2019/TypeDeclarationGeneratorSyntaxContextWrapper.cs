using Microsoft.CodeAnalysis;
using System.Threading;

namespace RoseLynn.Generators;

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a type.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
/// <typeparam name="TTypeDeclarationSyntax">The type of the type declaration syntax node.</typeparam>
public abstract record TypeDeclarationGeneratorSyntaxContextWrapper<TTypeDeclarationSyntax>(GeneratorSyntaxContext Context)
    : GeneratorSyntaxContextWrapper(Context)

    where TTypeDeclarationSyntax : SyntaxNode
{
    /// <summary>Gets the declared symbol, as the node in the <seealso cref="GeneratorSyntaxContext"/> represents.</summary>
    /// <remarks>
    /// This is not automatically loaded; <see cref="FetchAdditionalInformation(CancellationToken)"/>
    /// must have been invoked beforehand.
    /// </remarks>
    public INamedTypeSymbol DeclaredSymbol { get; private set; }

    /// <summary>Gets the node in the <seealso cref="GeneratorSyntaxContext"/> as a <typeparamref name="TTypeDeclarationSyntax"/>.</summary>
    public TTypeDeclarationSyntax DeclarationSyntax => Context.Node as TTypeDeclarationSyntax;

    /// <summary>
    /// Fetches the declared symbol that the node in the <seealso cref="GeneratorSyntaxContext"/> represents.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that will be used to notify the operation's cancellation.</param>
    /// <remarks>
    /// This method is not automatically invoked, and may be used for lazily loading the declared symbol.
    /// </remarks>
    public override void FetchAdditionalInformation(CancellationToken cancellationToken = default)
    {
        DeclaredSymbol = Context.SemanticModel.GetDeclaredSymbol(DeclarationSyntax, cancellationToken) as INamedTypeSymbol;
        base.FetchAdditionalInformation(cancellationToken);
    }
}

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a type.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
public record TypeDeclarationGeneratorSyntaxContextWrapper(GeneratorSyntaxContext Context)
    : TypeDeclarationGeneratorSyntaxContextWrapper<SyntaxNode>(Context);
