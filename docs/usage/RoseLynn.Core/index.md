# [RoseLynn.Core](https://www.nuget.org/packages/RoseLynn.Core/)

`RoseLynn.Core` is the foundational package. It contains language-agnostic Roslyn helpers, semantic utilities, operator inspection helpers, and abstractions that other RoseLynn packages build on. It carries only `Microsoft.CodeAnalysis.Common` as a Roslyn dependency — no C# or Visual Basic compiler references.

## Recommended Starting Points

### Semantics and utility inspection

- [Operator Symbol Utilities](operatorSymbols.md)
- [Awaitable Type Helpers](awaitableTypes.md)
- [Signature Views](signatureViews.md)

## Other Useful Types

- `AttributeDataViewModel` for projecting Roslyn `AttributeData` into a more inspection-friendly model
- `CachedInfrequentSpecialSymbols` for lazily discovering uncommon but useful members off a type symbol
- `DiagnosticFactory` for constructing diagnostics from descriptors and source locations
- `MetadataReferenceFactory` for test and workspace setup
- `TypeMemoryHelpers` for low-level type size and layout-related queries

## Language-Specific Packages

C#-specific helpers live in [RoseLynn.CSharp](../RoseLynn.CSharp/index.md). Visual Basic helpers live in [RoseLynn.VisualBasic](../RoseLynn.VisualBasic/index.md). To get both at once, depend on [RoseLynn.CrossLanguage](../RoseLynn.CrossLanguage/index.md).
