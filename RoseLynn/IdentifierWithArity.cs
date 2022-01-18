#nullable enable

namespace RoseLynn;

/// <summary>Represents a symbol's identifier and arity.</summary>
/// <param name="Name">The identifier of the symbol.</param>
/// <param name="Arity">The arity of the symbol.</param>
public record struct IdentifierWithArity(string Name, int Arity = 0)
{
    /// <summary>Gets the full identifier of the symbol, including its arity suffix, if it is generic.</summary>
    /// <remarks>For example, an arity of 2 would result in Name`2, where an arity of 0 would return Name.</remarks>
    public string FullIdentifier
    {
        get
        {
            if (Arity is 0)
                return Name;

            return $"{Name}`{Arity}";
        }
    }

    public override string ToString() => FullIdentifier;

    /// <summary>Parses a metadata symbol name, decoding its arity and constructing an <seealso cref="IdentifierWithArity"/> instance encoding the information.</summary>
    /// <param name="metadataName">The metadata name.</param>
    /// <returns>An <seealso cref="IdentifierWithArity"/> instance that contains the name and the arity of the symbol, matching its metadata name.</returns>
    public static IdentifierWithArity Parse(string metadataName)
    {
        var split = metadataName.Split('`');

        int arity = 0;
        if (split.Length > 1)
            arity = int.Parse(split[1]);

        return new(split[0], arity);
    }
}
