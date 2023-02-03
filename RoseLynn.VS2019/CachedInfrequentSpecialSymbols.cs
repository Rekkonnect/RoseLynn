using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace RoseLynn;

/// <summary>Contains cached infrequent special symbols for analyzed <seealso cref="ISymbol"/> instances.</summary>
public class CachedInfrequentSpecialSymbols
{
    /// <summary>The shared singleton instance containing <seealso cref="InfrequentSpecialSymbolCache"/> instances.</summary>
    public static readonly CachedInfrequentSpecialSymbols Instance = new();

    private readonly Dictionary<ISymbol, InfrequentSpecialSymbolCache> cachedSymbols = new(SymbolEqualityComparer.Default);

    private CachedInfrequentSpecialSymbols() { }

    public InfrequentSpecialSymbolCache.IStronglyTyped<TSymbol> GetStronglyTyped<TSymbol>(TSymbol symbol)
        where TSymbol : class, ISymbol
    {
        return Get<InfrequentSpecialSymbolCache.IStronglyTyped<TSymbol>>(symbol);
    }

    public InfrequentSpecialSymbolCache this[ISymbol symbol] => Get(symbol);
    public InfrequentSpecialSymbolCache.TypeSymbol this[ITypeSymbol symbol] => Get<InfrequentSpecialSymbolCache.TypeSymbol>(symbol);
    public InfrequentSpecialSymbolCache.NamedTypeSymbol this[INamedTypeSymbol symbol] => Get<InfrequentSpecialSymbolCache.NamedTypeSymbol>(symbol);

    private TCache Get<TCache>(ISymbol symbol)
        where TCache : class, IInfrequentSpecialSymbolCache
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
public interface IInfrequentSpecialSymbolCache
{
    /// <summary>Gets the <seealso cref="ISymbol"/> whose special symbol cache is contained.</summary>
    public ISymbol Symbol { get; }
}

/// <summary>Contains cached information about infrequent special members of <seealso cref="ISymbol"/> instances.</summary>
/// <remarks>All special members are lazily evaluated and retrieved.</remarks>
public abstract class InfrequentSpecialSymbolCache : IInfrequentSpecialSymbolCache
{
    /// <summary>Gets the <seealso cref="ISymbol"/> whose special symbol cache is contained.</summary>
    public ISymbol Symbol { get; }

    /// <summary>Initializes a new <seealso cref="InfrequentSpecialSymbolCache"/> instance that will contain cache about the given symbol.</summary>
    /// <param name="symbol">The symbol whose infrequent special symbols are to be cached.</param>
    protected InfrequentSpecialSymbolCache(ISymbol symbol)
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
            ITypeSymbol type => new TypeSymbol(type),

            _ => new StronglyTyped<TSymbol>(symbol),
        };
    }

    /// <summary>Contains cached information about infrequent special members of instances a strongly-typed type deriving from the <seealso cref="ISymbol"/> interface.</summary>
    /// <remarks>All special members are lazily evaluated and retrieved.</remarks>
    public interface IStronglyTyped<out TSymbol> : IInfrequentSpecialSymbolCache
        where TSymbol : class, ISymbol
    {
        /// <summary>Gets the <typeparamref name="TSymbol"/> whose special symbol cache is contained.</summary>
        public new TSymbol Symbol { get; }
    }

    /// <summary>Contains cached information about infrequent special members of instances a strongly-typed type deriving from the <seealso cref="ISymbol"/> interface.</summary>
    /// <remarks>All special members are lazily evaluated and retrieved.</remarks>
    public class StronglyTyped<TSymbol> : InfrequentSpecialSymbolCache, IStronglyTyped<TSymbol>
        where TSymbol : class, ISymbol
    {
        /// <summary>Gets the <seealso cref="ISymbol"/> whose special symbol cache is contained.</summary>
        public new TSymbol Symbol => (base.Symbol as TSymbol)!;

        /// <summary>Initializes a new <seealso cref="StronglyTyped{TSymbol}"/> instance that will contain cache about the given symbol.</summary>
        /// <param name="symbol">The symbol whose infrequent special symbols are to be cached.</param>
        public StronglyTyped(TSymbol symbol)
            : base(symbol) { }
    }

    /// <summary>Contains cached information about infrequent special members of <seealso cref="INamedTypeSymbol"/> instances.</summary>
    /// <remarks>All special members are lazily evaluated and retrieved.</remarks>
    public class TypeSymbol : StronglyTyped<ITypeSymbol>
    {
        private readonly Lazy<CachedOperatorSymbols> operatorSymbolsLazy;

        /// <summary>Gets the <seealso cref="CachedOperatorSymbols"/> instance reflecting all the operator symobls defined in the type.</summary>
        public CachedOperatorSymbols OperatorSymbols => operatorSymbolsLazy.Value;

        /// <summary>Initializes a new <seealso cref="TypeSymbol"/> instance that will contain cache about the given symbol.</summary>
        /// <param name="symbol">The symbol whose infrequent special symbols are to be cached.</param>
        public TypeSymbol(ITypeSymbol symbol)
            : base(symbol)
        {
            operatorSymbolsLazy = new(GetOperatorSymbols);
        }

        private CachedOperatorSymbols GetOperatorSymbols()
        {
            return CachedOperatorSymbols.ForType(Symbol);
        }
    }

    /// <summary>Contains cached information about infrequent special members of <seealso cref="INamedTypeSymbol"/> instances.</summary>
    /// <remarks>All special members are lazily evaluated and retrieved.</remarks>
    public sealed class NamedTypeSymbol : TypeSymbol, IStronglyTyped<INamedTypeSymbol>
    {
        private readonly Lazy<IMethodSymbol?> destructorLazy;
        private readonly Lazy<ImmutableArray<IMethodSymbol>> extensionMethodsLazy;
        private readonly Lazy<ImmutableArray<IFieldSymbol>> constantFieldsLazy;

        /// <summary>Gets the <seealso cref="INamedTypeSymbol"/> whose special symbol cache is contained.</summary>
        public new INamedTypeSymbol Symbol => (base.Symbol as INamedTypeSymbol)!;

        /// <summary>The <seealso cref="IMethodSymbol"/> representing the destructor of the <seealso cref="StronglyTyped{T}.Symbol"/>, or <see langword="null"/> if it doesn't contain such.</summary>
        public IMethodSymbol? Destructor => destructorLazy.Value;

        /// <summary>An array of <seealso cref="IMethodSymbol"/> instances representing the extension methods contained in the <seealso cref="StronglyTyped{T}.Symbol"/>.</summary>
        public ImmutableArray<IMethodSymbol> ExtensionMethods => extensionMethodsLazy.Value;

        /// <summary>An array of <seealso cref="IFieldSymbol"/> instances representing the constant fields contained in the <seealso cref="StronglyTyped{T}.Symbol"/>.</summary>
        /// <remarks>Enum members also count as constant fields.</remarks>
        public ImmutableArray<IFieldSymbol> ConstantFields => constantFieldsLazy.Value;

        /// <summary>Initializes a new <seealso cref="NamedTypeSymbol"/> instance that will contain cache about the given symbol.</summary>
        /// <param name="symbol">The symbol whose infrequent special symbols are to be cached.</param>
        public NamedTypeSymbol(INamedTypeSymbol symbol)
            : base(symbol)
        {
            destructorLazy = new(GetDestructor);
            extensionMethodsLazy = new(GetExtensionMethods);
            constantFieldsLazy = new(GetConstantFields);
        }

        private IMethodSymbol? GetDestructor()
        {
            return Symbol.GetMembers<IMethodSymbol>().FirstOrDefault(member => member is { MethodKind: MethodKind.Destructor });
        }
        private ImmutableArray<IMethodSymbol> GetExtensionMethods()
        {
            if (!Symbol.MightContainExtensionMethods)
                return ImmutableArray<IMethodSymbol>.Empty;

            return Symbol.GetMembers<IMethodSymbol>().Where(method => method.IsExtensionMethod).ToImmutableArray();
        }
        private ImmutableArray<IFieldSymbol> GetConstantFields()
        {
            return Symbol.GetMembers<IFieldSymbol>().Where(field => field.IsConst).ToImmutableArray();
        }
    }
}