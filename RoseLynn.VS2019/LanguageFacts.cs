using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoseLynn;

/// <summary>Provides facts for .NET languages and extensions for the <seealso cref="NETLanguage"/> enum.</summary>
public static class LanguageFacts
{
    #region Language Names
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public const string CodeFriendlyCSharp = nameof(NETLanguage.CSharp);
    public const string CodeFriendlyFSharp = nameof(NETLanguage.FSharp);
    public const string CodeFriendlyVisualBasic = nameof(NETLanguage.VisualBasic);
    
    public const string CSharpExtension = "cs";
    public const string FSharpExtension = "fs";
    public const string VisualBasicExtension = "vb";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    #endregion

    private static readonly NETLanguageNameDictionary languageNameMappings = new();

    static LanguageFacts()
    {
        languageNameMappings.MapRange(NETLanguage.CSharp, 
            LanguageNames.CSharp, CodeFriendlyCSharp, CSharpExtension);

        languageNameMappings.MapRange(NETLanguage.FSharp,
            LanguageNames.FSharp, CodeFriendlyFSharp, FSharpExtension);

        languageNameMappings.MapRange(NETLanguage.VisualBasic,
            LanguageNames.VisualBasic, CodeFriendlyVisualBasic, VisualBasicExtension);
    }

    /// <summary>Determines whether the given <seealso cref="NETLanguage"/> value reflects a .NET language supported by Roslyn.</summary>
    /// <param name="language">The .NET language to determine whether it's supported by Roslyn.</param>
    /// <returns><see langword="true"/> if the given language is supported by Roslyn, otherwise <see langword="false"/>.</returns>
    public static bool IsRoslynNETLanguage(this NETLanguage language)
    {
        return language is NETLanguage.CSharp
                        or NETLanguage.VisualBasic;
    }

    /// <summary>Gets the source file extension for the specified language.</summary>
    /// <param name="language">The language for which the file extension to get.</param>
    /// <returns>
    /// The file extension, excluding the period. If the given <seealso cref="NETLanguage"/>
    /// value is not known/supported, the string representation of the value is returned instead.
    /// </returns>
    public static string GetSourceFileExtension(this NETLanguage language) => language switch
    {
        NETLanguage.CSharp => CSharpExtension,
        NETLanguage.FSharp => FSharpExtension,
        NETLanguage.VisualBasic => VisualBasicExtension,

        _ => language.ToString(),
    };
    /// <summary>Maps the name of the language to a <seealso cref="NETLanguage"/> instance.</summary>
    /// <param name="languageName">
    /// The name of the language. It may be any of the <see langword="const"/> strings in
    /// <seealso cref="LanguageNames"/>, or in <seealso cref="LanguageFacts"/>. Character casing is ignored.
    /// </param>
    /// <returns>
    /// A <seealso cref="NETLanguage"/> value representing the .NET language represented by the
    /// provided language name if available, otherwise <seealso cref="NETLanguage.Unknown"/>.
    /// </returns>
    public static NETLanguage MapToNETLanguage(string languageName) => languageNameMappings.MapToNETLanguage(languageName);

    /// <summary>Gets the source file extension for the specified language.</summary>
    /// <param name="languageName">The name of the language for which the file extension to get.</param>
    /// <returns>The file extension, excluding the period.</returns>
    public static string GetSourceFileExtensionForLanguage(string languageName)
    {
        return GetSourceFileExtension(MapToNETLanguage(languageName));
    }

    private sealed class NETLanguageNameDictionary : Dictionary<string, NETLanguage>
    {
        public void Map(string name, NETLanguage language)
        {
            Add(name.ToLower(), language);
        }
        
        public void MapRange(NETLanguage language, params string[] names)
        {
            foreach (var name in names)
                Map(name, language);
        }

        public NETLanguage MapToNETLanguage(string name)
        {
            if (TryGetValue(name.ToLower(), out var language))
                return language;
            
            return NETLanguage.Unknown;
        }
    }
}
