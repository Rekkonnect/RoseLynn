using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace RoseLynn.Writing;

/// <summary>
/// Provides a simple mechanism for mapping sources to their file hint names,
/// disregarding the order in which they were added.
/// </summary>
/// <remarks>
/// This is especially useful for testing purposes for generators, where the order
/// of the generated files can be non-deterministic, and the test intends to capture
/// independently-generated files.
/// </remarks>
public sealed class SourceCollection : SortedDictionary<string, SourceText>
{
    /// <summary>
    /// Gets or sets the default encoding to use when adding new sources using the <see langword="string"/> overloads.
    /// </summary>
    public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

    public SourceCollection() { }
    public SourceCollection(IComparer<string> hintNameComparer)
        : base(hintNameComparer) { }

    /// <summary>
    /// Adds a generated source to the dictionary, with the provided hint name.
    /// </summary>
    /// <param name="hintName">The hint name of the generated source that will be mapped to the source.</param>
    /// <param name="source">The generated source that will be added to the dictionary.</param>
    public void Add(string hintName, string source)
    {
        Add(hintName, SourceText.From(source, DefaultEncoding));
    }

    /// <summary>
    /// Sets the generated source of the provided hint name to a different generated source.
    /// </summary>
    /// <param name="hintName">The hint name of the generated source that will be mapped to the new source.</param>
    /// <param name="source">The new generated source that will be mapped to the given hint name.</param>
    public void Set(string hintName, string source)
    {
        this[hintName] = SourceText.From(source, DefaultEncoding);
    }
}
