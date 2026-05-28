using Microsoft.CodeAnalysis;
using RoseLynn.Analyzers;

namespace RoseLynn.PoC.UnitTests;

public sealed class PoCDiagnosticDescriptorStorage : DiagnosticDescriptorStorageBase
{
    public static readonly PoCDiagnosticDescriptorStorage Instance = new();

    public const string Category = "PoC";

    public readonly DiagnosticDescriptor POC0001_TwoParameterMethodRule;

    private PoCDiagnosticDescriptorStorage()
        : base(typeof(TwoParameterMethodAnalyzer))
    {
        POC0001_TwoParameterMethodRule = TwoParameterMethodAnalyzer.POC0001_TwoParameterMethodRule;
    }
}
