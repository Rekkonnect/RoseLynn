using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace RoseLynn.Analyzers;

/// <summary>Represents a <seealso cref="DiagnosticSuppressor"/> that reflects its own stored supported suppressions.</summary>
public abstract class StoredDescriptorDiagnosticSuppressor : DiagnosticSuppressor
{
    /// <summary>Gets the suppression descriptors declared by this suppressor.</summary>
    protected abstract ImmutableArray<SuppressionDescriptor> DeclaredSuppressions { get; }

    /// <summary>Gets the supported suppressions.</summary>
    public sealed override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => DeclaredSuppressions;
}

/// <summary>Represents a C# <seealso cref="StoredDescriptorDiagnosticSuppressor"/>.</summary>
public abstract class CSharpDiagnosticSuppressor : StoredDescriptorDiagnosticSuppressor
{
}

/// <summary>Represents a Visual Basic <seealso cref="StoredDescriptorDiagnosticSuppressor"/>.</summary>
public abstract class VisualBasicDiagnosticSuppressor : StoredDescriptorDiagnosticSuppressor
{
}
