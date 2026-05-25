using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;

namespace RoseLynn;

/// <summary>Provides facts for .NET languages and extensions for the <seealso cref="NetLanguage"/> enum.</summary>
public static class LanguageFacts
{
    #region Language Names
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public const string CodeFriendlyCSharp = nameof(NetLanguage.CSharp);
    public const string CodeFriendlyFSharp = nameof(NetLanguage.FSharp);
    public const string CodeFriendlyVisualBasic = nameof(NetLanguage.VisualBasic);
    
    public const string CSharpExtension = "cs";
    public const string FSharpExtension = "fs";
    public const string VisualBasicExtension = "vb";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    #endregion

    private static readonly NETLanguageNameDictionary languageNameMappings = new();

    static LanguageFacts()
    {
        languageNameMappings.MapRange(NetLanguage.CSharp, 
            LanguageNames.CSharp, CodeFriendlyCSharp, CSharpExtension);

        languageNameMappings.MapRange(NetLanguage.FSharp,
            LanguageNames.FSharp, CodeFriendlyFSharp, FSharpExtension);

        languageNameMappings.MapRange(NetLanguage.VisualBasic,
            LanguageNames.VisualBasic, CodeFriendlyVisualBasic, VisualBasicExtension);
    }

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
    public static NetLanguage MapToNETLanguage(string languageName) => languageNameMappings.MapToNETLanguage(languageName);

    /// <summary>Gets the source file extension for the specified language.</summary>
    /// <param name="languageName">The name of the language for which the file extension to get.</param>
    /// <returns>The file extension, excluding the period.</returns>
    public static string GetSourceFileExtensionForLanguage(string languageName)
    {
        return GetSourceFileExtension(MapToNETLanguage(languageName));
    }

    /// <summary>Stores case-insensitive mappings from language names to <see cref="NetLanguage"/> values.</summary>
    private sealed class NETLanguageNameDictionary : Dictionary<string, NetLanguage>
    {
        /// <summary>Maps a language name to a <see cref="NetLanguage"/> value.</summary>
        /// <param name="name">The language name to map.</param>
        /// <param name="language">The mapped language value.</param>
        public void Map(string name, NetLanguage language)
        {
            Add(name.ToLower(), language);
        }
        
        /// <summary>Maps multiple language names to the same <see cref="NetLanguage"/> value.</summary>
        /// <param name="language">The mapped language value.</param>
        /// <param name="names">The names to associate with the language.</param>
        public void MapRange(NetLanguage language, params string[] names)
        {
            foreach (var name in names)
                Map(name, language);
        }

        /// <summary>Maps a language name to a <see cref="NetLanguage"/>, falling back to <see cref="NetLanguage.Unknown"/>.</summary>
        /// <param name="name">The language name to map.</param>
        /// <returns>The mapped language value.</returns>
        public NetLanguage MapToNETLanguage(string name)
        {
            if (TryGetValue(name.ToLower(), out var language))
                return language;
            
            return NetLanguage.Unknown;
        }
    }
}
