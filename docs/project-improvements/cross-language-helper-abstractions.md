# Separate C# and VB Helpers Behind a Unified Package

## Status

Implemented. `RoseLynn.Core` is now the language-neutral shared base. `RoseLynn.CSharp` and `RoseLynn.VisualBasic` are dedicated language-specific packages. `RoseLynn.CrossLanguage` is the unifying bundle package.

## Current trouble

The repository already has a mix of:

- C#-specific helpers under dedicated namespaces
- some core abstractions that are language-agnostic
- a few VB-aware concepts in analyzer-facing areas

The boundaries are not clean enough yet to let C# and VB helpers evolve independently while still presenting a coherent package story to consumers.

## Proposed improvement

Refactor the helper layers so that C#-specific and VB-specific helpers can live in dedicated spaces while a higher-level package can still unify both.

Desired end state:

- language-neutral abstractions live in a truly shared core
- C# helpers live in a clearly isolated C# area
- VB helpers can be introduced without awkward placement in C#-leaning namespaces
- consumers can choose either language-specific packages or a unified package that references both

## Why defer

This is broader than a namespace cleanup:

- API ownership boundaries need to be defined carefully
- public surface moves may be breaking
- some current helpers are only partially abstracted and would need redesign first

## Future work outline

1. inventory which public APIs are genuinely language-neutral
2. identify C#-only helpers that should move behind clearer C# package/namespace boundaries
3. sketch equivalent VB helper shapes where they make sense
4. define how a unifying package should compose both without duplicating concepts
5. update docs and packaging together if the split is pursued

## Reason for note

This direction supports cleaner architecture and future language coverage, but it should be approached as a package-architecture milestone rather than incremental cleanup.
