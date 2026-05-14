using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace RoseLynn.Testing;

/// <summary>Provides helpers for creating test-facing C# compilations from source collections.</summary>
public static class CSharpCompilationExtensions
{
    extension(CSharpCompilation)
    {
        /// <summary>Creates a C# compilation from source strings and explicit references.</summary>
        /// <param name="sources">The source texts to compile.</param>
        /// <param name="references">The metadata references to include.</param>
        /// <param name="parseOptions">Optional parse options.</param>
        /// <param name="compilationOptions">Optional compilation options.</param>
        /// <returns>The created compilation.</returns>
        public static CSharpCompilation Create(
            IEnumerable<string> sources,
            IEnumerable<MetadataReference> references,
            CSharpParseOptions? parseOptions,
            CSharpCompilationOptions? compilationOptions)
        {
            var trees = sources
                .Select(source => CSharpSyntaxTree.ParseText(source, options: parseOptions));
            return CSharpCompilation.Create(null, trees, references, compilationOptions);
        }

        /// <summary>Creates a C# compilation and augments it with resolved reference assemblies.</summary>
        /// <param name="sources">The source texts to compile.</param>
        /// <param name="references">The metadata references to include initially.</param>
        /// <param name="referenceAssemblies">The reference assemblies to resolve and add.</param>
        /// <param name="parseOptions">Optional parse options.</param>
        /// <param name="compilationOptions">Optional compilation options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created compilation.</returns>
        public static async Task<CSharpCompilation> Create(
            IEnumerable<string> sources,
            IEnumerable<MetadataReference> references,
            ReferenceAssemblies referenceAssemblies,
            CSharpParseOptions? parseOptions,
            CSharpCompilationOptions? compilationOptions,
            CancellationToken cancellationToken)
        {
            var compilation = CSharpCompilation.Create(
                sources,
                references,
                parseOptions,
                compilationOptions);
            return await compilation.AddReferenceAssemblies(
                referenceAssemblies,
                cancellationToken);
        }
    }
}
