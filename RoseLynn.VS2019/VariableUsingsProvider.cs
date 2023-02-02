namespace RoseLynn;

/// <summary>Provides a framework to prepending usings to code snippets, where the usings might vary per case.</summary>
public sealed class VariableUsingsProvider : UsingsProviderBase
{
    /// <inheritdoc/>
    public override string DefaultNecessaryUsings => Usings;

    /// <summary>Gets or sets the usings for a piece of source code.</summary>
    public string Usings { get; set; }

    public VariableUsingsProvider(string usings)
    {
        Usings = usings;
    }
}