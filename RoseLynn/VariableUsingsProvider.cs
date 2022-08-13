namespace RoseLynn;

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