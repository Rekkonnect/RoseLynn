using System;

namespace RoseLynn.Utilities
{
    /// <summary>Provides useful extensions for the <seealso cref="Type"/> type.</summary>
    public static class TypeExtensions
    {
        /// <summary>Gets the generic type definition of a generic type.</summary>
        /// <param name="type">The type whose generic type definition to get.</param>
        /// <returns>The generic type definition of the generic type, or the provided type itself if the type is not generic.</returns>
        public static Type GetGenericTypeDefinitionOrSame(this Type type)
        {
            if (!type.IsGenericType)
                return type;

            return type.GetGenericTypeDefinition();
        }
    }
}
