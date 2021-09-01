using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoseLynn.Analyzers
{
    /// <summary>Denotes that a <seealso cref="DiagnosticDescriptor"/> member is supported by the specified <seealso cref="DiagnosticAnalyzer"/> type.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DiagnosticSupportedAttribute : Attribute
    {
        /// <summary>The type that represents the <see cref="DiagnosticAnalyzer"/> that supports the marked member representing a <seealso cref="DiagnosticDescriptor"/>.</summary>
        public Type DiagnosticAnalyzerType { get; }

        /// <summary>Initializes a new instance of the <seealso cref="DiagnosticSupportedAttribute"/>.</summary>
        /// <param name="diagnosticAnalyzerType">The type that represents the <see cref="DiagnosticAnalyzer"/> that supports the marked member representing a <seealso cref="DiagnosticDescriptor"/>.</param>
        public DiagnosticSupportedAttribute(Type diagnosticAnalyzerType) => DiagnosticAnalyzerType = diagnosticAnalyzerType;
    }
}
