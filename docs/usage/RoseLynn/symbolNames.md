# Symbol Name Helpers

`SymbolNameHelpers` provides display-oriented helpers for producing stable names from Roslyn symbols without depending on the default symbol-display formatting pipeline.

## Main entry points

- `FullyQualifiedString(ISymbol, Compilation?)`
- `FullyQualified(ISymbol, Compilation?)`
- `FullyQualifiedOrKeyword(...)`
- `KeywordIdentifierWithNullability(ITypeSymbol)`

## What they are useful for

- building deterministic symbol names for analysis output
- generating source that should prefer language keywords like `string` and `int` when possible
- keeping nullability markers when projecting type names
- preserving extern-alias qualification when the compilation context requires it

## Example

```csharp
var qualified = SymbolNameHelpers.FullyQualifiedString(typeSymbol, compilation);
```

For framework types, the result looks like:

```text
global::System.String
```

If a symbol comes from an aliased metadata reference, the helper can emit the extern alias form instead of `global::...`.

## Returned models

The non-string overloads return lightweight records:

- `FullyQualifiedName`
- `FullMetadataName`

These are useful when you want to keep more than just the flattened string representation.

## When to use `FullyQualifiedOrKeyword`

Use `FullyQualifiedOrKeyword` when generated output should prefer C# predefined type keywords where available, while still handling nullability correctly:

- `string`
- `int`
- `bool?`

If the type does not have a predefined keyword representation, the helper falls back to a fully-qualified name.
