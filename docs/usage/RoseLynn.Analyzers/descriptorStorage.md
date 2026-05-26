# Diagnostic Descriptor Storage

`DiagnosticDescriptorStorageBase` is a read-only registry around analyzers and their declared descriptors.

The current implementation no longer builds descriptors inside the storage itself. Instead, it discovers descriptors from analyzer types that already expose `SupportedDiagnostics`.

## What the storage does

The storage constructor accepts analyzer types:

```csharp
public sealed class SampleDescriptorStorage : DiagnosticDescriptorStorageBase
{
    public static readonly SampleDescriptorStorage Instance = new();

    private SampleDescriptorStorage()
        : base(typeof(SampleAnalyzer), typeof(AnotherSampleAnalyzer))
    {
    }
}
```

At construction time, the storage:

- creates each analyzer
- reads `SupportedDiagnostics`
- groups descriptors by analyzer type
- indexes descriptors by full ID and, when possible, by the trailing 4-digit numeric ID

## Available lookups

- `GetDiagnosticDescriptor(int ruleID)`
- `GetDiagnosticDescriptor(string ruleID)`
- `GetDiagnosticDescriptors(Type analyzerType)`
- `GetDiagnosticDescriptors<TAnalyzer>()`
- `GetDiagnosticDescriptorsByAnalyzersImmutable()`

The indexers `this[int]` and `this[string]` delegate to the matching lookup methods.

## Numeric ID behavior

Numeric lookup is only intended for IDs whose last four characters are decimal digits.

Examples:

- `RSLN0001` can be retrieved with `storage[1]`
- `RSLN1204` can be retrieved with `storage[1204]`
- IDs that do not end in four digits are still supported by string lookup, but not by numeric lookup

## Constraints and validation

During discovery, the storage rejects:

- types that are not `DiagnosticAnalyzer`
- analyzer types that cannot be instantiated
- duplicate descriptor IDs
- duplicate trailing 4-digit numeric IDs

That makes the storage useful as both a lookup helper and an integrity guard for analyzer packages.

## Typical use

The storage is commonly used from:

- test fixtures that infer the tested rule from the test type name
- code fix providers that want to expose fixable diagnostics through shared descriptor references
- package-level helper code that wants a single place for descriptor retrieval
