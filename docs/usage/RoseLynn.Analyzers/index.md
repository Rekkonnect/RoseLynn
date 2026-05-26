# [RoseLynn.Analyzers](https://www.nuget.org/packages/RoseLynn.Analyzers/)

`RoseLynn.Analyzers` contains analyzer-oriented abstractions that reduce the amount of repetitive descriptor plumbing needed in Roslyn analyzer packages.

## Main documentation

- [Diagnostic Descriptor Storage](descriptorStorage.md)
- [Stored Descriptor Diagnostic Analyzer](storedDescriptorAnalyzer.md)

## Package focus

The package is intentionally small. Its main job is to centralize:

- descriptor lookup by ID
- grouping descriptors by analyzer type
- reusable analyzer base classes for C# and Visual Basic analyzer declarations
