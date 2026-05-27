using Garyon.Extensions;
using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Contains extensions for the <seealso cref="Compilation"/> class.</summary>
public static class CompilationExtensions
{
    /// <summary>Given a <seealso cref="ISymbol"/> from another <seealso cref="Compilation"/>, gets the symbol matching the same <seealso cref="SymbolKind"/> and <seealso cref="FullSymbolName"/>.</summary>
    /// <param name="compilation">The <seealso cref="Compilation"/> whose <seealso cref="ISymbol"/> will be returned, if matched.</param>
    /// <param name="match">The matching symbol whose kind and symbol name will be respected.</param>
    /// <param name="matchingLevel">The level at which the symbol names will be compared.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <seealso cref="ISymbol"/> contained in <paramref name="compilation"/> that matches the name and kind of <paramref name="match"/>,
    /// or <see langword="null"/> if the operation has been cancelled, or no symbol matched the requested symbol's properties.
    /// </returns>
    public static ISymbol? GetMatchingSymbol(
        this Compilation compilation,
        ISymbol? match,
        SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace,
        CancellationToken cancellationToken = default)
    {
        if (match is null)
            return null;

        var symbols = compilation.GetSymbolsWithName(match.Name, match.GetRespectiveSymbolFilter(), cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return null;

        return symbols.FirstOrDefault(symbol => symbol.MatchesKindAndFullSymbolName(match, matchingLevel));
    }

    /// <summary>Gets all the nodes of all the syntax trees in the given <seealso cref="Compilation"/> of the specified type.</summary>
    /// <typeparam name="T">The type of the nodes to get.</typeparam>
    /// <param name="compilation">The <seealso cref="Compilation"/> whose syntax trees' nodes of the specified type to get.</param>
    /// <returns>A collection of <seealso cref="SyntaxNode"/> of the specified type contained in all the syntax trees of the specified <seealso cref="Compilation"/>.</returns>
    public static IEnumerable<T> NodesOfType<T>(this Compilation compilation)
        where T : SyntaxNode
    {
        return compilation.SyntaxTrees.Select(SyntaxTreeExtensions.NodesOfType<T>).SelectMany(nodes => nodes);
    }

    /// <summary>
    /// Gets all the symbols in the given <seealso cref="Compilation"/> that match the specified <seealso cref="SymbolFilter"/>.
    /// </summary>
    /// <param name="compilation">The <seealso cref="Compilation"/> whose </param>
    /// <returns></returns>
    public static IEnumerable<INamedTypeSymbol> GetAllDefinedTypes(this Compilation compilation)
    {
        return compilation.Assembly.GlobalNamespace.GetAllContainedTypes();
    }

    /// <summary>Gets all the referenced assembly or module symbols of the <seealso cref="Compilation"/>.</summary>
    /// <param name="compilation">The <seealso cref="Compilation"/> whose referenced assembly or module symbols to get.</param>
    /// <returns>A collection of <seealso cref="IAssemblySymbol"/> or <seealso cref="IModuleSymbol"/> instances reflecting the referenced assembly or module symbols.</returns>
    public static IEnumerable<ISymbol> GetAllAssemblyOrModuleSymbols(this Compilation compilation)
    {
        return compilation.References
            .Select(compilation.GetAssemblyOrModuleSymbol)
            .WhereNotNull();
    }
    /// <summary>Gets all the referenced assembly symbols of the <seealso cref="Compilation"/>.</summary>
    /// <param name="compilation">The <seealso cref="Compilation"/> whose referenced assembly symbols to get.</param>
    /// <returns>A collection of <seealso cref="IAssemblySymbol"/> instances reflecting the referenced assembly symbols.</returns>
    public static IEnumerable<ISymbol> GetAllAssemblySymbols(this Compilation compilation)
    {
        return compilation.SourceModule.ReferencedAssemblySymbols
                          .ConcatSingleValue(compilation.Assembly);
    }

    /// <summary>Gets the <seealso cref="NetLanguage"/> value representing the source language of the compilation.</summary>
    /// <param name="compilation">The compilation whose source language to parse into a <seealso cref="NetLanguage"/> value.</param>
    /// <returns>The <seealso cref="NetLanguage"/> of the compilation.</returns>
    public static NetLanguage GetNetLanguage(this Compilation compilation)
    {
        return LanguageFacts.MapToNETLanguage(compilation.Language);
    }

    extension<TCompilation>(TCompilation compilation)
        where TCompilation : Compilation
    {
        /// <summary>Replaces <paramref name="tree"/> in the compilation with a new tree whose text is <paramref name="text"/>.</summary>
        /// <param name="tree">The syntax tree to replace.</param>
        /// <param name="text">The new source text.</param>
        /// <returns>A new compilation with the updated syntax tree.</returns>
        public Compilation ReplaceSyntaxTree(
            SyntaxTree tree,
            string text)
        {
            var newTree = tree.WithChangedText(text);
            return compilation.ReplaceSyntaxTree(tree, newTree);
        }

        /// <summary>Removes all syntax trees generated by <paramref name="driver"/> from the compilation.</summary>
        /// <param name="driver">The generator driver whose generated trees to remove.</param>
        /// <returns>A new compilation without the generated syntax trees.</returns>
        public Compilation RemoveGeneratedTrees(
            GeneratorDriver driver)
        {
            var runResult = driver.GetRunResult();
            var generatedTrees = runResult.GeneratedTrees;
            return compilation.RemoveSyntaxTrees(generatedTrees);
        }
    }
}
