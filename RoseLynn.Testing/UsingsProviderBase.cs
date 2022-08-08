using RoseLynn.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoseLynn.Testing;

/// <summary>Provides a framework to prepending usings to code bound to testing.</summary>
public abstract class UsingsProviderBase
{
    /// <summary>Provides the default usings provider that applies no usings.</summary>
    public static readonly UsingsProviderBase Default = new DefaultUsingsProvider();

    /// <summary>Provides the default necessary usings for a piece of source code that is to be tested.</summary>
    public abstract string DefaultNecessaryUsings { get; }

    /// <summary>Prepends the default necessary usings before the original source code and returns the new code with the usings.</summary>
    /// <param name="original">The original source code on which to prepend the usings.</param>
    /// <returns>The resulting source code with the given usings prepended to the original source code.</returns>
    public string WithUsings(string original) => WithUsings(original, DefaultNecessaryUsings);

    /// <summary>Prepends the specified usings before the original source code and returns the new code with the specified usings.</summary>
    /// <param name="original">The original source code on which to prepend the usings.</param>
    /// <param name="usings">The usings to prepend to the source code.</param>
    /// <returns>The resulting source code with the given usings prepended to the original source code.</returns>
    public static string WithUsings(string original, string usings) => $"{usings}\n{original}";

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
        if (sort)
            usingStatements = new SortedSet<UsingDirectiveStatementInfo>(usingStatements, UsingDirectiveStatementInfo.SortingComparer.Instance);

        var result = new StringBuilder();
        usingStatements
            .Select(statement => statement.ToString())
            .Select(result.AppendLine);

        return new VariableUsingsProvider(result.ToString());
    }

    private sealed class DefaultUsingsProvider : UsingsProviderBase
    {
        public override string DefaultNecessaryUsings => "";
    }
}

/// <summary>Provides a framework to prepending usings to code bound to testing, where the usings might vary per case.</summary>
public sealed class VariableUsingsProvider : UsingsProviderBase
{
    public override string DefaultNecessaryUsings => Usings;

    /// <summary>Gets or sets the usings that are to be applied to the piece of code that is to be tested.</summary>
    public string Usings { get; set; }

    public VariableUsingsProvider(string usings)
    {
        Usings = usings;
    }
}