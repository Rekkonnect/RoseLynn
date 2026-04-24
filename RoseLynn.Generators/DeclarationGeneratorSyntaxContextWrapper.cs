using Microsoft.CodeAnalysis;
using System.Threading;

namespace RoseLynn.Generators;

#nullable enable

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> applied on a node representing the declaration of a symbol.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
/// <typeparam name="TDeclarationSyntax">The type of the declaration syntax node.</typeparam>
/// <typeparam name="TDeclaredSymbol">The type of the declared symbol.</typeparam>
public abstract record DeclarationGeneratorSyntaxContextWrapper<TDeclarationSyntax, TDeclaredSymbol>(GeneratorSyntaxContext Context)
    : GeneratorSyntaxContextWrapper(Context)

    where TDeclarationSyntax : SyntaxNode
    where TDeclaredSymbol : class, ISymbol
{
    /// <summary>Gets the declared symbol, as the node in the <seealso cref="GeneratorSyntaxContext"/> represents.</summary>
    /// <remarks>
    /// This is not automatically loaded; <see cref="FetchAdditionalInformation(CancellationToken)"/>
    /// must have been invoked beforehand.
    /// </remarks>
    public TDeclaredSymbol? DeclaredSymbol { get; private set; }

    /// <summary>Gets the node in the <seealso cref="GeneratorSyntaxContext"/> as a <typeparamref name="TDeclarationSyntax"/>.</summary>
    public TDeclarationSyntax? DeclarationSyntax => Context.Node as TDeclarationSyntax;

    /// <summary>
    /// Fetches the declared symbol that the node in the <seealso cref="GeneratorSyntaxContext"/> represents.
    /// If the <seealso cref="DeclarationSyntax"/> is not of the specified type, it is considered <see langword="null"/>
    /// and thus the <seealso cref="DeclaredSymbol"/> will also be <see langword="null"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that will be used to notify the operation's cancellation.</param>
    /// <remarks>
    /// This method is not automatically invoked, and may be used for lazily loading the declared symbol.
    /// </remarks>
    public override void FetchAdditionalInformation(CancellationToken cancellationToken = default)
    {
        if (DeclarationSyntax is not null)
        {
            DeclaredSymbol = Context.SemanticModel.GetDeclaredSymbol(DeclarationSyntax, cancellationToken) as TDeclaredSymbol;
        }

        base.FetchAdditionalInformation(cancellationToken);
    }
}
