
using Garyon.DataStructures;
using Microsoft.CodeAnalysis;

namespace RoseLynn.Core;

/// <summary>Contains cached information about a type's operators.</summary>
/// <remarks>For advanced querying, <seealso cref="CachedOperatorSymbolQuerying"/> contains extensions that apply common filters.</remarks>
public class CachedOperatorSymbols
{
    private readonly OperatorSymbolDictionary cachedOperators = new();

    /// <summary>Gets the type that contains the cached operator symbols.</summary>
    public ITypeSymbol Type { get; }

    #region Querying
    /// <summary>Gets all the operator symbols found in the type, in the form of <see cref="CachedOperatorSymbol"/> instances.</summary>
    public IEnumerable<CachedOperatorSymbol> AllOperators => cachedOperators.All;

    /// <summary>Gets all unary operator symbols found in the type.</summary>
    public IEnumerable<CachedUnaryOperatorSymbol> UnaryOperators => Operators<CachedUnaryOperatorSymbol>();
    /// <summary>Gets all binary operator symbols found in the type.</summary>
    public IEnumerable<CachedBinaryOperatorSymbol> BinaryOperators => Operators<CachedBinaryOperatorSymbol>();

    /// <summary>Gets all implicit conversion operator symbols found in the type.</summary>
    public IEnumerable<CachedImplicitOperatorSymbol> ImplicitConversions => Operators<CachedImplicitOperatorSymbol>();
    /// <summary>Gets all explicit conversion operator symbols found in the type.</summary>
    public IEnumerable<CachedExplicitOperatorSymbol> ExplicitConversions => Operators<CachedExplicitOperatorSymbol>();

    /// <summary>Gets all unchecked operator symbols found in the type.</summary>
    public IEnumerable<CachedOperatorSymbol> UncheckedOperators => OfCheckingMode(OperatorCheckingMode.Unchecked);
    /// <summary>Gets all checked operator symbols found in the type.</summary>
    public IEnumerable<CachedOperatorSymbol> CheckedOperators => OfCheckingMode(OperatorCheckingMode.Checked);

    private IEnumerable<TCachedOperator> Operators<TCachedOperator>()
        where TCachedOperator : CachedOperatorSymbol
    {
        return AllOperators.OfType<TCachedOperator>();
    }

    /// <summary>Gets all operator symbols matching the given composite operator kind.</summary>
    public IEnumerable<CachedOperatorSymbol> OfKind(CompositeOperatorKind kind) => cachedOperators[kind];
    /// <summary>Gets all operator symbols matching the given operator kind.</summary>
    public IEnumerable<CachedOperatorSymbol> OfKind(OperatorKind kind) => cachedOperators[kind];
    /// <summary>Gets all operator symbols matching the given checking mode.</summary>
    public IEnumerable<CachedOperatorSymbol> OfCheckingMode(OperatorCheckingMode mode) => cachedOperators[mode];
    #endregion

    #region Factory
    private CachedOperatorSymbols(ITypeSymbol type)
    {
        Type = type;
    }

    private void Add(IMethodSymbol method)
    {
        cachedOperators.Add(method);
    }

    /// <summary>Creates a <seealso cref="CachedOperatorSymbols"/> instance for the given type by scanning all its members for operator methods.</summary>
    /// <param name="type">The type whose operators to cache.</param>
    /// <returns>A new <seealso cref="CachedOperatorSymbols"/> instance populated with the operators found in <paramref name="type"/>.</returns>
    public static CachedOperatorSymbols ForType(ITypeSymbol type)
    {
        var result = new CachedOperatorSymbols(type);
        foreach (var method in type.GetMembers<IMethodSymbol>())
            result.Add(method);

        return result;
    }
    #endregion

    /// <summary>Indexes parsed operator symbols by composite kind, operator kind, and checking mode.</summary>
    private sealed class OperatorSymbolDictionary
    {
        // Eminem would be proud
        private readonly List<CachedOperatorSymbol> operatorList = new();
        private readonly FlexInitDictionary<CompositeOperatorKind, List<CachedOperatorSymbol>> compositelyMapped = new();
        private readonly FlexInitDictionary<OperatorKind, List<CachedOperatorSymbol>> operatorKindMapped = new();
        private readonly FlexInitDictionary<OperatorCheckingMode, List<CachedOperatorSymbol>> checkingModeMapped = new();

        /// <summary>Gets all cached operator symbols in insertion order.</summary>
        public IEnumerable<CachedOperatorSymbol> All => operatorList;

        /// <summary>Adds an operator symbol parsed from a Roslyn method symbol.</summary>
        /// <param name="symbol">The method symbol to parse and add.</param>
        public void Add(IMethodSymbol symbol)
        {
            var parsed = CachedOperatorSymbol.Parse(symbol);
            if (parsed is null)
                return;

            Add(parsed);
        }
        /// <summary>Adds a cached operator symbol to all index structures.</summary>
        /// <param name="symbol">The cached operator symbol to add.</param>
        public void Add(CachedOperatorSymbol symbol)
        {
            operatorList.Add(symbol);
            compositelyMapped[symbol.CompositeOperatorKind].Add(symbol);
            operatorKindMapped[symbol.OperatorKind].Add(symbol);
            checkingModeMapped[symbol.CheckingMode].Add(symbol);
        }

        /// <summary>Gets cached operators by composite operator kind.</summary>
        public IEnumerable<CachedOperatorSymbol> this[CompositeOperatorKind kind]
            => compositelyMapped[kind];

        /// <summary>Gets cached operators by operator kind.</summary>
        public IEnumerable<CachedOperatorSymbol> this[OperatorKind kind]
            => operatorKindMapped[kind];

        /// <summary>Gets cached operators by checking mode.</summary>
        public IEnumerable<CachedOperatorSymbol> this[OperatorCheckingMode mode]
            => checkingModeMapped[mode];
    }
}

/// <summary>Contains cached information about an operator method symbol.</summary>
/// <param name="CheckingMode">The <seealso cref="OperatorCheckingMode"/> of the operator.</param>
/// <param name="OperatorKind">The <seealso cref="RoseLynn.OperatorKind"/> of the operator.</param>
/// <param name="Method">The <seealso cref="IMethodSymbol"/> representing the operator.</param>
public abstract record class CachedOperatorSymbol(OperatorCheckingMode CheckingMode, OperatorKind OperatorKind, IMethodSymbol Method)
{
    /// <summary>Gets the composite operator kind combining both the <seealso cref="OperatorKind"/> and <seealso cref="CheckingMode"/>.</summary>
    public CompositeOperatorKind CompositeOperatorKind => new(CheckingMode, OperatorKind);

    /// <summary>Gets the high-level <seealso cref="RoseLynn.OperatorMethodKind"/> category of this operator.</summary>
    public OperatorMethodKind OperatorMethodKind => Method.MethodKind.GetOperatorMethodKind();

    /// <summary>Gets the <seealso cref="ITypeSymbol"/> that declares this operator.</summary>
    public ITypeSymbol DeclaringType => Method.ContainingType;

    /// <summary>Determines whether the operator involves its declaring type.</summary>
    /// <remarks>
    /// Per the current C# and VB restrictions, this always evaluates to <see langword="true"/>,
    /// as an operator declaration must involve its declaring type. This method can be applied to
    /// certain cases where an operator is declared using other media (i.e. IL), causing it to be
    /// loaded in the metadata.
    /// <br/>
    /// Again, there should be very specific circumstances where evaluating this property can be
    /// proven useful; most of the times it should be assumed that it evaluates to <see langword="true"/>.
    /// </remarks>
    public abstract bool InvolvesDeclaringType { get; }

    private protected bool EqualsDeclaringType(ITypeSymbol type) => type.Equals(DeclaringType, SymbolEqualityComparer.Default);

    private protected ITypeSymbol MethodParameterType(int index) => Method.Parameters[index].Type;

    private static bool IsOperatorMethod(IMethodSymbol method)
    {
        return method.MethodKind
            is MethodKind.UserDefinedOperator
            or MethodKind.BuiltinOperator
            or MethodKind.Conversion;
    }

    /// <summary>Parses a <seealso cref="CachedOperatorSymbol"/> from the given method symbol, or <see langword="null"/> if the method is not an operator.</summary>
    /// <param name="method">The method symbol to parse.</param>
    /// <returns>A <seealso cref="CachedOperatorSymbol"/> subtype instance, or <see langword="null"/>.</returns>
    public static CachedOperatorSymbol? Parse(IMethodSymbol method)
    {
        if (!IsOperatorMethod(method))
            return null;

        var operatorKind = OperatorKindFacts.MapNameToKind(method.Name, out var checkingMode);

        // I don't like the switch expression here
        switch (operatorKind)
        {
            case OperatorKind.Implicit:
                return new CachedImplicitOperatorSymbol(method);
            case OperatorKind.Explicit:
                return new CachedExplicitOperatorSymbol(checkingMode, method);
        }

        if (OperatorKindFacts.IsUnary(operatorKind))
            return new CachedUnaryOperatorSymbol(checkingMode, operatorKind, method);

        return new CachedBinaryOperatorSymbol(checkingMode, operatorKind, method);
    }
}

/// <summary>Contains cached information about a unary operator method symbol.</summary>
/// <inheritdoc/>
public record class CachedUnaryOperatorSymbol(OperatorCheckingMode CheckingMode, OperatorKind OperatorKind, IMethodSymbol Method)
    : CachedOperatorSymbol(CheckingMode, OperatorKind, Method)
{
    /// <summary>Gets the type of the single operand of this unary operator.</summary>
    /// <summary>Gets the type of the single operand of this unary operator.</summary>
    public ITypeSymbol ParameterType => MethodParameterType(0);

    /// <inheritdoc/>
    public override bool InvolvesDeclaringType => EqualsDeclaringType(ParameterType);
}

/// <summary>Contains cached information about a binary operator method symbol.</summary>
/// <inheritdoc/>
public sealed record class CachedBinaryOperatorSymbol(OperatorCheckingMode CheckingMode, OperatorKind OperatorKind, IMethodSymbol Method)
    : CachedOperatorSymbol(CheckingMode, OperatorKind, Method)
{
    /// <summary>Gets the type of the left operand of this binary operator.</summary>
    public ITypeSymbol LeftParameterType => MethodParameterType(0);
    /// <summary>Gets the type of the right operand of this binary operator.</summary>
    public ITypeSymbol RightParameterType => MethodParameterType(1);

    /// <inheritdoc/>
    public override bool InvolvesDeclaringType => EqualsDeclaringType(LeftParameterType)
                                               || EqualsDeclaringType(RightParameterType);
}

/// <summary>Contains cached information about a conversion operator method symbol.</summary>
/// <inheritdoc/>
public abstract record class CachedConversionOperatorSymbol(OperatorCheckingMode CheckingMode, OperatorKind OperatorKind, IMethodSymbol Method)
    : CachedUnaryOperatorSymbol(CheckingMode, OperatorKind, Method)
{
    /// <summary>
    /// The type of the incoming value that is being converted.
    /// </summary>
    public ITypeSymbol From => ParameterType;
    /// <summary>
    /// The type of the returning value, which is the result of
    /// the conversion.
    /// </summary>
    public ITypeSymbol To => Method.ReturnType;

    /// <inheritdoc/>
    public sealed override bool InvolvesDeclaringType => EqualsDeclaringType(From)
                                                      || EqualsDeclaringType(To);

    /// <summary>
    /// Gets the foreign type of the conversion, which is the type not
    /// equal to the type declaring the operator. In other words, if the
    /// direction is <seealso cref="ConversionOperatorDirection.FromDeclaring"/>,
    /// <seealso cref="To"/> is returned, and vice versa.
    /// </summary>
    /// <remarks>
    /// <seealso cref="InvolvesDeclaringType"/> is not evaluated and is always
    /// assumed to be <see langword="true"/>.
    /// </remarks>
    public ITypeSymbol OtherType => Direction switch
    {
        ConversionOperatorDirection.FromDeclaring => To,
        ConversionOperatorDirection.ToDeclaring => From,
        _ => null!, // Unreachable
    };

    /// <summary>
    /// Gets the direction of the operator, evaluating the <seealso cref="InvolvesDeclaringType"/>
    /// property's result.
    /// </summary>
    /// <remarks>
    /// If the <seealso cref="InvolvesDeclaringType"/> is <see langword="false"/>,
    /// <seealso cref="ConversionOperatorDirection.Unrelated"/> is returned.
    /// </remarks>
    public ConversionOperatorDirection CheckedDirection
    {
        get
        {
            if (!InvolvesDeclaringType)
                return ConversionOperatorDirection.Unrelated;

            return Direction;
        }
    }

    /// <summary>
    /// Gets the direction of the operator.
    /// </summary>
    /// <remarks>
    /// The <seealso cref="InvolvesDeclaringType"/> is assumed to always be <see langword="true"/>.
    /// Use <seealso cref="CheckedDirection"/> to evaluate that property's value.
    /// </remarks>
    public ConversionOperatorDirection Direction
    {
        get
        {
            return EqualsDeclaringType(From) switch
            {
                true => ConversionOperatorDirection.FromDeclaring,
                false => ConversionOperatorDirection.ToDeclaring,
            };
        }
    }
}

/// <summary>Contains cached information about an implicit conversion operator method symbol.</summary>
/// <inheritdoc/>
public sealed record class CachedImplicitOperatorSymbol(IMethodSymbol Method)
    : CachedConversionOperatorSymbol(OperatorCheckingMode.Unchecked, OperatorKind.Implicit, Method);

/// <summary>Contains cached information about an explicit conversion operator method symbol.</summary>
/// <inheritdoc/>
public sealed record class CachedExplicitOperatorSymbol(OperatorCheckingMode CheckingMode, IMethodSymbol Method)
    : CachedConversionOperatorSymbol(CheckingMode, OperatorKind.Explicit, Method);
