#nullable enable

using Microsoft.CodeAnalysis;
using System;

namespace RoseLynn;

/// <summary>Represents the symbol kind of a container symbol, that is a symbol that contains another symbol's definition.</summary>
public enum ContainerSymbolKind
{
    /// <summary>Represents any kind of container symbol.</summary>
    Any = -1,

    /// <summary>Represents the assembly container symbol kind.</summary>
    Assembly = 0,
    /// <summary>Represents the module container symbol kind.</summary>
    Module,
    /// <summary>Represents the namespace container symbol kind.</summary>
    Namespace,
    /// <summary>Represents the type container symbol kind.</summary>
    Type,
    /// <summary>Represents the method container symbol kind.</summary>
    Method,
}

/// <summary>Contains extensions for the <seealso cref="SymbolKind"/> enum.</summary>
public static class ContainerSymbolKindExtensions
{
    /// <summary>Gets the respective <seealso cref="ContainerSymbolKind"/> value represented by the given <seealso cref="SymbolKind"/> value.</summary>
    /// <param name="symbolKind">The <seealso cref="SymbolKind"/> whose represented <seealso cref="ContainerSymbolKind"/> to get.</param>
    /// <returns>The respective <seealso cref="ContainerSymbolKind"/> value represented by the given <seealso cref="SymbolKind"/> value.</returns>
    /// <exception cref="ArgumentException">Thrown when the given <seealso cref="SymbolKind"/> does not represent a <seealso cref="ContainerSymbolKind"/>, such as <seealso cref="SymbolKind.Local"/>.</exception>
    public static ContainerSymbolKind ToContainerSymbolKind(this SymbolKind symbolKind)
    {
        return symbolKind switch
        {
            SymbolKind.Assembly => ContainerSymbolKind.Assembly,
            SymbolKind.NetModule => ContainerSymbolKind.Module,
            SymbolKind.Namespace => ContainerSymbolKind.Namespace,
            SymbolKind.Method => ContainerSymbolKind.Method,

            _ when symbolKind.IsTypeSymbolKind() => ContainerSymbolKind.Type,

            _ => throw new ArgumentException("Invalid container symbol kind."),
        };
    }

    /// <summary>Gets the respective <seealso cref="SymbolNameMatchingLevel"/> value represented by the given <seealso cref="ContainerSymbolKind"/> value.</summary>
    /// <param name="kind">The <seealso cref="ContainerSymbolKind"/> whose represented <seealso cref="SymbolNameMatchingLevel"/> to get.</param>
    /// <returns>The respective <seealso cref="SymbolNameMatchingLevel"/> value represented by the given <seealso cref="ContainerSymbolKind"/> value.</returns>
    /// <exception cref="ArgumentException">Thrown when the given <seealso cref="ContainerSymbolKind"/> is not a valid container symbol kind, such as <seealso cref="ContainerSymbolKind.Any"/>.</exception>
    public static SymbolNameMatchingLevel GetRespectiveSymbolNameMatchingLevel(this ContainerSymbolKind kind)
    {
        return kind switch
        {
            ContainerSymbolKind.Method => SymbolNameMatchingLevel.ContainerMethod,
            ContainerSymbolKind.Type => SymbolNameMatchingLevel.ContainerType,
            ContainerSymbolKind.Namespace => SymbolNameMatchingLevel.Namespace,
            ContainerSymbolKind.Module => SymbolNameMatchingLevel.Module,
            ContainerSymbolKind.Assembly => SymbolNameMatchingLevel.Assembly,

            _ => throw new ArgumentException("Invalid container symbol kind."),
        };
    }
}
