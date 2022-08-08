using System;

namespace RoseLynn.CSharp.Syntax;

/// <summary>Denotes the kind of a <see langword="using"/> statement at the top of the source.</summary>
[Flags]
public enum UsingDirectiveKind
{
    /// <summary>Denotes that no <see langword="using"/> statement.</summary>
    None = 0,

    /// <summary>Denotes the <see langword="using"/> statement that imports a namespace.</summary>
    Using = 1 << 0,
    /// <summary>Denotes the <see langword="using"/> statement that creates an alias for a type.</summary>
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
}

/// <summary>Contains extension methods for the <seealso cref="UsingDirectiveKind"/> enum.</summary>
public static class UsingDirectiveKindExtensions
{
    public static bool IsGlobal(this UsingDirectiveKind kind) => kind.HasFlag(UsingDirectiveKind.Global);
    public static bool IsStatic(this UsingDirectiveKind kind) => kind.HasFlag(UsingDirectiveKind.Static);

    public static UsingDirectiveKind GetDirectiveKind(this UsingDirectiveKind kind) => kind & UsingDirectiveKind.UsingDirectiveMask;
}
