namespace RoseLynn.Utilities;

/// <summary>Provides small fluent helpers for null handling and inline transformations.</summary>
public static class FluentExtensions
{
    // Kind of a misnomer; it's not really select, but transform doesn't sit well either
    /// <summary>Transforms a reference value when it is not <see langword="null"/>.</summary>
    /// <typeparam name="TSource">The source reference type.</typeparam>
    /// <typeparam name="TResult">The result reference type.</typeparam>
    /// <param name="source">The source value to transform.</param>
    /// <param name="selector">The transformation applied when <paramref name="source"/> is not <see langword="null"/>.</param>
    /// <returns>The transformed value, or <see langword="null"/> when the source is <see langword="null"/>.</returns>
    public static TResult? SelectOrNull<TSource, TResult>(
        this TSource? source,
        Func<TSource, TResult?> selector)
        where TSource : class
        where TResult : class
    {
        if (source is null)
        {
            return null;
        }

        return selector(source);
    }

    /// <summary>Executes an action only when the source reference is not <see langword="null"/>.</summary>
    /// <typeparam name="T">The source reference type.</typeparam>
    /// <param name="source">The source value to inspect.</param>
    /// <param name="action">The action to execute when the source is not <see langword="null"/>.</param>
    public static void IfNotNull<T>(
        this T? source,
        Action<T> action)
        where T : class
    {
        if (source is not null)
        {
            action(source);
        }
    }

    /// <summary>Applies an action to a value and then returns the same value.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The value to pass through.</param>
    /// <param name="action">The action to apply to the value.</param>
    /// <returns>The original <paramref name="source"/>.</returns>
    public static T ApplyOnSelf<T>(this T source, Action<T> action)
    {
        action(source);
        return source;
    }

    /// <summary>Applies an action to each element and inserts an intermediate action between elements.</summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="sourceAction">The action applied to each element.</param>
    /// <param name="intermediateAction">The action applied between consecutive elements.</param>
    public static void ApplyWithIntermediate<T>(
        this IEnumerable<T> source,
        Action<T> sourceAction,
        Action intermediateAction)
    {
        bool first = true;
        foreach (var item in source)
        {
            if (!first)
            {
                intermediateAction();
            }

            sourceAction(item);
            first = false;
        }
    }
}
