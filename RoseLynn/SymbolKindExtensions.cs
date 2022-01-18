using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Contains extensions for the <seealso cref="SymbolKind"/> enum.</summary>
public static class SymbolKindExtensions
{
    /// <summary>Gets the respective <seealso cref="SymbolFilter"/> that includes the given <seealso cref="SymbolKind"/>.</summary>
    /// <param name="kind">The symbol kind for which to get the respective <seealso cref="SymbolFilter"/>.</param>
    /// <returns>The respective <seealso cref="SymbolFilter"/>. If the <seealso cref="SymbolKind"/> is not included by any <seealso cref="SymbolFilter"/>, <seealso cref="SymbolFilter.None"/> is returned.</returns>
    public static SymbolFilter GetRespectiveSymbolFilter(this SymbolKind kind)
    {
        return kind switch
        {
            SymbolKind.Namespace => SymbolFilter.Namespace,

            SymbolKind.Method or
            SymbolKind.Property or
            SymbolKind.Field or
            SymbolKind.Event => SymbolFilter.Member,

            _ when kind.IsTypeSymbolKind() => SymbolFilter.Type,

            _ => SymbolFilter.None,
        };
    }

    /// <summary>Determines whether a symbol kind represents a type symbol.</summary>
    /// <param name="symbolKind">The <seealso cref="SymbolKind"/> to determine whether it represents a type symbol.</param>
    /// <returns><see langword="true"/> if the given <seealso cref="SymbolKind"/> represents a type symbol, even <seealso cref="SymbolKind.ErrorType"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsTypeSymbolKind(this SymbolKind symbolKind)
    {
        return symbolKind
            is SymbolKind.NamedType
            or SymbolKind.ArrayType
            or SymbolKind.DynamicType
            or SymbolKind.TypeParameter
            or SymbolKind.PointerType
            or SymbolKind.ErrorType
            or SymbolKind.FunctionPointerType;
    }
}
