using Microsoft.CodeAnalysis;
using System.Threading;
using System;

namespace RoseLynn;

using SyntaxPredicate = Func<SyntaxNode, CancellationToken, bool>;

#nullable enable

/// <summary>
/// Provides useful extensions for <seealso cref="SyntaxValueProvider"/>.
/// </summary>
public static class SyntaxValueProviderExtensions
{
    // The ForAttributeWithMetadataName extensions come from
    // https://github.com/hikarin522/VisitorPatternGenerator/blob/master/VisitorPatternGenerator/Extensions/SyntaxValueProviderExtensions.cs
    // Drop them a star if you see this

    #region Attributes
    /// <inheritdoc cref="ForAttributeWithMetadataName(SyntaxValueProvider, string, SyntaxPredicate?)"/>
    /// <typeparam name="T">
    /// The type whose fully qualified metadata name will be used to compare
    /// the nodes' attributes.
    /// </typeparam>
    public static IncrementalValuesProvider<GeneratorAttributeSyntaxContext> ForAttributeWithMetadataName<T>(
        this SyntaxValueProvider provider,
        SyntaxPredicate? predicate = null)

        where T : Attribute
    {
        return provider.ForAttributeWithMetadataName(typeof(T), predicate);
    }

    /// <inheritdoc cref="ForAttributeWithMetadataName(SyntaxValueProvider, string, SyntaxPredicate?)"/>
    /// <param name="type">
    /// The runtime type whose fully qualified metadata name will be used to compare
    /// the nodes' attributes.
    /// </param>
    public static IncrementalValuesProvider<GeneratorAttributeSyntaxContext> ForAttributeWithMetadataName(
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        this SyntaxValueProvider provider,
        Type type,
        SyntaxPredicate? predicate = null)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
    {
        return provider.ForAttributeWithMetadataName(type.FullName, predicate);
    }

    /// <summary>
    /// Provides a simple <seealso cref="IncrementalValuesProvider{TValues}"/> for
    /// an attribute with the given metadata name.
    /// </summary>
    /// <param name="provider">
    /// The <seealso cref="SyntaxValueProvider"/> providing the incremental values provider.
    /// </param>
    /// <param name="fullyQualifiedMetadataName">
    /// The fully qualified metadata name of the attribute type.
    /// </param>
    /// <param name="predicate">
    /// The predicate filtering the attribute nodes.
    /// Providing <see langword="null"/> will result in all nodes being evaluated.
    /// </param>
    public static IncrementalValuesProvider<GeneratorAttributeSyntaxContext> ForAttributeWithMetadataName(
        this SyntaxValueProvider provider,
        string fullyQualifiedMetadataName,
        SyntaxPredicate? predicate = null)
    {
        return provider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName,
            predicate ?? (static (_, _) => true),
            static (context, _) => context);
    }
    #endregion
}
