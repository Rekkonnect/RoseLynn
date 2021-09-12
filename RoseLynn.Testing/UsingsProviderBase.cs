namespace RoseLynn.Testing
{
    /// <summary>Provides a framework to prepending usings to code bound to testing.</summary>
    public abstract class UsingsProviderBase
    {
        /// <summary>Provides the default usings provider that applies no usings.</summary>
        public static readonly UsingsProviderBase Default = new DefaultUsingsProvider();

        /// <summary>Provides the default necessary usings for a piece of source code that is to be tested.</summary>
        public abstract string DefaultNecessaryUsings { get; }

        /// <summary>Prepends the default necessary usings before the original source code and returns the new code with the usings.</summary>
        /// <param name="original">The original source code on which to prepend the usings.</param>
        /// <returns>The resulting source code with the given usings prepended to the original source code.</returns>
        public string WithUsings(string original) => WithUsings(original, DefaultNecessaryUsings);

        /// <summary>Prepends the specified usings before the original source code and returns the new code with the specified usings.</summary>
        /// <param name="original">The original source code on which to prepend the usings.</param>
        /// <param name="usings">The usings to prepend to the source code.</param>
        /// <returns>The resulting source code with the given usings prepended to the original source code.</returns>
        public static string WithUsings(string original, string usings) => $"{usings}\n{original}";

        // TODO: Create factory methods for raw strings and using statement syntax nodes

        private sealed class DefaultUsingsProvider : UsingsProviderBase
        {
            public override string DefaultNecessaryUsings => "";
        }
    }
}
