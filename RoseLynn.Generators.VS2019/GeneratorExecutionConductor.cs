﻿using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using RoseLynn.Utilities;
using System.Collections.Generic;

namespace RoseLynn.Generators;

#nullable enable

// The vocabularial choice required effort
/// <summary>Provides an internal caching mechanism for more deliberately handling the added sources from a generator.</summary>
public sealed class GeneratorExecutionConductor
{
    private readonly GeneratedSourceMappings sourceMappings;
    private string extension = string.Empty;

    /// <summary>Gets the <seealso cref="IGeneratorExecutionContext"/> that this instance reflects upon.</summary>
    public IGeneratorExecutionContext Context { get; }

    /// <summary>Gets the generated sources dictionary.</summary>
    public IDictionary<string, SourceText> GeneratedSources => sourceMappings;
    /// <summary>Gets the <seealso cref="IComparer{T}"/> for the hint name, by which the dictionary keys are sorted.</summary>
    public IComparer<string> HintNameComparer => sourceMappings.Comparer;

    /// <summary>Gets or sets the extension that will be appended to the end of the hint name of the generated sources.</summary>
    /// <remarks>Defaults to the empty string. A <see langword="null"/> value also represents the empty string.</remarks>
    public string Extension
    {
        get => extension;
        set
        {
            extension = value.EnsureStartsWith(".");
        }
    }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="GeneratorExecutionConductor"/> class from a
    /// <seealso cref="GeneratorExecutionContext"/> instance.
    /// </summary>
    /// <param name="context">
    /// The <seealso cref="GeneratorExecutionContext"/> that links the compilation that the genrator acts upon.
    /// </param>
    /// <remarks>
    /// Upon construction, <see cref="Extension"/> is set to the detected language of the compilation
    /// the context refers to.
    /// </remarks>
    public GeneratorExecutionConductor(GeneratorExecutionContext context)
        : this(context, Comparer<string>.Default) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="GeneratorExecutionConductor"/> class from a
    /// <seealso cref="GeneratorExecutionContext"/> instance and a custom <seealso cref="IComparer{T}"/>
    /// for the <seealso cref="HintNameComparer"/>.
    /// </summary>
    /// <param name="hintNameComparer">The custom hint name comparer that will be used for <seealso cref="HintNameComparer"/>.</param>
    /// <inheritdoc cref="GeneratorExecutionConductor(GeneratorExecutionContext)"/>
    public GeneratorExecutionConductor(GeneratorExecutionContext context, IComparer<string> hintNameComparer)
        : this(context.RoseContext(), hintNameComparer) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="GeneratorExecutionConductor"/> class from an
    /// <seealso cref="IGeneratorExecutionContext"/> instance.
    /// </summary>
    /// <param name="context">
    /// The <seealso cref="IGeneratorExecutionContext"/> that links the compilation that the genrator acts upon.
    /// </param>
    /// <remarks>
    /// Upon construction, <see cref="Extension"/> is set to the detected language of the compilation
    /// the context refers to.
    /// </remarks>
    public GeneratorExecutionConductor(IGeneratorExecutionContext context)
        : this(context, Comparer<string>.Default) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="GeneratorExecutionConductor"/> class from an
    /// <seealso cref="IGeneratorExecutionContext"/> instance and a custom <seealso cref="IComparer{T}"/>
    /// for the <seealso cref="HintNameComparer"/>.
    /// </summary>
    /// <param name="hintNameComparer">The custom hint name comparer that will be used for <seealso cref="HintNameComparer"/>.</param>
    /// <inheritdoc cref="GeneratorExecutionConductor(IGeneratorExecutionContext)"/>
    public GeneratorExecutionConductor(IGeneratorExecutionContext context, IComparer<string> hintNameComparer)
    {
        Context = context;
        sourceMappings = new(hintNameComparer);
        SetCommonGeneratedSourceFileExtension(context.CompilationLanguage);
    }

    /// <summary>Registers a source that will be added to the provided context.</summary>
    /// <param name="source">The source to add.</param>
    /// <remarks>Calling this method will not immediately add the source. It is rather internally stored and only added along with the other registered sources through <seealso cref="FinalizeGeneratorExecution"/>.</remarks>
    /// <inheritdoc cref="HintNameDocSample(string)"/>
    public void AddSource(string hintName, string source)
    {
        sourceMappings.Add(FullHintName(hintName), source);
    }
    /// <inheritdoc cref="AddSource(string, string)"/>
    public void AddSource(string hintName, SourceText source)
    {
        sourceMappings.Add(FullHintName(hintName), source);
    }

    /// <inheritdoc cref="AddSource(string, string)"/>
    /// <param name="source">The source to add.</param>
    /// <param name="usingsProvider">The <seealso cref="UsingsProviderBase"/> whose usings to prepend to the given source.</param>
    public void AddSource(string hintName, string source, UsingsProviderBase usingsProvider)
    {
        AddSource(hintName, usingsProvider.WithUsings(source));
    }

    /// <summary>Removes the source that was added with the given hint name.</summary>
    /// <inheritdoc cref="HintNameDocSample(string)"/>
    public void RevokeAddedSource(string hintName)
    {
        sourceMappings.Remove(FullHintName(hintName));
    }
    /// <summary>Removes all sources that were added.</summary>
    public void ClearAddedSources()
    {
        sourceMappings.Clear();
    }

    /// <summary>Gets the added source with the given hint name.</summary>
    /// <returns>The <seealso cref="SourceText"/> that is mapped by the given hint name, or <see langword="null"/> if not found.</returns>
    /// <inheritdoc cref="HintNameDocSample(string)"/>
    public SourceText? GetAddedSource(string hintName)
    {
        return sourceMappings.ValueOrDefault(FullHintName(hintName));
    }
    /// <summary>Sets the added source with the given hint name to a new source.</summary>
    /// <param name="source">The new source to map the hint name with.</param>
    /// <inheritdoc cref="HintNameDocSample(string)"/>
    public void SetAddedSource(string hintName, string source)
    {
        sourceMappings.Set(FullHintName(hintName), source);
    }
    /// <inheritdoc cref="SetAddedSource(string, string)"/>
    public void SetAddedSource(string hintName, SourceText source)
    {
        sourceMappings[FullHintName(hintName)] = source;
    }

    private string FullHintName(string extensionlessHintName) => $"{extensionlessHintName}{extension}";

    /// <param name="hintName">The hint name of the source. <seealso cref="Extension"/> will be appended to the end of it.</param>
    private static void HintNameDocSample(string hintName) { }

    /// <summary>
    /// Considers the source generation process complete for the given instance.
    /// As a result, all generated sources are added in the order they are sorted, given the <seealso cref="HintNameComparer"/>.
    /// </summary>
    /// <remarks>
    /// After adding the sources, they remain cached, and will be added again after a new invocation of this method.
    /// Consider clearing the source cache with <seealso cref="ClearAddedSources"/>, or alternatively call the <seealso cref="FinalizeClearGeneratorExecution"/> method.
    /// </remarks>
    public void FinalizeGeneratorExecution()
    {
        foreach (var sourceMapping in sourceMappings)
            Context.AddSource(sourceMapping.Key, sourceMapping.Value);
    }
    /// <summary>
    /// Considers the source generation process complete for the given instance.
    /// As a result, all generated sources are added in the order they are sorted, given the <seealso cref="HintNameComparer"/>.
    /// </summary>
    public void FinalizeClearGeneratorExecution()
    {
        FinalizeGeneratorExecution();
        ClearAddedSources();
    }

    /// <summary>
    /// Sets the <seealso cref="Extension"/> to the commonly-used generated source hint name extension ".g.[language]".
    /// </summary>
    /// <param name="language">The language of the generated sources.</param>
    public void SetCommonGeneratedSourceFileExtension(NETLanguage language)
    {
        if (language is NETLanguage.Unknown)
            return;

        extension = GetCommonGeneratedSourceFileExtension(language);
    }
    private static string GetCommonGeneratedSourceFileExtension(NETLanguage language)
    {
        return $".g.{LanguageFacts.GetSourceFileExtension(language)}";
    }
}
