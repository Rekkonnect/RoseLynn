#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

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

    /// <inheritdoc cref="WithTypeArgumentsCSharp(string[])"/>
    public string WithTypeArgumentsCSharp(IEnumerable<string> typeArguments)
    {
        return WithTypeArgumentsCSharp(typeArguments.ToArray());
    }
    /// <summary>Gets the C# string representation of using the symbol with the specified type arguments.</summary>
    /// <param name="typeArguments">The type arguments to use in the generic symbol. The argument count must match <seealso cref="Arity"/>.</param>
    /// <returns>The string representation of the substituted symbol with the specified type arguments, or simply its name if its arity is 0.</returns>
    /// <exception cref="ArgumentException">Thrown when the given arguments' count do not match the arity of the symbol.</exception>
    public string WithTypeArgumentsCSharp(params string[] typeArguments)
    {
        return WithTypeArguments(typeArguments, "<", ">");
    }

    /// <inheritdoc cref="WithTypeArgumentsVisualBasic(string[])"/>
    public string WithTypeArgumentsVisualBasic(IEnumerable<string> typeArguments)
    {
        return WithTypeArgumentsVisualBasic(typeArguments.ToArray());
    }
    /// <summary>Gets the Visual Basic string representation of using the symbol with the specified type arguments.</summary>
    /// <param name="typeArguments">The type arguments to use in the generic symbol. The argument count must match <seealso cref="Arity"/>.</param>
    /// <returns>The string representation of the substituted symbol with the specified type arguments, or simply its name if its arity is 0.</returns>
    /// <exception cref="ArgumentException">Thrown when the given arguments' count do not match the arity of the symbol.</exception>
    public string WithTypeArgumentsVisualBasic(params string[] typeArguments)
    {
        return WithTypeArguments(typeArguments, "(Of ", ")");
    }

    private string WithTypeArguments(string[] typeArguments, string opener, string closer)
    {
        if (Arity != typeArguments.Length)
            throw new ArgumentException("The given type arguments' count does not match the arity of the symbol.");

        if (Arity is 0)
            return Name;

        return $"{Name}{opener}{string.Join(", ", typeArguments)}{closer}";
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
