using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;

namespace RoseLynn.Diagnostics
{
    /// <summary>Provides useful extensions for the <seealso cref="SyntaxNodeAnalysisContext"/> type.</summary>
    public static class SyntaxNodeAnalysisContextExtensions
    {
        /// <summary>Reports diagnostics on the specified nodes, with each node being reported a diagnostic based on the given generator.</summary>
        /// <typeparam name="TNode">The type of the node that the diagnostics will be reported on.</typeparam>
        /// <param name="context">The <seealso cref="SyntaxNodeAnalysisContext"/>.</param>
        /// <param name="nodes">The nodes on which the diagnostics will be reported on.</param>
        /// <param name="diagnosticGenerator">The function that generates a new <seealso cref="Diagnostic"/> instance for each node that diagnostics will be reported on.</param>
        public static void ReportDiagnostics<TNode>(this SyntaxNodeAnalysisContext context, IEnumerable<TNode> nodes, Func<TNode, Diagnostic> diagnosticGenerator)
            where TNode : SyntaxNode
        {
            foreach (var n in nodes)
                context.ReportDiagnostic(diagnosticGenerator(n));
        }
    }
}
