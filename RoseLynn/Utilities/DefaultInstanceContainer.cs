#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RoseLynn.Utilities
{
    /// <summary>Provides a mechanism to initialize default instances of a type, including deriving types.</summary>
    /// <typeparam name="TBase">The base type whose default instances to initialize.</typeparam>
    public abstract class DefaultInstanceContainer<TBase>
        where TBase : class
    {
        private readonly Dictionary<Type, TBase> defaultInstances;

        /// <summary>Gets all the default instance types that were found in all the assemblies this program depends on.</summary>
        public ImmutableArray<Type> DefaultInstanceTypes { get; }

        /// <summary>Gets the arguments to be used in the construtor.</summary>
        protected abstract object?[] GetDefaultInstanceArguments();
        /// <summary>Gets the argument types to be used in the construtor for each of the valid types to initialize the default instance from.</summary>
        /// <remarks>Defaults to returning the types of the arguments as provided from <seealso cref="GetDefaultInstanceArguments"/>. For <see langword="null"/> objects, the <seealso cref="object"/> type is used.</remarks>
        protected virtual Type[] GetDefaultInstanceArgumentTypes()
        {
            return GetDefaultInstanceArguments().Select(arg => arg?.GetType() ?? typeof(object)).ToArray();
        }

        /// <summary>Initializes a new <seealso cref="DefaultInstanceContainer{TBase}"/> instance with all the types available from <seealso cref="LoadedAssemblyInformation.AllDiscoveredTypes"/>, filtered through <seealso cref="IsValidInstanceType(Type)"/>.</summary>
        /// <remarks>All types that are considered valid must contain a constructor matching the argument types as returned from <seealso cref="GetDefaultInstanceArgumentTypes"/>.</remarks>
        protected DefaultInstanceContainer()
        {
            DefaultInstanceTypes = LoadedAssemblyInformation.AllDiscoveredTypes.Where(IsValidInstanceTypeFullCheck).ToImmutableArray();
            defaultInstances = new(DefaultInstanceTypes.Length);

            var arguments = GetDefaultInstanceArguments();
            var argumentTypes = GetDefaultInstanceArgumentTypes();

            foreach (var type in DefaultInstanceTypes)
            {
                var instance = (TBase)type.GetConstructor(argumentTypes).Invoke(arguments);
                defaultInstances.Add(type, instance);
            }
        }

        /// <summary>Determines whether the given type can be considered a valid instance type.</summary>
        /// <param name="type">The instance type to determine whether it is considered valid. The type will never be <see langword="abstract"/>, and will always be assignable to <typeparamref name="TBase"/> instancess.</param>
        /// <returns><see langword="true"/> if the type's default instance should be stored, otherwise <see langword="false"/>.</returns>
        protected abstract bool IsValidInstanceType(Type type);

        // Bad function name? Yes
        // Proper function implementation? Maybe?
        private bool IsValidInstanceTypeFullCheck(Type type)
        {
            var baseType = typeof(TBase);
            return !type.IsAbstract && baseType.IsAssignableFrom(type) && IsValidInstanceType(type);
        }

        /// <summary>Gets the default initialized instance of the given type.</summary>
        /// <typeparam name="TInstance">The type whose default initialized instance to get.</typeparam>
        /// <returns>The default initialized instance of the type, if it is initialized, otherwise <see langword="null"/>.</returns>
        public TBase? GetDefaultInstance<TInstance>()
            where TInstance : TBase
        {
            return GetDefaultInstance(typeof(TInstance));
        }
        /// <summary>Gets the default initialized instance of the given type.</summary>
        /// <param name="type">The type whose default initialized instance to get.</param>
        /// <returns>The default initialized instance of the type, if it is initialized, otherwise <see langword="null"/>.</returns>
        public TBase? GetDefaultInstance(Type? type)
        {
            return defaultInstances.ValueOrDefault(type);
        }
        /// <summary>Gets the default initialized instance of the type bearing the given type name.</summary>
        /// <param name="typeName">The name of the type whose default initialized instance to get.</param>
        /// <returns>The default initialized instance of the type, if it is initialized, otherwise <see langword="null"/>.</returns>
        public TBase? GetDefaultInstance(string typeName)
        {
            return GetDefaultInstance(DefaultInstanceTypes.FirstOrDefault(t => t.Name == typeName));
        }
    }
}
