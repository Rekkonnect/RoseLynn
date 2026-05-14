using Microsoft.CodeAnalysis;

namespace RoseLynn.Testing;

// dotnet/roslyn:/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/Core/EmbeddedLanguages/PredefinedEmbeddedLanguageClassifierNames.cs
/// <summary>Defines embedded-language names used by test-facing string-syntax annotations.</summary>
public static class PredefinedEmbeddedLanguageNames
{
    /// <summary>The embedded-language name for regex content.</summary>
    public const string Regex = nameof(Regex);

    /// <summary>The embedded-language name for JSON content.</summary>
    public const string Json = nameof(Json);

    /// <summary>The embedded-language name for C# source used in tests.</summary>
    public const string CSharpTest = $"{LanguageNames.CSharp}-Test";
}
