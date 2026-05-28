using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Provides facts for .NET languages and extensions for the <seealso cref="NetLanguage"/> enum.</summary>
public static class LanguageFacts
{
    #region Language Names
    /// <summary>The code-friendly name for C#, equal to <c>"CSharp"</c>.</summary>
    public const string CodeFriendlyCSharp = nameof(NetLanguage.CSharp);
    /// <summary>The code-friendly name for F#, equal to <c>"FSharp"</c>.</summary>
    public const string CodeFriendlyFSharp = nameof(NetLanguage.FSharp);
    /// <summary>The code-friendly name for Visual Basic, equal to <c>"VisualBasic"</c>.</summary>
    public const string CodeFriendlyVisualBasic = nameof(NetLanguage.VisualBasic);

    /// <summary>The source file extension for C#, equal to <c>"cs"</c>.</summary>
    public const string CSharpExtension = "cs";
    /// <summary>The source file extension for F#, equal to <c>"fs"</c>.</summary>
    public const string FSharpExtension = "fs";
    /// <summary>The source file extension for Visual Basic, equal to <c>"vb"</c>.</summary>
    public const string VisualBasicExtension = "vb";
    #endregion

    /// <summary>Determines whether the given <seealso cref="NetLanguage"/> value reflects a .NET language supported by Roslyn.</summary>
    /// <param name="language">The .NET language to determine whether it's supported by Roslyn.</param>
    /// <returns><see langword="true"/> if the given language is supported by Roslyn, otherwise <see langword="false"/>.</returns>
    public static bool IsRoslynNetLanguage(this NetLanguage language)
    {
        return language is NetLanguage.CSharp
                        or NetLanguage.VisualBasic;
    }

    /// <summary>Gets the source file extension for the specified language.</summary>
    /// <param name="language">The language for which the file extension to get.</param>
    /// <returns>
    /// The file extension, excluding the period. If the given <seealso cref="NetLanguage"/>
    /// value is not known/supported, the string representation of the value is returned instead.
    /// </returns>
    public static string GetSourceFileExtension(this NetLanguage language) => language switch
    {
        NetLanguage.CSharp => CSharpExtension,
        NetLanguage.FSharp => FSharpExtension,
        NetLanguage.VisualBasic => VisualBasicExtension,

        _ => language.ToString(),
    };
    /// <summary>Maps the name of the language to a <seealso cref="NetLanguage"/> instance.</summary>
    /// <param name="languageName">
    /// The name of the language. It may be any of the <see langword="const"/> strings in
    /// <seealso cref="LanguageNames"/>, or in <seealso cref="LanguageFacts"/>. Character casing is ignored.
    /// </param>
    /// <returns>
    /// A <seealso cref="NetLanguage"/> value representing the .NET language represented by the
    /// provided language name if available, otherwise <seealso cref="NetLanguage.Unknown"/>.
    /// </returns>
    public static NetLanguage MapToNetLanguage(string languageName) => languageName switch
    {
        var name when EqualsAnyOf(name, LanguageNames.CSharp, CodeFriendlyCSharp, CSharpExtension)
            => NetLanguage.CSharp,

        var name when EqualsAnyOf(name, LanguageNames.FSharp, CodeFriendlyFSharp, FSharpExtension)
            => NetLanguage.FSharp,

        var name when EqualsAnyOf(name, LanguageNames.VisualBasic, CodeFriendlyVisualBasic, VisualBasicExtension)
            => NetLanguage.VisualBasic,

        _ => NetLanguage.Unknown,
    };

    private static bool EqualsIgnoreCase(string? left, string? right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }

    private static bool EqualsAnyOf(string? source, string target1, string target2)
    {
        return EqualsIgnoreCase(source, target1)
            || EqualsIgnoreCase(source, target2);
    }

    private static bool EqualsAnyOf(string? source, string target1, string target2, string target3)
    {
        return EqualsAnyOf(source, target1, target2)
            || EqualsIgnoreCase(source, target3);
    }

    private static bool EqualsAnyOf(string? source, string target1, string target2, string target3, string target4)
    {
        return EqualsAnyOf(source, target1, target2, target3)
            || EqualsIgnoreCase(source, target4);
    }

    /// <summary>Gets the source file extension for the specified language.</summary>
    /// <param name="languageName">The name of the language for which the file extension to get.</param>
    /// <returns>The file extension, excluding the period.</returns>
    public static string GetSourceFileExtensionForLanguage(string languageName)
    {
        return GetSourceFileExtension(MapToNetLanguage(languageName));
    }
}
