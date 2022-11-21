using Microsoft.CodeAnalysis;
using RoseLynn.CSharp;
using RoseLynn.Utilities;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable annotations

namespace RoseLynn;

/// <summary>Provides a mechanism to store all <seealso cref="INamedTypeSymbol"/>s used in registered <seealso cref="ITypeSymbol"/>s.</summary>
public sealed class TypeSymbolUsageInfo
{
#pragma warning disable RS1024 // Compare symbols correctly
    private readonly HashSet<INamedTypeSymbol> usedTypes = new(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly

    /// <summary>Gets the <seealso cref="FullSymbolName"/>s of all the used types.</summary>
    public IEnumerable<FullSymbolName> FullSymbolNames => usedTypes.Select(u => u.GetFullSymbolName());

    /// <summary>Gets the namespaces of all the used types.</summary>
    public IEnumerable<string> Namespaces => FullSymbolNames.Select(n => n.FullNamespaceString);

    /// <summary>Registers the types used in the signature of a provided <seealso cref="IMethodSymbol"/>.</summary>
    /// <param name="method">The method symbol whose signature's types to register for usage.</param>
    public void RegisterSignature(IMethodSymbol method)
    {
        var used = method.GetUsedSignatureTypeSymbols();
        var selected = SelectTypeSymbols(used);
        usedTypes.AddRange(selected);
    }

    /// <summary>
    /// Registers the <seealso cref="INamedTypeSymbol"/>s from the used types of a given <seealso cref="ITypeSymbol"/>.
    /// </summary>
    /// <param name="symbol">The <seealso cref="ITypeSymbol"/> whose used types to register.</param>
    /// <remarks>
    /// This method recursively registers all named types used to construct the given type.
    /// For instance, if the type symbol represents an array type, the underlying element type is the named type of interest.
    /// In another example, if the type is a generic type, or a function pointer, its type arguments are used for that matter.
    /// </remarks>
    public void RegisterType(ITypeSymbol symbol)
    {
        var used = GetUsedNamedTypeSymbols(symbol);
        usedTypes.AddRange(used);
    }

    /// <summary>Registers a type that does not have a keyword alias.</summary>
    /// <param name="symbol">The type whose used named types to register, except for the ones aliased by keyword identifiers.</param>
    /// <remarks>
    /// The keyword alias availability is determined from the
    /// <seealso cref="KeywordIdentifierExtensions.HasKeywordIdentifier(ITypeSymbol)"/> method.
    /// </remarks>
    public void RegisterNonKeywordType(ITypeSymbol symbol)
    {
        var keyword = symbol.GetKeywordIdentifierForPredefinedType();
        if (keyword is not null)
        {
            return;
        }

        var used = GetUsedNamedTypeSymbols(symbol)
                  .Where(type => !type.HasKeywordIdentifier());

        usedTypes.AddRange(used);
    }

    // TODO: Abstract those into extensions

    private static INamedTypeSymbol GetUsedNamedTypeSymbol(ITypeSymbol typeSymbol)
    {
        return typeSymbol switch
        {
            INamedTypeSymbol named => named,
            IAliasSymbol alias => alias.Target as INamedTypeSymbol,
            IPointerTypeSymbol pointer => GetUsedNamedTypeSymbol(pointer.PointedAtType),
            IArrayTypeSymbol array => GetUsedNamedTypeSymbol(array.ElementType),

            _ => null,
        };
    }
    private static IEnumerable<INamedTypeSymbol> GetUsedNamedTypeSymbols(ITypeSymbol typeSymbol)
    {
        switch (typeSymbol)
        {
            case IFunctionPointerTypeSymbol functionPointer:
                return SelectTypeSymbols(functionPointer.GetUsedSignatureTypeSymbols());

            case INamedTypeSymbol named:
                return SelectTypeSymbols(GetAllUsedTypeSymbols(named));

            default:
                return new[] { GetUsedNamedTypeSymbol(typeSymbol) };
        }
    }

    private static IEnumerable<INamedTypeSymbol> SelectTypeSymbols(IEnumerable<ITypeSymbol> types)
    {
        return types
              .SelectMany(GetUsedNamedTypeSymbols);
    }

    private static ImmutableArray<ITypeSymbol> GetAllUsedTypeSymbols(INamedTypeSymbol namedTypeSymbol)
    {
        int typeCount = namedTypeSymbol.Arity + 1;
        var immutableBuilder = ImmutableArray.CreateBuilder<ITypeSymbol>(typeCount);

        immutableBuilder.Add(namedTypeSymbol);
        immutableBuilder.AddRange(namedTypeSymbol.TypeArguments);

        return immutableBuilder.ToImmutable();
    }
}
