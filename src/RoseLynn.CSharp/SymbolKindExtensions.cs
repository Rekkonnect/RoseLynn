using Microsoft.CodeAnalysis;
using RoseLynn.CSharp.Syntax;

namespace RoseLynn.CSharp;

/// <summary>Contains C#-specific extensions for the <seealso cref="SymbolKind"/> enum.</summary>
public static class SymbolKindExtensions
{
    /// <summary>Gets the respective <seealso cref="AttributeListTarget"/> that matches the given <seealso cref="SymbolKind"/>.</summary>
    /// <param name="symbolKind">The symbol kind for which to get the respective <seealso cref="AttributeListTarget"/>.</param>
    /// <returns>The respective <seealso cref="AttributeListTarget"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the given <seealso cref="SymbolKind"/> is not a valid attribute list target
    /// (i.e. <seealso cref="SymbolKind.ErrorType"/>, <seealso cref="SymbolKind.Namespace"/>, etc.).
    /// </exception>
    public static AttributeListTarget GetAttributeListTarget(this SymbolKind symbolKind)
    {
        return symbolKind switch
        {
            SymbolKind.Method => AttributeListTarget.Method,
            SymbolKind.Property => AttributeListTarget.Property,
            SymbolKind.Event => AttributeListTarget.Event,
            SymbolKind.Field => AttributeListTarget.Field,
            SymbolKind.NamedType => AttributeListTarget.Type,
            SymbolKind.Parameter => AttributeListTarget.Param,
            SymbolKind.Assembly => AttributeListTarget.Assembly,
            SymbolKind.NetModule => AttributeListTarget.Module,

            _ => throw new ArgumentException("The given symbol kind cannot be attributed."),
        };
    }
}
