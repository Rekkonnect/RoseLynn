#nullable enable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.Testing;

[NeedsDocs]
public static class CompilationExtensions
{
    extension<TCompilation>(TCompilation compilation)
        where TCompilation : Compilation
    {
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
