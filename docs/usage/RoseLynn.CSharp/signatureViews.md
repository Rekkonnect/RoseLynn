# Signature Views

The `RoseLynn.CSharp.SignatureViews` namespace projects Roslyn symbols and compilations into immutable signature models. These models are useful when you want to compare, persist, serialize, or emit symbol information without holding on to the original Roslyn object graph.

## Main entry points

- `Compilation.ToSignatureView()`
- `AssemblyIdentity.ToSignatureView()`
- `ISymbol.ToSymbolSignatureView(Compilation?)`

There are also strongly typed overloads for common symbol kinds such as:

- `INamedTypeSymbol`
- `IMethodSymbol`
- `IPropertySymbol`
- `IParameterSymbol`
- `INamespaceSymbol`

## Why use signature views

- they are easier to compare than live Roslyn symbols
- they are shaped around symbol identity and signature data rather than workspace state
- they work well with generation helpers such as `CSharpCodeWriter`

## Example

```csharp
var methodView = methodSymbol.ToSymbolSignatureView(compilation);
var typeView = namedTypeSymbol.ToSymbolSignatureView(compilation);
```

## Common usage patterns

- snapshot a symbol before a transform or rewrite
- build deterministic signatures for tests
- feed type/member information into code-generation helpers
- inspect nullable annotations, type parameters, parameter modifiers, and container structure from a stable view model

## Related APIs

- `SignatureViewExtensions` for construction and cloning helpers
- `WellKnownSymbolSignatureViews` for reusable predefined framework signatures
- `CSharpCodeWriter.AppendPartialType(...)` and parameter-writing helpers for turning signature views back into source
