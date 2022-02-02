using Microsoft.CodeAnalysis;
using RoseLynn.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace RoseLynn;

/// <summary>Contains cached infrequent special symbols for analyzed <seealso cref="ISymbol"/> instances.</summary>
public class CachedInfrequentSpecialSymbols
{
    /// <summary>The shared singleton instance containing <seealso cref="InfrequentSpecialSymbolCache"/> instances.</summary>
    public static readonly CachedInfrequentSpecialSymbols Instance = new();

#pragma warning disable RS1024 // Compare symbols correctly
    private readonly Dictionary<ISymbol, InfrequentSpecialSymbolCache> cachedSymbols = new(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly

    private CachedInfrequentSpecialSymbols() { }

    public InfrequentSpecialSymbolCache this[ISymbol symbol] => Get(symbol);
    public InfrequentSpecialSymbolCache.NamedTypeSymbol this[INamedTypeSymbol symbol] => Get<InfrequentSpecialSymbolCache.NamedTypeSymbol>(symbol);

    private TCache Get<TCache>(ISymbol symbol)
        where TCache : InfrequentSpecialSymbolCache
    {
        return (Get(symbol) as TCache)!;
    }
    private InfrequentSpecialSymbolCache Get(ISymbol symbol)
    {
        var cache = cachedSymbols.ValueOrDefault(symbol);
        if (cache is null)
        {
            cache = InfrequentSpecialSymbolCache.CreateNew(symbol);
            cachedSymbols.Add(symbol, cache);
        }

        return cache;
    }
}

/// <summary>Contains cached information about infrequent special members of <seealso cref="ISymbol"/> instances.</summary>
/// <remarks>All special members are lazily evaluated and retrieved.</remarks>
public abstract class InfrequentSpecialSymbolCache
{
    /// <summary>Gets the <seealso cref="ISymbol"/> whose special symbol cache is contained.</summary>
    public ISymbol Symbol { get; }

    public InfrequentSpecialSymbolCache(ISymbol symbol)
    {
        Symbol = symbol;
    }

    /// <summary>Creates a new <seealso cref="InfrequentSpecialSymbolCache"/> instance for the given <seealso cref="ISymbol"/> type.</summary>
    /// <typeparam name="TSymbol">The <seealso cref="ISymbol"/> type of the given symbol.</typeparam>
    /// <param name="symbol">The symbol whose special symbol cache to get.</param>
    /// <returns>A new <seealso cref="InfrequentSpecialSymbolCache"/> specialized for the given <seealso cref="ISymbol"/> type.</returns>
    public static InfrequentSpecialSymbolCache CreateNew<TSymbol>(TSymbol symbol)
        where TSymbol : class, ISymbol
    {
        return symbol switch
        {
            INamedTypeSymbol named => new NamedTypeSymbol(named),
            _ => new StronglyTyped<TSymbol>(symbol),
        };
    }

    public class StronglyTyped<TSymbol> : InfrequentSpecialSymbolCache
        where TSymbol : class, ISymbol
    {
        /// <summary>Gets the <seealso cref="ISymbol"/> whose special symbol cache is contained.</summary>
        public new TSymbol Symbol => (base.Symbol as TSymbol)!;

        public StronglyTyped(TSymbol symbol)
            : base(symbol) { }
    }

    public sealed class NamedTypeSymbol : StronglyTyped<INamedTypeSymbol>
    {
        private readonly Lazy<IMethodSymbol?> destructorLazy;

        /// <summary>The <seealso cref="IMethodSymbol"/> representing the destructor of the <seealso cref="Symbol"/>, or <see langword="null"/> if it doesn't contain such.</summary>
        public IMethodSymbol? Destructor => destructorLazy.Value;

        public NamedTypeSymbol(INamedTypeSymbol symbol)
            : base(symbol)
        {
            destructorLazy = new(GetDestructor);
        }

        private IMethodSymbol? GetDestructor()
        {
            return Symbol.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(member => member is { MethodKind: MethodKind.Destructor });
        }
    }
}