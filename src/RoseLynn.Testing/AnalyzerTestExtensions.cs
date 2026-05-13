using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;

namespace RoseLynn.Testing;

public static class AnalyzerTestExtensions
{
    extension<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        public void ApplyReferences(
            ReferenceAssemblies referenceAssemblies,
            IEnumerable<MetadataReference> metadataReferences)
        {
            test.ReferenceAssemblies = referenceAssemblies;
            test.TestState.AdditionalReferences.AddRange(metadataReferences);
        }
    }
}
