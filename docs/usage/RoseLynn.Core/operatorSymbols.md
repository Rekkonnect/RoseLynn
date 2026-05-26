# Operator Symbol Utilities

The core package includes a small operator-model layer on top of Roslyn symbols. It is useful when you want to inspect user-defined operators without repeatedly parsing `IMethodSymbol.Name`, checking `MethodKind`, or manually separating checked and unchecked variants.

## Main entry points

- `CachedOperatorSymbols.ForType(ITypeSymbol)` scans a type and caches all operator methods that RoseLynn can classify
- `CachedOperatorSymbol.Parse(IMethodSymbol)` converts a Roslyn operator method into a richer RoseLynn model
- `OperatorKindFacts` maps CLR operator names such as `op_Addition` and `op_CheckedAddition`
- `CachedOperatorSymbolQuerying` provides sequence filters for common queries

## Supported operator metadata

Each cached operator exposes:

- `CheckingMode`
- `OperatorKind`
- `CompositeOperatorKind`
- `OperatorMethodKind`
- the underlying Roslyn `IMethodSymbol`

The current `OperatorKind` model includes newer C# forms such as:

- checked arithmetic operator names
- checked explicit conversions
- unsigned right shift via `>>>`

## Typical flow

```csharp
var operatorSymbols = CachedOperatorSymbols.ForType(typeSymbol);

var checkedAdditions = operatorSymbols
    .OfKind(OperatorKind.Addition)
    .OfCheckingMode(OperatorCheckingMode.Checked);

var explicitConversions = operatorSymbols.ExplicitConversions;
```

## Query options

`CachedOperatorSymbols` exposes a few direct groupings:

- `AllOperators`
- `UnaryOperators`
- `BinaryOperators`
- `ImplicitConversions`
- `ExplicitConversions`
- `CheckedOperators`
- `UncheckedOperators`

For more composable filtering, use `CachedOperatorSymbolQuerying`:

```csharp
var uncheckedExplicitConversions = operatorSymbols
    .ExplicitConversions
    .OfCheckingMode(OperatorCheckingMode.Unchecked);
```

## Conversion helpers

Conversion operators are represented by `CachedConversionOperatorSymbol` and expose:

- `From`
- `To`
- `Direction`
- `CheckedDirection`
- `OtherType`

This is useful when you need to answer questions such as "does this type define a conversion to `string`?" or "which side of the conversion is the declaring type?".

## Notes

- The cached model is built from Roslyn symbols, not syntax.
- `InvolvesDeclaringType` is mostly a safety check for unusual metadata scenarios; in ordinary C# source it should typically be true.
- `OperatorKindFacts.CanBeChecked` reflects which operator kinds currently support checked variants in the package model.
