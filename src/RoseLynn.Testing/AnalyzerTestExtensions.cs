using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace RoseLynn.Testing;

/// <summary>Provides helpers for configuring Roslyn analyzer test instances.</summary>
public static class AnalyzerTestExtensions
{
    extension<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        /// <summary>Applies reference assemblies and additional metadata references to an analyzer test.</summary>
        /// <param name="referenceAssemblies">The reference assembly set to use.</param>
        /// <param name="metadataReferences">The additional metadata references to add.</param>
        public void ApplyReferences(
            ReferenceAssemblies referenceAssemblies,
            IEnumerable<MetadataReference> metadataReferences)
        {
            test.ReferenceAssemblies = referenceAssemblies;
            test.TestState.AdditionalReferences.AddRange(metadataReferences);
        }
    }
}
