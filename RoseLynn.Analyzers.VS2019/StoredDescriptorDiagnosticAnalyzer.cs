using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace RoseLynn.Analyzers;

/// <summary>Represents a <seealso cref="DiagnosticAnalyzer"/> that reflects its own stored supported diagnostics.</summary>
public abstract class StoredDescriptorDiagnosticAnalyzer : DiagnosticAnalyzer
{
    /// <summary>Gets the <seealso cref="DiagnosticDescriptorStorageBase"/> that contains this analyzer's supported <seealso cref="DiagnosticDescriptor"/>s.</summary>
    protected abstract DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage { get; }

    /// <summary>Gets the supported diagnostics of the analyzer.</summary>
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    /// <summary>Initializes a new <seealso cref="StoredDescriptorDiagnosticAnalyzer"/> instance, discovering its <seealso cref="SupportedDiagnostics"/>.</summary>
    protected StoredDescriptorDiagnosticAnalyzer()
    {
        SupportedDiagnostics = DiagnosticDescriptorStorage.GetDiagnosticDescriptors(GetType());
    }
}

/// <summary>Represents a C# <seealso cref="DiagnosticAnalyzer"/> that reflects its own stored supported diagnostics.</summary>
public abstract class CSharpDiagnosticAnalyzer : StoredDescriptorDiagnosticAnalyzer
{
}
/// <summary>Represents a Visual Basic <seealso cref="DiagnosticAnalyzer"/> that reflects its own stored supported diagnostics.</summary>
public abstract class VisualBasicDiagnosticAnalyzer : StoredDescriptorDiagnosticAnalyzer
{
}
