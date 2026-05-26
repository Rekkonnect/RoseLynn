# Multiple Diagnostic Code Fix Provider

`MultipleDiagnosticCodeFixProvider` is a base class for code fixes that apply the same action shape to more than one diagnostic.

## What it provides

- `FixableDiagnosticIds` is derived from `FixableDiagnosticDescriptors`
- `RegisterCodeFixesAsync` is sealed
- `GetFixAllProvider()` defaults to `WellKnownFixAllProviders.BatchFixer`
- you only implement the actual document transformation once

## Members to override

- `FixableDiagnosticDescriptors`
- `CodeFixTitle`
- `PerformCodeFixActionAsync(...)`

## Typical usage

The common pattern is to source descriptors from a shared analyzer storage:

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SampleCodeFixProvider))]
public sealed class SampleCodeFixProvider : MultipleDiagnosticCodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors =>
    [
        SampleDescriptorStorage.Instance["RSLN0001"]!,
        SampleDescriptorStorage.Instance["RSLN0002"]!,
    ];

    public override string CodeFixTitle => "Apply sample fix";

    protected override Task<Document> PerformCodeFixActionAsync(
        CodeFixContext context,
        SyntaxNode syntaxNode,
        CancellationToken cancellationToken)
    {
        // produce the updated document
    }
}
```

## Behavior

For each reported diagnostic in the Roslyn `CodeFixContext`, the base class:

- resolves the syntax root
- finds the syntax node at the diagnostic span
- registers a `CodeAction` with your `CodeFixTitle`
- calls `PerformCodeFixActionAsync(...)`

This is a good fit when multiple related diagnostics can be fixed by the same syntax rewrite.

## Related helpers

`CodeFixContextExtensions` provides convenience methods for common operations such as:

- removing attributes
- removing attribute arguments
- removing or inserting syntax nodes
- retrieving the syntax root using the context's cancellation token
