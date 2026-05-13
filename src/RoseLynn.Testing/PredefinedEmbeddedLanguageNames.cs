using Microsoft.CodeAnalysis;

namespace RoseLynn.Testing;

// dotnet/roslyn:/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/Core/EmbeddedLanguages/PredefinedEmbeddedLanguageClassifierNames.cs
public static class PredefinedEmbeddedLanguageNames
{
    public const string Regex = nameof(Regex);

    public const string Json = nameof(Json);

    public const string CSharpTest = $"{LanguageNames.CSharp}-Test";
}
