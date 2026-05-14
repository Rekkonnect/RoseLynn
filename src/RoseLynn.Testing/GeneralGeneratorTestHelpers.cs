using Garyon.Objects.Strings;
using Microsoft.CodeAnalysis.Text;
using RoseLynn.Writing;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoseLynn.Testing;

/// <summary>Provides helpers shared by source-generator tests.</summary>
public static class GeneralGeneratorTestHelpers
{
    /// <summary>Creates UTF-8 C# source text with normalized CRLF newlines.</summary>
    /// <param name="source">The C# source text.</param>
    /// <returns>The normalized UTF-8 source text.</returns>
    public static SourceText Utf8CSharpSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string source)
    {
        var normalizedSource = source.NormalizeNewLines(WhitespaceFacts.CrLf);
        return SourceText.From(normalizedSource, Encoding.UTF8, SourceHashAlgorithm.Sha256);
    }
}

