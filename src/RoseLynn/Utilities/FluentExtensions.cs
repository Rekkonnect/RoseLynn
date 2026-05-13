namespace RoseLynn.Utilities;

public static class FluentExtensions
{
    // Kind of a misnomer; it's not really select, but transform doesn't sit well either
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

    public static T ApplyOnSelf<T>(this T source, Action<T> action)
    {
        action(source);
        return source;
    }

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
