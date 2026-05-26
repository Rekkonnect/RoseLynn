# Stored Descriptor Diagnostic Analyzer

`StoredDescriptorDiagnosticAnalyzer` is a small base class for analyzers whose supported diagnostics are declared once and then reused elsewhere through `DiagnosticDescriptorStorageBase`.

## Current model

To implement an analyzer, override:

- `DeclaredDiagnostics`
- `DiagnosticDescriptorStorage`

`SupportedDiagnostics` is sealed and simply returns `DeclaredDiagnostics`.

## Language-specific base classes

RoseLynn exposes two language-specific marker bases:

- `CSharpDiagnosticAnalyzer`
- `VisualBasicDiagnosticAnalyzer`

These currently add structure and intent rather than extra behavior, but they are the preferred entry points for concrete analyzers.

## Example shape

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SampleAnalyzer : CSharpDiagnosticAnalyzer
{
    protected override ImmutableArray<DiagnosticDescriptor> DeclaredDiagnostics
        => [SampleDescriptorStorage.Instance["RSLN0001"]!];

    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage
        => SampleDescriptorStorage.Instance;

    public override void Initialize(AnalysisContext context)
    {
        // register actions
    }
}
```

## Why pair it with descriptor storage

The main advantage is consistency:

- analyzers declare descriptors once
- tests and code fixes can reuse the same descriptor objects
- package code can query diagnostics by analyzer type or rule ID from one place

## Important note

The analyzer base classes do not replace Roslyn's `[DiagnosticAnalyzer(...)]` attribute. Concrete analyzers still need the correct Roslyn attribute for discovery.
