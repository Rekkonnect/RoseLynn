#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.Testing;

/// <summary>Provides helpers for adding reference assemblies to Roslyn compilations used in tests.</summary>
public static class CompilationExtensions
{
    extension<TCompilation>(TCompilation compilation)
        where TCompilation : Compilation
    {
        /// <summary>Resolves and adds the specified reference assemblies to a compilation.</summary>
        /// <param name="referenceAssemblies">The reference assemblies to resolve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The compilation with the resolved references added.</returns>
        public async Task<TCompilation> AddReferenceAssemblies(
            ReferenceAssemblies referenceAssemblies,
            CancellationToken cancellationToken)
        {
            var metadata = await referenceAssemblies.ResolveAsync(
                compilation.Language, cancellationToken);
            return (TCompilation)compilation.AddReferences(metadata);
        }
    }
}
