#nullable enable

using System;

namespace RoseLynn;

/// <summary>
/// Code style extensions that you may or may not want to use -- convenience involves investment.
/// </summary>
/// <remarks>Multiple paradigms are fun, not scary. Proceed at your own risk of having an open mind.</remarks>
public static class MoreLikeFunctionalProgramming
{
    /// <summary>
    /// Executes an action on the passed argument, only if the filter predicate succeeds.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument that is being filtered and acted upon.</typeparam>
    /// <param name="argument">The argument to filter and act upon.</param>
    /// <param name="filterPredicate">
    /// The predicate that determines whether the action will be executed on the argument.
    /// If the predicate returns <see langword="true"/>, the action is executed, otherwise the method exits.
    /// </param>
    /// <param name="action">The action to perform on the argument, if the filter predicate succeeds.</param>
    /// <returns>
    /// <see langword="true"/> if the filter predicate passes, otherwise <see langword="false"/>.
    /// </returns>
    public static bool ExecuteFiltered<TArgument>(this TArgument argument, Predicate<TArgument> filterPredicate, Action<TArgument> action)
    {
        bool passes = filterPredicate(argument);
        if (passes)
        {
            action(argument);
        }
        return passes;
    }
    /// <summary>
    /// Executes an action on the passed argument, only if the filter predicate succeeds.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument that is being filtered and acted upon.</typeparam>
    /// <typeparam name="TResult">The type of the result returned from the function performed on the argument.</typeparam>
    /// <param name="argument">The argument to filter and act upon.</param>
    /// <param name="filterPredicate">
    /// The predicate that determines whether the action will be executed on the argument.
    /// If the predicate returns <see langword="true"/>, the action is executed, otherwise the method exits.
    /// </param>
    /// <param name="func">The action to perform on the argument, if the filter predicate succeeds.</param>
    /// <param name="result">
    /// The returned value from the function's execution.
    /// This will be equal to <see langword="default"/> if the filter predicate fails.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the filter predicate passes, otherwise <see langword="false"/>.
    /// </returns>
    public static bool ExecuteFiltered<TArgument, TResult>(this TArgument argument, Predicate<TArgument> filterPredicate, Func<TArgument, TResult> func, out TResult? result)
    {
        bool passes = filterPredicate(argument);
        result = default;
        if (passes)
        {
            result = func(argument);
        }
        return passes;
    }
}
