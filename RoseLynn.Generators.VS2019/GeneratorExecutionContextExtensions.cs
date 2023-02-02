using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Provides helper extensions for the <seealso cref="GeneratorExecutionContext"/> type.</summary>
public static class GeneratorExecutionContextExtensions
{
    /// <summary>Gets the <seealso cref="NETLanguage"/> value representing the source language of the compilation the generator is executed on.</summary>
    /// <param name="context">The <seealso cref="GeneratorExecutionContext"/> whose source language to parse into a <seealso cref="NETLanguage"/> value.</param>
    /// <returns>The <seealso cref="NETLanguage"/> of the compilation that the generator is executed on.</returns>
    public static NETLanguage GetCompilationLanguage(this GeneratorExecutionContext context)
    {
        return context.Compilation.GetNETLanguage();
    }
}
