using RoseLynn.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn;

/// <summary>Provides a framework to prepending usings to code snippets.</summary>
public abstract class UsingsProviderBase
{
    /// <summary>The default separator used when initializing a usings provider.</summary>
    public const string DefaultSeparator = "\r\n";

    /// <summary>Provides the default usings provider that applies no usings.</summary>
    public static UsingsProviderBase Default => new DefaultUsingsProvider();

    /// <summary>Provides the default necessary usings for a piece of source code.</summary>
    public abstract string DefaultNecessaryUsings { get; }

    /// <summary>The separator that will be applied when prepending usings to the original source code.</summary>
    public string Separator { get; set; } = DefaultSeparator;

    /// <summary>Prepends the default necessary usings before the original source code and returns the new code with the usings.</summary>
    /// <param name="original">The original source code on which to prepend the usings.</param>
    /// <returns>The resulting source code with the given usings prepended to the original source code.</returns>
    public string WithUsings(string original) => WithUsings(original, DefaultNecessaryUsings, Separator);

    /// <summary>Prepends the default necessary usings before the original source code and returns the new code with the usings.</summary>
    /// <param name="original">The original source code on which to prepend the usings.</param>
    /// <param name="separator">The separator between the usings and the source code.</param>
    /// <returns>The resulting source code with the given usings prepended to the original source code.</returns>
    public string WithUsings(string original, string separator)
    {
        Separator = separator;
        return WithUsings(original);
    }

    /// <summary>Prepends the specified usings before the original source code and returns the new code with the specified usings.</summary>
    /// <param name="original">The original source code on which to prepend the usings.</param>
    /// <param name="usings">The usings to prepend to the source code.</param>
    /// <param name="separator">The separator between the usings and the source code.</param>
    /// <returns>The resulting source code with the given usings prepended to the original source code.</returns>
    public static string WithUsings(string original, string usings, string separator = DefaultSeparator) => $"{usings}{separator}{original}";

    /// <summary>
    /// Creates an instance of a <seealso cref="UsingsProviderBase"/> for the specified qualified names
    /// using the specified <seealso cref="UsingDirectiveKind"/> for all of them, optionally sorting them too.
    /// </summary>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> to apply to all the usings.</param>
    /// <param name="sort">
    /// <see langword="true"/> if the using directives will be sorted based on
    /// <seealso cref="UsingDirectiveStatementInfo.SortingComparer"/>, otherwise <see langword="false"/>.
    /// </param>
    /// <param name="qualifiedNames">The qualified names that will be present in the using directives.</param>
    /// <remarks></remarks>
    public static UsingsProviderBase ForUsings(UsingDirectiveKind kind, bool sort, params string[] qualifiedNames)
    {
        var factory = UsingDirectiveStatementInfo.DirectiveFactoryForKind(kind);
        return CreateForUsings(qualifiedNames.Select(factory), sort);
    }

    /// <summary>Creates a <seealso cref="UsingsProviderBase"/> for the specified using directive statements.</summary>
    /// <param name="usingStatements">The using statements that are to be included.</param>
    /// <param name="sort">Determines whether the usings will be sorted according to <seealso cref="UsingDirectiveStatementInfo.SortingComparer"/>.</param>
    /// <returns>
    /// A <seealso cref="UsingsProviderBase"/> instance whose <seealso cref="DefaultNecessaryUsings"/>
    /// property returns the string representation of the collection of using statements, it the order
    /// they were enumerated.
    /// </returns>
    public static UsingsProviderBase CreateForUsings(IEnumerable<UsingDirectiveStatementInfo> usingStatements, bool sort = false)
    {
        var usingDirectiveList = UsingDirectiveStatementInfoList.CreateOrCurrent(usingStatements, sort);
        return new VariableUsingsProvider(usingDirectiveList.ToString());
    }

    private sealed class DefaultUsingsProvider : UsingsProviderBase
    {
        public override string DefaultNecessaryUsings => "";
    }
}
