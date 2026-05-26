# Explore Diagnostic Suppressors

## Status

Exploration only. No implementation commitment yet.

## Current trouble

RoseLynn currently focuses on analyzers, code fixes, generators, and testing helpers, but it does not yet offer guidance or abstractions around Roslyn `DiagnosticSuppressor` authoring.

That leaves a gap for scenarios where:

- an existing diagnostic is technically correct but undesired in a known pattern
- teams want to suppress diagnostics based on semantic context rather than global configuration
- analyzer packages need a structured way to coexist with third-party diagnostics

## Proposed improvement

Investigate whether RoseLynn should expose dedicated suppressor helpers, examples, or a package slice for `DiagnosticSuppressor` implementations.

Questions to answer:

- is a suppressor-specific base abstraction actually helpful, or would it just wrap a small Roslyn API surface?
- should suppressor testing live in `RoseLynn.Testing`, and if so, what ergonomic helpers are missing today?
- can descriptor storage or similar registry concepts be reused for suppression descriptors?

## Why defer

This needs design work before code:

- suppressors have different lifecycle and authoring concerns from analyzers
- poor abstractions here could add little value over plain Roslyn
- the testing story should be clear before committing to public APIs

## Future work outline

1. prototype a small suppressor package or test project
2. document common suppressor scenarios and pain points
3. evaluate whether shared registration, descriptor, or testing helpers emerge naturally
4. only then decide whether public package APIs are justified

## Reason for note

This is a realistic extension of RoseLynn's analysis tooling focus, but the package should not grow surface area here without evidence that the abstraction earns its keep.
