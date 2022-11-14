using System;
using System.Reflection;

namespace RoseLynn.Utilities;

#nullable enable

/// <summary>Contains extensions for the <seealso cref="Type"/> class.</summary>
public static class TypeExtensions
{
    /// <summary>Gets the declaring method of a type, without failing if the constraints are not met.</summary>
    /// <param name="type">The type whose declaring method to get.</param>
    /// <returns>The declaring method of the type parameter, or <see langword="null"/> if the provided type is not a type parameter.</returns>
    public static MethodBase? GetDeclaringMethodSafe(this Type type)
    {
        if (type.IsGenericParameter)
            return type.DeclaringMethod;

        return null;
    }
}
