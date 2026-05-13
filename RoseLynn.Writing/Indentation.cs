namespace RoseLynn.Writing;

/// <summary>
/// Represents an indentation style, specifying the character and the number of
/// times to repeat the character.
/// </summary>
/// <param name="Character">The repeated character denoting the indentation.</param>
/// <param name="Size">
/// The number of times to repeat the character per indentation level.
/// This value is multiplied by the level of indentation that is currently applied.
/// </param>
public record struct Indentation(char Character, int Size)
{
    public static Indentation SingleTab => new('\t', 1);
    public static Indentation DoubleTab => new('\t', 2);

    public static Indentation TwoSpaces => new(' ', 2);
    public static Indentation ThreeSpaces => new(' ', 3);
    public static Indentation FourSpaces => new(' ', 4);
    public static Indentation SixSpaces => new(' ', 6);
    public static Indentation EightSpaces => new(' ', 8);
}
