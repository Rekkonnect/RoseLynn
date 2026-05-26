# Analyzer Test Fixture

`BaseAnalyzerTestFixture` provides a small but useful convention: it can infer the tested diagnostic rule from the test type name and a shared `DiagnosticDescriptorStorageBase`.

## How rule inference works

By default, `TestedDiagnosticRule`:

- enumerates all descriptors in `DiagnosticDescriptorStorage`
- prefers the longest matching diagnostic ID
- checks whether the current test type name starts with that ID

This means test classes such as `RSLN0001_Tests` or `RSLN0001AnalyzerTests` can resolve their descriptor automatically.

If your test naming does not follow that convention, override `TestedDiagnosticRule`.

## Required member

Concrete fixtures must provide:

- `DiagnosticDescriptorStorage`

## Typical derived fixtures

The base fixture is usually consumed through higher-level abstractions such as:

- `BaseDiagnosticTests`
- `BaseDiagnosticTests<TAnalyzer>`
- `BaseDiagnosticTests<TAnalyzer, TUsingsProvider>`
- `BaseCodeFixDiagnosticTests<TAnalyzer, TCodeFix>`

## Why it is useful

The convention keeps analyzer tests aligned with the shared descriptor registry and removes a common source of duplication in rule-specific fixtures.
