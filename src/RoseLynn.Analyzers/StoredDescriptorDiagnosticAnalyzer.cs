using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace RoseLynn.Analyzers;

/// <summary>Represents a <seealso cref="DiagnosticAnalyzer"/> that reflects its own stored supported diagnostics.</summary>
public abstract class StoredDescriptorDiagnosticAnalyzer : DiagnosticAnalyzer
{
    /// <summary>Gets the diagnostics declared by the analyzer.</summary>
    protected abstract ImmutableArray<DiagnosticDescriptor> DeclaredDiagnostics { get; }

    /// <summary>Gets the <seealso cref="DiagnosticDescriptorStorageBase"/> that contains this analyzer's supported <seealso cref="DiagnosticDescriptor"/>s.</summary>
    protected abstract DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage { get; }

    /// <summary>Gets the supported diagnostics of the analyzer.</summary>
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => DeclaredDiagnostics;
}

/// <summary>Represents a C# <seealso cref="DiagnosticAnalyzer"/> that reflects its own stored supported diagnostics.</summary>
public abstract class CSharpDiagnosticAnalyzer : StoredDescriptorDiagnosticAnalyzer
{
}
/// <summary>Represents a Visual Basic <seealso cref="DiagnosticAnalyzer"/> that reflects its own stored supported diagnostics.</summary>
public abstract class VisualBasicDiagnosticAnalyzer : StoredDescriptorDiagnosticAnalyzer
{
}
