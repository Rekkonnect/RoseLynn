# [RoseLynn](https://www.nuget.org/packages/RoseLynn/)

`RoseLynn` is the core package. It contains syntax helpers, semantic helpers, symbol-name utilities, operator inspection helpers, and several abstractions that other RoseLynn packages build on.

## Recommended Starting Points

### Syntax helpers

- [Type Parameter Constraint Clause Segmentation](clauseSegmentation.md)
- [Using Directive Statement Info](usingDirectiveStatementInfo.md)
- [Usings Provider](usingsProvider.md)

### Symbols and names

- [Symbol Name Helpers](symbolNames.md)
- [Signature Views](signatureViews.md)

### Semantics and utility inspection

- [Operator Symbol Utilities](operatorSymbols.md)
- [Awaitable Type Helpers](awaitableTypes.md)

## Other Useful Types

- `AttributeDataViewModel` for projecting Roslyn `AttributeData` into a more inspection-friendly model
- `CachedInfrequentSpecialSymbols` for lazily discovering uncommon but useful members off a type symbol
- `DiagnosticFactory` for constructing diagnostics from descriptors and source locations
- `MetadataReferenceFactory` for test and workspace setup
- `TypeMemoryHelpers` for low-level type size and layout-related queries

## Language-Specific Areas

The core package currently contains several C#-specific helpers under the `RoseLynn.CSharp` namespace, including:

- syntax construction helpers
- attribute helpers
- type/member declaration helpers
- symbol-to-syntax representation helpers

The public docs in this folder currently focus on the APIs that are the most broadly reusable across package consumers.
