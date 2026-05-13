using Garyon.Functions;
using Microsoft.CodeAnalysis;
using RoseLynn.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace RoseLynn.Generators;

[NeedsDocs]
public static class GeneratorExtensions
{
    /// <summary>
    /// Provides the same provider as
    /// <see cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}(
    ///     string, Func{SyntaxNode, CancellationToken, bool},
    ///     Func{GeneratorAttributeSyntaxContext, CancellationToken, T})"/>
    /// but without the need for a filter method.
    /// This comes in handy especially since it's recommended to avoid filtering
    /// the provider, and instead only rely on the transformation.
    /// </summary>
    public static IncrementalValuesProvider<T> ForAttributeWithMetadataName<T>(
        this SyntaxValueProvider context,
        string fullyQualifiedMetadataName,
        Func<GeneratorAttributeSyntaxContext, CancellationToken, T> transform)
    {
        return context.ForAttributeWithMetadataName(fullyQualifiedMetadataName, FilterTrue, transform);

        static bool FilterTrue(SyntaxNode node, CancellationToken token)
        {
            return true;
        }
    }

    /// <summary>
    /// Creates an <see cref="IncrementalValueProvider{T}"/> with the provided
    /// values deriving from the initial discovery of an attribute with the
    /// given metadata name, and eventually grouped with a key. 
    /// </summary>
    /// <param name="attributeTypeName">
    /// The metadata name of the attribute type to register the syntax
    /// provider for.
    /// </param>
    /// <param name="getGroupKey">
    /// Gets the group key from the given attribute. This function can return
    /// <see langword="null"/> when the context is not valid to include in the
    /// final generation.
    /// </param>
    /// <param name="constructGroup">
    /// Construts the group out of the group key. The key must contain enough
    /// information at the stage of executing this method, so that the context
    /// can be inferred.
    /// </param>
    /// <remarks>
    /// This utility is useful for registering a source provider to act on
    /// groups of symbols, which groups may derive from a symbol container,
    /// such as the containing type of the attributed members for instance.
    /// To take advantage of the incrementality on this provider, ensure that
    /// both the group key and the group itself are leniently but correctly
    /// comparable for equality.
    /// <br/>
    /// After all, this utility might not be so useful.
    /// </remarks>
    public static IncrementalValueProvider<ImmutableArray<TGroup>> ForGroupedValuesFromAttribute<TGroupKey, TGroup>(
        this IncrementalGeneratorInitializationContext context,
        string attributeTypeName,
        Func<GeneratorAttributeSyntaxContext, CancellationToken, TGroupKey?> getGroupKey,
        Func<TGroupKey, CancellationToken, TGroup> constructGroup)
    {
        return context.SyntaxProvider
            .ForAttributeWithMetadataName(
                attributeTypeName,
                getGroupKey)
            .WhereNotNull()
            .Select(constructGroup)
            .Collect();
    }

    /// <summary>
    /// Registers a source output based on the distinct values of the provider's
    /// array value. The performed action will be run against the distinct
    /// values of the array.
    /// </summary>
    public static void RegisterDistinctSourceOutput<T>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<ImmutableArray<T>> provider,
        Action<SourceProductionContext, T> action)
    {
        var distinctProvider = provider
            .Select(s => s.Distinct().ToEquatableImmutableArray());

        context.RegisterSourceOutput(distinctProvider, GenerateSources);

        void GenerateSources(
            SourceProductionContext context,
            EquatableImmutableArray<T> values)
        {
            foreach (var item in values)
            {
                action(context, item);
            }
        }
    }

    public static IncrementalValuesProvider<T> WhereNotNull<T>(
        this IncrementalValuesProvider<T?> provider)
    {
        return provider.Where(Predicates.NotNull)!;
    }

    public static IncrementalValueProvider<TResult> Select<TSource, TResult>(
        this IncrementalValueProvider<TSource> provider,
        Func<TSource, TResult> selector)
    {
        return provider.Select((item, _) => selector(item));
    }

    public static GeneratorDriverCompilationExecutionStep RunExecutionStep(
        this GeneratorDriver driver,
        Compilation initialCompilation,
        CancellationToken cancellationToken)
    {
        var resultingDriver = driver.RunGeneratorsAndUpdateCompilation(
            initialCompilation, out var resultCompilation, out _, cancellationToken);
        return new(
            OriginalDriver: driver,
            OriginalCompilation: initialCompilation,
            ResultingDriver: resultingDriver,
            ResultingCompilation: resultCompilation);
    }

    public static void ColdStart(
        this GeneratorDriver driver,
        Compilation initialCompilation,
        int iterations,
        CancellationToken cancellationToken)
    {
        foreach (var _1 in Enumerable.Range(0, iterations))
        {
            driver.RunGeneratorsAndUpdateCompilation(
                initialCompilation,
                out _,
                out _,
                cancellationToken);
        }
    }

    public static IEnumerable<ISourceGenerator> AsSourceGenerators(
        this IEnumerable<IIncrementalGenerator> generators)
    {
        return generators.Select(static g => g.AsSourceGenerator());
    }
}
