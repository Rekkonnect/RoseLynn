# Keep Operator Symbol Utilities Current With Newer C# Operators

## Status

Ongoing maintenance concern.

## Current trouble

RoseLynn already models operator metadata through:

- `OperatorKind`
- `OperatorKindFacts`
- `CachedOperatorSymbols`
- `CachedOperatorSymbolQuerying`

These utilities are only valuable if they stay aligned with modern C# operator forms and Roslyn naming behavior.

Recent examples already relevant here include:

- checked operator method names
- checked explicit conversions
- unsigned right shift via `>>>`

## Proposed improvement

Treat operator coverage as an explicitly maintained compatibility area.

That means:

- reviewing new C# language versions for operator-related changes
- updating the `OperatorKind` model when Roslyn or the language adds relevant cases
- keeping docs and tests in sync with those additions

## Why defer full action

This is not one feature. It is a maintenance track that needs periodic review and regression coverage rather than a single implementation pass.

## Future work outline

1. audit `OperatorKind`, `OperatorKindFacts`, and `CachedOperatorSymbols` against the current C# language surface
2. add or expand tests for checked and newer operators
3. document supported operators in the public usage docs
4. repeat this audit when adopting new Roslyn/C# versions

## Reason for note

Operator modeling is subtle and easy to let drift. Capturing it as a tracked concern makes it less likely that language evolution silently outpaces the helper layer.
