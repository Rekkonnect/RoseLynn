using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace RoseLynn.Testing;

public static class CSharpCompilationExtensions
{
    extension(CSharpCompilation)
    {
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
