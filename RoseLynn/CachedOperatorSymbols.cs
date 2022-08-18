#nullable enable

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RoseLynn;

/*
 * TODO: Report a bug in inheritdoc inside a primary constructor based on its inherited record
 *       - Should be done by 19/08/2022 12:43 UTC +03:00
 */

/// <summary>Contains cached information about a type's operators.</summary>
/// <remarks>For advanced querying, <seealso cref="CachedOperatorSymbolQueryingExtensions"/> contains extensions that apply common filters.</remarks>
public class CachedOperatorSymbols
{
    private readonly OperatorSymbolDictionary cachedOperators = new();

    /// <summary>Gets the type that contains the cached operator symbols.</summary>
    public ITypeSymbol Type { get; }

    #region Querying
    /// <summary>Gets all the operator symbols found in the type, in the form of <see cref="CachedOperatorSymbol"/> instances.</summary>
    public IEnumerable<CachedOperatorSymbol> AllOperators => cachedOperators.Values.SelectMany(op => op);

    public IEnumerable<CachedUnaryOperatorSymbol> UnaryOperators => Operators<CachedUnaryOperatorSymbol>();
    public IEnumerable<CachedBinaryOperatorSymbol> BinaryOperators => Operators<CachedBinaryOperatorSymbol>();

    public IEnumerable<CachedImplicitOperatorSymbol> ImplicitConversions => Operators<CachedImplicitOperatorSymbol>();
    public IEnumerable<CachedExplicitOperatorSymbol> ExplicitConversions => Operators<CachedExplicitOperatorSymbol>();

    private IEnumerable<TCachedOperator> Operators<TCachedOperator>()
        where TCachedOperator : CachedOperatorSymbol
    {
        return AllOperators.OfType<TCachedOperator>();
    }

    public IEnumerable<CachedOperatorSymbol> OfKind(OperatorKind kind) => cachedOperators[kind];
    public IEnumerable<CachedOperatorSymbol> OfKind(CompositeOperatorKind kind) => cachedOperators[kind];

    // There seems to be something missing here for sure
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

    private sealed class OperatorSymbolDictionary : Dictionary<CompositeOperatorKind, List<CachedOperatorSymbol>>
    {
        public void Add(IMethodSymbol symbol)
        {
            var parsed = CachedOperatorSymbol.Parse(symbol);
            if (parsed is null)
                return;

            Add(parsed);
        }
        public void Add(CachedOperatorSymbol symbol)
        {
            Add(symbol.CompositeOperatorKind, symbol);
        }

        private void Add(CompositeOperatorKind kind, CachedOperatorSymbol symbol)
        {
            bool existed = TryGetValue(kind, out var list);
            if (!existed)
            {
                list = new();
                this[kind] = list;
            }

            list.Add(symbol);
        }

        public IEnumerable<CachedOperatorSymbol> this[OperatorKind kind]
        {
            get
            {
                CompositeOperatorKind.ForBothCheckingModes(kind, out var a, out var b);
                return this[a].Concat(this[b]);
            }
        }
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

    protected bool EqualsDeclaringType(ITypeSymbol type) => type.Equals(DeclaringType, SymbolEqualityComparer.Default);

    protected ITypeSymbol MethodParameterType(int index) => Method.Parameters[index].Type;

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

    public override bool InvolvesDeclaringType => EqualsDeclaringType(LeftParameterType) || EqualsDeclaringType(RightParameterType);
}

/// <summary>Contains cached information about a conversion operator method symbol.</summary>
/// <inheritdoc/>
public abstract record class CachedConversionOperatorSymbol(OperatorCheckingMode CheckingMode, OperatorKind OperatorKind, IMethodSymbol Method)
    : CachedUnaryOperatorSymbol(CheckingMode, OperatorKind, Method)
{
    public ITypeSymbol From => ParameterType;
    public ITypeSymbol To => Method.ReturnType;

    public sealed override bool InvolvesDeclaringType => EqualsDeclaringType(From) || EqualsDeclaringType(To);

    public ITypeSymbol OtherType => Direction switch
    {
        ConversionOperatorDirection.FromDeclaring => To,
        ConversionOperatorDirection.ToDeclaring => From,
        _ => null!, // Unreachable
    };
    public ConversionOperatorDirection Direction => EqualsDeclaringType(From) switch
    {
        true => ConversionOperatorDirection.FromDeclaring,
        false => ConversionOperatorDirection.ToDeclaring,
    };
}

/// <summary>Contains cached information about an implicit conversion operator method symbol.</summary>
/// <inheritdoc/>
public sealed record class CachedImplicitOperatorSymbol(IMethodSymbol Method)
    : CachedConversionOperatorSymbol(OperatorCheckingMode.Unchecked, OperatorKind.Implicit, Method);

/// <summary>Contains cached information about an explicit conversion operator method symbol.</summary>
/// <inheritdoc/>
public sealed record class CachedExplicitOperatorSymbol(OperatorCheckingMode CheckingMode, IMethodSymbol Method)
    : CachedConversionOperatorSymbol(CheckingMode, OperatorKind.Explicit, Method);

/// <summary>Denotes the direction of the conversion operator.</summary>
public enum ConversionOperatorDirection
{
    /// <summary>Denotes that the conversion operator converts an instance of the declaring type into a foreign one.</summary>
    FromDeclaring,
    /// <summary>Denotes that the conversion operator converts an instance of a foreign type into the declaring one.</summary>
    ToDeclaring,
}

public record struct CompositeOperatorKind(OperatorCheckingMode CheckingMode, OperatorKind Kind)
{
    public static void ForBothCheckingModes(OperatorKind kind, out CompositeOperatorKind uncheckedMode, out CompositeOperatorKind checkedMode)
    {
        uncheckedMode = new(OperatorCheckingMode.Unchecked, kind);
        checkedMode = new(OperatorCheckingMode.Checked, kind);
    }
}

/// <summary>Denotes the operator method kind of the operator method symbol.</summary>
public enum OperatorMethodKind
{
    UserDefined,
    Builtin,
    Conversion,
}

public static class OperatorMethodKindExtensions
{
    public static OperatorMethodKind GetOperatorMethodKind(this MethodKind methodKind) => methodKind switch
    {
        MethodKind.UserDefinedOperator => OperatorMethodKind.UserDefined,
        MethodKind.BuiltinOperator => OperatorMethodKind.Builtin,
        MethodKind.Conversion => OperatorMethodKind.Conversion,

        _ => throw new InvalidEnumArgumentException("The given MethodKind does not reflect an operator method kind.")
    };
    public static MethodKind GetMethodKind(this OperatorMethodKind operatorMethodKind) => operatorMethodKind switch
    {
        OperatorMethodKind.UserDefined => MethodKind.UserDefinedOperator,
        OperatorMethodKind.Builtin => MethodKind.BuiltinOperator,
        OperatorMethodKind.Conversion => MethodKind.Conversion,

        _ => throw new InvalidEnumArgumentException("The given value is undefined.")
    };
}

public enum OperatorCheckingMode
{
    Unchecked,
    Checked,

    Undefined = Unchecked,
}
