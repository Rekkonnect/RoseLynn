using Microsoft.CodeAnalysis.Testing;
using System;
using System.Reflection;

namespace RoseLynn.Testing
{
    /// <summary>Provides extension methods for the <see cref="MetadataReferenceCollection"/> type.</summary>
    public static class MetadataReferenceCollectionExtensions
    {
        /// <summary>Adds a reference to the assembly of the provided type to the <seealso cref="MetadataReferenceCollection"/>.</summary>
        /// <param name="collection">The collection to which to add the referenced assembly.</param>
        /// <param name="type">The type whose assembly to add as a reference.</param>
        public static void AddType(this MetadataReferenceCollection collection, Type type)
        {
            collection.Add(type.Assembly);
        }

        /// <summary>Adds a number of assemblies to the <seealso cref="MetadataReferenceCollection"/>.</summary>
        /// <param name="collection">The collection to which to add the referenced assemblies.</param>
        /// <param name="assemblies">The assemblies to add.</param>
        public static void AddAssemblies(this MetadataReferenceCollection collection, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
                collection.Add(assembly);
        }
        /// <summary>Adds a number of assemblies, containing the provided types, to the <seealso cref="MetadataReferenceCollection"/>.</summary>
        /// <param name="collection">The collection to which to add the referenced assemblies.</param>
        /// <param name="types">The types whose assemblies to add as references.</param>
        public static void AddTypes(this MetadataReferenceCollection collection, params Type[] types)
        {
            foreach (var type in types)
                collection.AddType(type);
        }
    }
}
