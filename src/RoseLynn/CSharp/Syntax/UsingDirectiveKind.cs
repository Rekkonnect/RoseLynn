using System;
using System.ComponentModel;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Denotes the kind of a <see langword="using"/> statement at the top of the source.</summary>
[Flags]
public enum UsingDirectiveKind
{
    /// <summary>Denotes that no <see langword="using"/> statement.</summary>
    None = 0,

    /// <summary>Denotes the <see langword="using"/> statement that imports a namespace.</summary>
    Using = 1 << 0,
    /// <summary>Denotes the <see langword="using"/> statement that creates an alias for a symbol.</summary>
    UsingAlias = 2 << 0,

    /// <summary>
    /// Denotes the mask that represents the using directive statement kind,
    /// excluding the <see langword="static"/> and <see langword="global"/> modifiers.
    /// </summary>
    UsingDirectiveMask = Using | UsingAlias,

    /// <summary>Denotes the <see langword="static"/> modifier for a <see langword="using"/> statement.</summary>
    Static = 1 << 2,
    /// <summary>Denotes the <see langword="global"/> modifier for a <see langword="using"/> statement.</summary>
    Global = 1 << 3,

    /// <summary>Denotes the <see langword="global using"/> statement.</summary>
    GlobalUsing = Global | Using,
    /// <summary>Denotes the <see langword="using static"/> statement.</summary>
    UsingStatic = Using | Static,
    /// <summary>Denotes the <see langword="global using static"/> statement.</summary>
    GlobalUsingStatic = GlobalUsing | Static,

    /// <summary>Denotes the <see langword="global using"/> statement that creates an alias for a symbol.</summary>
    GlobalUsingAlias = Global | UsingAlias,
}

/// <summary>Contains extension methods for the <seealso cref="UsingDirectiveKind"/> enum.</summary>
public static class UsingDirectiveKindExtensions
{
    public static bool IsGlobal(this UsingDirectiveKind kind) => kind.HasFlag(UsingDirectiveKind.Global);
    public static bool IsStatic(this UsingDirectiveKind kind) => kind.HasFlag(UsingDirectiveKind.Static);

    public static UsingDirectiveKind WithGlobal(this UsingDirectiveKind kind, bool isGlobal)
    {
        return WithFlag(kind, UsingDirectiveKind.Global, isGlobal);
    }
    public static UsingDirectiveKind WithStatic(this UsingDirectiveKind kind, bool isStatic)
    {
        return WithFlag(kind, UsingDirectiveKind.Static, isStatic);
    }
    public static UsingDirectiveKind WithFlag(this UsingDirectiveKind kind, UsingDirectiveKind flag, bool toggle)
    {
        if (toggle)
            kind |= flag;
        else
            kind &= ~flag;

        return kind;
    }

    public static UsingDirectiveKind GetDirectiveKind(this UsingDirectiveKind kind) => kind & UsingDirectiveKind.UsingDirectiveMask;

    /// <summary>Determines whether the <seealso cref="UsingDirectiveKind"/> contains a directive kind flag.</summary>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> value whose directive kind to evaluate.</param>
    /// <returns><see langword="true"/> if <seealso cref="GetDirectiveKind(UsingDirectiveKind)"/> returns a non-zero value, otherwise <see langword="false"/>.</returns>
    public static bool HasDirectiveKind(this UsingDirectiveKind kind)
    {
        return kind.GetDirectiveKind() is not 0;
    }
    /// <summary>
    /// Validates that the <seealso cref="UsingDirectiveKind"/> contains a directive kind flag,
    /// throwing an exception upon absence of such a flag.
    /// </summary>
    /// <param name="kind">The <seealso cref="UsingDirectiveKind"/> value whose directive kind presence to assert.</param>
    /// <exception cref="InvalidEnumArgumentException">
    /// Thrown if the provided <seealso cref="UsingDirectiveKind"/> value does not have a directive kind flag.
    /// </exception>
    public static void ValidateDirectiveKindPresence(this UsingDirectiveKind kind)
    {
        if (!HasDirectiveKind(kind))
        {
            throw new InvalidEnumArgumentException("The using directive kind must contain a directive kind (using or alias).");
        }
    }
}
