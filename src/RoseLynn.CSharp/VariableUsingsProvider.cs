namespace RoseLynn.CSharp;

/// <summary>Provides a framework to prepending usings to code snippets, where the usings might vary per case.</summary>
public sealed class VariableUsingsProvider : UsingsProviderBase
{
    /// <inheritdoc/>
    public override string DefaultNecessaryUsings => Usings;

    /// <summary>Gets or sets the usings for a piece of source code.</summary>
    public string Usings { get; set; }

    /// <summary>Initializes a new <see cref="VariableUsingsProvider"/> with the given using directives string.</summary>
    /// <param name="usings">The using directives to prepend.</param>
    public VariableUsingsProvider(string usings)
    {
        Usings = usings;
    }
}