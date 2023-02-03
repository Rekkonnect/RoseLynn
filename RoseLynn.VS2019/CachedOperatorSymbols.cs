#nullable enable

using Garyon.DataStructures;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn;

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

    public IEnumerable<CachedUnaryOperatorSymbol> UnaryOperators => Operators<CachedUnaryOperatorSymbol>();
    public IEnumerable<CachedBinaryOperatorSymbol> BinaryOperators => Operators<CachedBinaryOperatorSymbol>();

    public IEnumerable<CachedImplicitOperatorSymbol> ImplicitConversions => Operators<CachedImplicitOperatorSymbol>();
    public IEnumerable<CachedExplicitOperatorSymbol> ExplicitConversions => Operators<CachedExplicitOperatorSymbol>();

    public IEnumerable<CachedOperatorSymbol> UncheckedOperators => OfCheckingMode(OperatorCheckingMode.Unchecked);
    public IEnumerable<CachedOperatorSymbol> CheckedOperators => OfCheckingMode(OperatorCheckingMode.Checked);

    private IEnumerable<TCachedOperator> Operators<TCachedOperator>()
        where TCachedOperator : CachedOperatorSymbol
    {
        return AllOperators.OfType<TCachedOperator>();
    }

    public IEnumerable<CachedOperatorSymbol> OfKind(CompositeOperatorKind kind) => cachedOperators[kind];
    public IEnumerable<CachedOperatorSymbol> OfKind(OperatorKind kind) => cachedOperators[kind];
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

    public static CachedOperatorSymbols ForType(ITypeSymbol type)
    {
        var result = new CachedOperatorSymbols(type);
        foreach (var method in type.GetMembers<IMethodSymbol>())
            result.Add(method);

        return result;
    }
    #endregion

    private sealed class OperatorSymbolDictionary
    {
        // Eminem would be proud
        private readonly List<CachedOperatorSymbol> operatorList = new();
        private readonly FlexibleInitializableValueDictionary<CompositeOperatorKind, List<CachedOperatorSymbol>> compositelyMapped = new();
        private readonly FlexibleInitializableValueDictionary<OperatorKind, List<CachedOperatorSymbol>> operatorKindMapped = new();
        private readonly FlexibleInitializableValueDictionary<OperatorCheckingMode, List<CachedOperatorSymbol>> checkingModeMapped = new();

        public IEnumerable<CachedOperatorSymbol> All => operatorList;

        public void Add(IMethodSymbol symbol)
        {
            var parsed = CachedOperatorSymbol.Parse(symbol);
            if (parsed is null)
                return;

            Add(parsed);
        }
        public void Add(CachedOperatorSymbol symbol)
        {
            operatorList.Add(symbol);
            compositelyMapped[symbol.CompositeOperatorKind].Add(symbol);
            operatorKindMapped[symbol.OperatorKind].Add(symbol);
            checkingModeMapped[symbol.CheckingMode].Add(symbol);
        }

        public IEnumerable<CachedOperatorSymbol> this[CompositeOperatorKind kind]
            => compositelyMapped[kind];

        public IEnumerable<CachedOperatorSymbol> this[OperatorKind kind]
            => operatorKindMapped[kind];

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
    public CompositeOperatorKind CompositeOperatorKind => new(CheckingMode, OperatorKind);

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
    public ITypeSymbol ParameterType => MethodParameterType(0);

    public override bool InvolvesDeclaringType => EqualsDeclaringType(ParameterType);
}

/// <summary>Contains cached information about a binary operator method symbol.</summary>
/// <inheritdoc/>
public sealed record class CachedBinaryOperatorSymbol(OperatorCheckingMode CheckingMode, OperatorKind OperatorKind, IMethodSymbol Method)
    : CachedOperatorSymbol(CheckingMode, OperatorKind, Method)
{
    public ITypeSymbol LeftParameterType => MethodParameterType(0);
    public ITypeSymbol RightParameterType => MethodParameterType(1);

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
