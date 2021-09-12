# Multiple Diagnostic Code Fix Provider

The `MutlipleDiagnosticCodeFixProvider` type provides a generalized implementation that provides code fixes for multiple diagnostics reported on the same node, with specified fixable diagnostic IDs provided from `DiagnosticDescriptor` instances instead of their raw IDs.

Additionally, a code fix title is attempted to be retrieved from a `ResourceManager`.

## Declaring Code Fix Provider

The `FixableDiagnosticDescriptors` can be easily initialized as an array of `DiagnosticDescriptor` instances retrieved from a relative [diagnostic descriptor storage](../RoseLynn.Analyzers/descriptorStorage.md).

The code fix title is expected to be a resource with the name `{TypeName}_Title`. The `CodeFixTitle` property is overridable in the case that the resource is to be alternatively retrieved.

For example, a code fix provider named `SomeCodeFixProvider`, its code fix title would be the value of the resource named `SomeCodeFixProvider_Title`.

The fix all provider defaults to the `WellKnownFixAllProviders.BatchFixer` implementation, and is overridable.

The `PerformCodeFixActionAsync` method is the core of the implementation of a code fix, which will be executed on every diagnostic instance.

The resulting code fix class should have a structure similar to this:

```csharp
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExampleRemover))]
public sealed class ExampleRemover : MutlipleDiagnosticCodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => new DiagnosticDescriptor[]
    {
        MOCKDiagnosticDescriptorStorage.Instance[0001]
    };

    protected sealed override ResourceManager ResourceManager => CodeFixResources.ResourceManager;

    protected override async Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        // adjust the document accordingly
    }
}
```
