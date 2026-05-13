using System;
using System.Collections.Generic;

namespace RoseLynn.Utilities;

public static class FluentExtensions
{
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
