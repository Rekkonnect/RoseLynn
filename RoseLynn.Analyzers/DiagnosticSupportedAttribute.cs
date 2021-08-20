using System;

namespace RoseLynn.Analyzers
{
    /// <summary>Denotes that a diagnostic field is supported by the specified diagnostics analyzer type.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DiagnosticSupportedAttribute : Attribute
    {
        public Type DiagnosticAnalyzerType { get; }

        public DiagnosticSupportedAttribute(Type diagnosticAnalyzerType) => DiagnosticAnalyzerType = diagnosticAnalyzerType;
    }
}
