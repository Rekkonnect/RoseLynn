using System;

namespace RoseLynn.Analyzers
{
    /// <summary>Denotes that a diagnostic field or property is supported by the specified diagnostics analyzer type.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DiagnosticSupportedAttribute : Attribute
    {
        public Type DiagnosticAnalyzerType { get; }

        public DiagnosticSupportedAttribute(Type diagnosticAnalyzerType) => DiagnosticAnalyzerType = diagnosticAnalyzerType;
    }
}
