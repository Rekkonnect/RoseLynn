#nullable enable

using Microsoft.CodeAnalysis;

namespace RoseLynn;

/// <summary>Represents the topmost level of the symbol's container members whose names must be equal to match two <seealso cref="ISymbol"/> instances.</summary>
public enum SymbolNameMatchingLevel
{
    /// <summary>Only require matching the symbol names.</summary>
    SymbolName,
    /// <summary>Require matching the symbol names and their container methods' order and names.</summary>
    ContainerMethod,
    /// <summary>Require matching the symbol names, their container methods' order and names, and their container types' order and names.</summary>
    ContainerType,
    /// <summary>Require matching the symbol names, their container methods' order and names, their container types' order and names, and their namespaces' order and names.</summary>
    Namespace,
    /// <summary>Require matching the symbol names, their container methods' order and names, their container types' order and names, their namespaces' order and names and their module information.</summary>
    Module,
    /// <summary>Require matching the symbol names, their container methods' order and names, their container types' order and names, their namespaces' order and names, their module and their assembly information.</summary>
    Assembly,
}

/// <summary>Contains extensions for the <seealso cref="SymbolNameMatchingLevel"/> enum.</summary>
public static class SymbolNameMatchingLevelExtensions
{
    /// <summary>Gets the respective <seealso cref="ContainerSymbolKind"/> value represented by the given <seealso cref="SymbolNameMatchingLevel"/> value.</summary>
    /// <param name="level">The <seealso cref="SymbolNameMatchingLevel"/> whose represented <seealso cref="ContainerSymbolKind"/> to get.</param>
    /// <returns>The respective <seealso cref="ContainerSymbolKind"/> value represented by the given <seealso cref="SymbolNameMatchingLevel"/> value.</returns>
    public static ContainerSymbolKind GetRespectiveContainerSymbolKind(this SymbolNameMatchingLevel level)
    {
        return level switch
        {
            SymbolNameMatchingLevel.ContainerMethod => ContainerSymbolKind.Method,
            SymbolNameMatchingLevel.ContainerType => ContainerSymbolKind.Type,
            SymbolNameMatchingLevel.Namespace => ContainerSymbolKind.Namespace,
            SymbolNameMatchingLevel.Module => ContainerSymbolKind.Module,
            SymbolNameMatchingLevel.Assembly => ContainerSymbolKind.Assembly,
        };
    }
}