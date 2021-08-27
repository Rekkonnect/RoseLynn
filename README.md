# RoseLynn

Additional tools for Roslyn-based applications

# Features
## [RoseLynn](https://www.nuget.org/packages/RoseLynn/)
- Several extensions for common cases, including missing abstractions from the Microsoft.CodeAnalysis APIs
- `TypeParameterConstraintClauseSegmentation` that helps better organize clauses
- Higher-level operations for code fixes
- Report diagnostics on multiple nodes

## [RoseLynn.Analyzers](https://www.nuget.org/packages/RoseLynn.Analyzers/)
- `DiagnosticDescriptorStorage` types that help better organize your workload by automatically detecting the supported diagnostics by the analyzer (usage docs are WIP)

## [RoseLynn.CodeFixes](https://www.nuget.org/packages/RoseLynn.CodeFixes/)
- `MultipleDiagnosticCodeFixProvider` abstracts away registering single code fix operations for multiple diagnostics that may be reported on the same node

## [RoseLynn.Testing](https://www.nuget.org/packages/RoseLynn.Testing/)
- Testing fixtures for both analyzers and code fixes
  - Testing fixture types' names may be used to infer the related tested diagnostic rule
  - Associativity with `DiagnosticDescriptorStorage` types
- Handlers for diagnostic indicator markup in test code
  - Package already has pre-built support for [`Microsoft.CodeAnalysis.Testing`](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Analyzer.Testing/) and [`Gu.Roslyn.Asserts`](https://www.nuget.org/packages/Gu.Roslyn.Asserts/)
  - Convert between styles
  - Wholly remove markup indicators from code string
  - Add diagnostic indicator to code snippet using the handler's markup style
- `UsingsProvider` types that provide the ability to prepend usings to a document
