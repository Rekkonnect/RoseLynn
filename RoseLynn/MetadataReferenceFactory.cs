using Microsoft.CodeAnalysis;
using System;
using System.Reflection;

namespace RoseLynn
{
    /// <summary>Provides functions useful for more conveniently creating <seealso cref="MetadataReference"/> objects.</summary>
    public static class MetadataReferenceFactory
    {
        /// <summary>Creates a <seealso cref="PortableExecutableReference"/> instance out of an <seealso cref="Assembly"/>.</summary>
        /// <param name="assembly">The assembly from which to create the <seealso cref="PortableExecutableReference"/>.</param>
        /// <returns>The <see cref="PortableExecutableReference"/> to the provided assembly.</returns>
        /// <remarks>This acts as a replacement for the now-obsolete <seealso cref="MetadataReference.CreateFromAssembly(Assembly)"/> function.</remarks>
        public static PortableExecutableReference CreateFromAssembly(Assembly assembly)
        {
            return MetadataReference.CreateFromFile(assembly.Location);
        }

        /// <summary>Creates a <seealso cref="PortableExecutableReference"/> instance out of a type.</summary>
        /// <typeparam name="T">The type from which to create the <seealso cref="PortableExecutableReference"/>.</typeparam>
        /// <returns>The <see cref="PortableExecutableReference"/> to the assembly containing the provided type.</returns>
        public static PortableExecutableReference CreateFromType<T>() => CreateFromType(typeof(T));
        /// <summary>Creates a <seealso cref="PortableExecutableReference"/> instance out of a type.</summary>
        /// <param name="type">The type from which to create the <seealso cref="PortableExecutableReference"/>.</param>
        /// <returns>The <see cref="PortableExecutableReference"/> to the assembly containing the provided type.</returns>
        public static PortableExecutableReference CreateFromType(Type type) => CreateFromAssembly(type.Assembly);
    }
}
