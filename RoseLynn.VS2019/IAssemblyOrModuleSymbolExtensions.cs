using Microsoft.CodeAnalysis;

#nullable enable

namespace RoseLynn;

/// <summary>Provides extensions for the <seealso cref="IAssemblySymbol"/> and <seealso cref="IModuleSymbol"/> types.</summary>
public static class IAssemblyOrModuleSymbolExtensions
{
    /// <summary>Gets the global namespace of the <seealso cref="IAssemblySymbol"/> or <seealso cref="IModuleSymbol"/>.</summary>
    /// <param name="symbol">The <seealso cref="IAssemblySymbol"/> or <seealso cref="IModuleSymbol"/> whose global namespace to get.</param>
    /// <returns>
    /// The value of the <seealso cref="IAssemblySymbol.GlobalNamespace"/> or <seealso cref="IModuleSymbol.GlobalNamespace"/> property,
    /// otherwise <see langword="null"/> if the <seealso cref="ISymbol"/> is not an <seealso cref="IAssemblySymbol"/> or <seealso cref="IModuleSymbol"/>.
    /// </returns>
    public static INamespaceSymbol? GetGlobalNamespace(this ISymbol? symbol)
    {
        return symbol switch
        {
            IAssemblySymbol assemblySymbol => assemblySymbol.GlobalNamespace,
            IModuleSymbol moduleSymbol => moduleSymbol.GlobalNamespace,

            _ => null,
        };
    }
}
