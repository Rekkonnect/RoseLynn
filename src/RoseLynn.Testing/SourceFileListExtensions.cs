using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace RoseLynn.Testing;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

/// <summary>Provides helpers for adding multiple sources to Roslyn testing source-file lists.</summary>
public static class SourceFileListExtensions
{
    /// <summary>Adds multiple source strings to a source-file list.</summary>
    /// <param name="list">The target source-file list.</param>
    /// <param name="sources">The source strings to add.</param>
    public static void AddRange(this SourceFileList list, IEnumerable<string> sources)
    {
        foreach (var source in sources)
            list.Add(source);
    }
    /// <summary>Adds multiple source texts to a source-file list.</summary>
    /// <param name="list">The target source-file list.</param>
    /// <param name="sources">The source texts to add.</param>
    public static void AddRange(this SourceFileList list, IEnumerable<SourceText> sources)
    {
        foreach (var source in sources)
            list.Add(source);
    }
}
