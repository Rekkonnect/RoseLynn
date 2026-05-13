using Garyon.Objects.Strings;
using Microsoft.CodeAnalysis.Text;
using RoseLynn.Writing;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoseLynn.Testing;

public static class GeneralGeneratorTestHelpers
{
    public static SourceText Utf8CSharpSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string source)
    {
        var normalizedSource = source.NormalizeNewLines(WhitespaceFacts.CrLf);
        return SourceText.From(normalizedSource, Encoding.UTF8, SourceHashAlgorithm.Sha256);
    }
}

