# Split RoseLynn Core and Provide an Analysis Bundle

## Status

Deferred. Do not implement yet.

## Current trouble

The current package layout makes the base `RoseLynn` package carry a wide mix of utilities:

- core Roslyn abstractions
- analysis-facing helpers
- language-specific helpers

At the same time, consumers who want "the analyzer stack" often need to discover and install multiple adjacent packages:

- `RoseLynn.Analyzers`
- `RoseLynn.CodeFixes`
- `RoseLynn.Generators`

## Proposed improvement

Split the base package into a narrower `RoseLynn.Core` package and provide a bundle package named `RoseLynn.Bundle.Analysis`.

Intended package roles:

- `RoseLynn.Core`: foundational cross-cutting Roslyn helpers and low-level abstractions
- `RoseLynn.Analyzers`: analyzer-specific helpers
- `RoseLynn.CodeFixes`: code-fix-specific helpers
- `RoseLynn.Generators`: source-generator-specific helpers
- `RoseLynn.Bundle.Analysis`: a convenience package that pulls together `.Analyzers`, `.CodeFixes`, and `.Generators`

## Why defer

This is a packaging and branding change with real migration cost:

- namespace decisions may need to be revisited
- NuGet dependency graphs will change
- package documentation and examples need a coordinated rewrite
- users may need migration guidance if types move between packages

## Future work outline

1. define which types remain in `RoseLynn.Core`
2. identify types that are analysis-specific rather than foundational
3. decide whether namespaces stay stable while package ownership changes
4. introduce `RoseLynn.Bundle.Analysis` as a meta-package
5. prepare release notes and migration documentation before any breaking split

## Reason for note

This improves package discoverability and makes consumer intent clearer, but it should be done as a deliberate package-design pass rather than mixed into regular feature work.
