namespace RoseLynn.Utilities;

/// <summary>Provides a collection of selector functions.</summary>
public static class Selectors
{
    /// <summary>Makes a tuple out of the provided arguments.</summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <param name="value1">The first argument.</param>
    /// <param name="value2">The second argument.</param>
    /// <returns>A tuple containing the two arguments in the order they were given.</returns>
    public static (T1, T2) MakeTuple<T1, T2>(T1 value1, T2 value2) => (value1, value2);
}
