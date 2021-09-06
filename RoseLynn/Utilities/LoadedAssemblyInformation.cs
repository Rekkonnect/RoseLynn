#nullable enable

using System;
using System.Collections.Immutable;
using System.Linq;

namespace RoseLynn.Utilities
{
    /// <summary>Provides information regarding all the loaded asseblies in the current execution domain.</summary>
    /// <remarks>Due to the potential amount of references to assemblies, loading all the types may take a significant time. Usually, the discovered types range in tens of thousands.</remarks>
    public static class LoadedAssemblyInformation
    {
        /// <summary>Returns an array of all the discovered types.</summary>
        public static ImmutableArray<Type> AllDiscoveredTypes { get; }

        static LoadedAssemblyInformation()
        {
            AllDiscoveredTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).ToImmutableArray();
        }
    }
}
