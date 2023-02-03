#nullable enable

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoseLynn;

// Querying is abstracted over here to avoid bloating CachedOperatorSymbols with unnecessary semantics
public static class CachedOperatorSymbolQuerying
{
    /// <summary>Gets the binary operators that receive arguments of the specified types. A <see langword="null"/> type is treated as a wildcard.</summary>
    /// <param name="symbols">The symbols whose type argument types to match.</param>
    /// <param name="left">The type of the left argument of the binary operator. <see langword="null"/> will ignore filtering by the left argument type.</param>
    /// <param name="right">The type of the right argument of the binary operator. <see langword="null"/> will ignore filtering by the right argument type.</param>
    /// <returns>A collection of <seealso cref="CachedBinaryOperatorSymbol"/> instances that match the argument types.</returns>
    public static IEnumerable<CachedBinaryOperatorSymbol> MatchingArgumentTypes(this IEnumerable<CachedBinaryOperatorSymbol> symbols, ITypeSymbol? left, ITypeSymbol? right)
    {
        return (left, right) switch
        {
            (null, null) => symbols,
            (_, null) => symbols.Where(MatchesLeft),
            (null, _) => symbols.Where(MatchesRight),
            _ => symbols.Where(MatchesBoth),
        };

        bool MatchesLeft(CachedBinaryOperatorSymbol symbol) => symbol.LeftParameterType.Equals(left, SymbolEqualityComparer.Default);
        bool MatchesRight(CachedBinaryOperatorSymbol symbol) => symbol.RightParameterType.Equals(right, SymbolEqualityComparer.Default);
        bool MatchesBoth(CachedBinaryOperatorSymbol symbol) => MatchesLeft(symbol) && MatchesRight(symbol);
    }
    /// <summary>Gets the unary operators that receive an argument of the specified type. A <see langword="null"/> type is treated as a wildcard.</summary>
    /// <param name="operators">The operators whose type argument type to match.</param>
    /// <param name="type">The type of the argument of the unary operator. <see langword="null"/> will ignore filtering by the argument type.</param>
    /// <returns>A collection of <seealso cref="CachedUnaryOperatorSymbol"/> instances that match the argument type.</returns>
    /// <remarks>Avoid using this method on <seealso cref="CachedConversionOperatorSymbol"/> instances.</remarks>
    public static IEnumerable<CachedUnaryOperatorSymbol> MatchingArgumentType(this IEnumerable<CachedUnaryOperatorSymbol> operators, ITypeSymbol? type)
    {
        if (type is null)
            return operators;

        return operators.Where(Matches);

        bool Matches(CachedUnaryOperatorSymbol symbol) => symbol.ParameterType.Equals(type, SymbolEqualityComparer.Default);
    }

    public static IEnumerable<TOperatorSymbol> Unchecked<TOperatorSymbol>(this IEnumerable<TOperatorSymbol> operators)
        where TOperatorSymbol : CachedOperatorSymbol
    {
        return operators.Where(Matches);

        static bool Matches(CachedOperatorSymbol symbol) => symbol.CheckingMode is OperatorCheckingMode.Unchecked;
    }
    public static IEnumerable<TOperatorSymbol> Checked<TOperatorSymbol>(this IEnumerable<TOperatorSymbol> operators)
        where TOperatorSymbol : CachedOperatorSymbol
    {
        return operators.Where(Matches);

        static bool Matches(CachedOperatorSymbol symbol) => symbol.CheckingMode is OperatorCheckingMode.Checked;
    }

    /// <summary>Gets the operators that match the specified operator kind.</summary>
    /// <param name="operators">The operators whose operator kind to match.</param>
    /// <param name="kind">The operator kind to match.</param>
    /// <returns>A collection of <typeparamref name="TOperatorSymbol"/> instances that match the given operator kind.</returns>
    /// <remarks>
    /// If possible, prefer the <seealso cref="CachedOperatorSymbols.OfKind(OperatorKind)"/> method,
    /// which should offer better performance in most scenarios.
    /// </remarks>
    public static IEnumerable<TOperatorSymbol> OfKind<TOperatorSymbol>(this IEnumerable<TOperatorSymbol> operators, OperatorKind kind)
        where TOperatorSymbol : CachedOperatorSymbol
    {
        return operators.Where(Matches);

        bool Matches(CachedOperatorSymbol symbol) => symbol.OperatorKind == kind;
    }

    /// <summary>Gets all the conversion operators that convert from the declaring type into a foreign type.</summary>
    /// <returns>
    /// A collection of <typeparamref name="TOperatorSymbol"/> instances whose
    /// <seealso cref="CachedConversionOperatorSymbol.Direction"/> is <seealso cref="ConversionOperatorDirection.FromDeclaring"/>.
    /// </returns>
    public static IEnumerable<TOperatorSymbol> FromDeclaringType<TOperatorSymbol>(this IEnumerable<TOperatorSymbol> operators)
        where TOperatorSymbol : CachedConversionOperatorSymbol
    {
        return operators.Where(Matches);

        static bool Matches(CachedConversionOperatorSymbol symbol) => symbol.Direction is ConversionOperatorDirection.FromDeclaring;
    }
    /// <summary>Gets all the conversion operators that convert from a foreign type into the declaring type.</summary>
    /// <returns>
    /// A collection of <typeparamref name="TOperatorSymbol"/> instances whose
    /// <seealso cref="CachedConversionOperatorSymbol.Direction"/> is <seealso cref="ConversionOperatorDirection.ToDeclaring"/>.
    /// </returns>
    public static IEnumerable<TOperatorSymbol> ToDeclaringType<TOperatorSymbol>(this IEnumerable<TOperatorSymbol> operators)
        where TOperatorSymbol : CachedConversionOperatorSymbol
    {
        return operators.Where(Matches);

        static bool Matches(CachedConversionOperatorSymbol symbol) => symbol.Direction is ConversionOperatorDirection.ToDeclaring;
    }

    /// <summary>Gets all the conversion operators that convert from the specified foreign type into the declaring type.</summary>
    /// <returns>
    /// A collection of <typeparamref name="TOperatorSymbol"/> instances whose
    /// <seealso cref="CachedConversionOperatorSymbol.Direction"/> is <seealso cref="ConversionOperatorDirection.ToDeclaring"/>
    /// and the <seealso cref="CachedConversionOperatorSymbol.From"/> type equals the given type.
    /// </returns>
    /// <remarks>This implicitly calls <seealso cref="ToDeclaringType{TOperatorSymbol}(IEnumerable{TOperatorSymbol})"/>.</remarks>
    public static IEnumerable<TOperatorSymbol> FromForeignType<TOperatorSymbol>(this IEnumerable<TOperatorSymbol> operators, ITypeSymbol type)
        where TOperatorSymbol : CachedConversionOperatorSymbol
    {
        return operators.ToDeclaringType().Where(Matches);

        bool Matches(CachedConversionOperatorSymbol symbol) => symbol.From.Equals(type, SymbolEqualityComparer.Default);
    }
    /// <summary>Gets all the conversion operators that convert from the declaring type into the specified foreign type.</summary>
    /// <returns>
    /// A collection of <typeparamref name="TOperatorSymbol"/> instances whose
    /// <seealso cref="CachedConversionOperatorSymbol.Direction"/> is <seealso cref="ConversionOperatorDirection.FromDeclaring"/>
    /// and the <seealso cref="CachedConversionOperatorSymbol.To"/> type equals the given type.
    /// </returns>
    /// <remarks>This implicitly calls <seealso cref="FromDeclaringType{TOperatorSymbol}(IEnumerable{TOperatorSymbol})"/>.</remarks>
    public static IEnumerable<TOperatorSymbol> ToForeignType<TOperatorSymbol>(this IEnumerable<TOperatorSymbol> operators, ITypeSymbol type)
        where TOperatorSymbol : CachedConversionOperatorSymbol
    {
        return operators.FromDeclaringType().Where(Matches);

        bool Matches(CachedConversionOperatorSymbol symbol) => symbol.To.Equals(type, SymbolEqualityComparer.Default);
    }
}
