# RoseLynn Family Documentation

This section contains public-facing documentation for the RoseLynn package family.

The current pages focus on the APIs that are most useful when building Roslyn analyzers, code fixes, generators, and tests around them.

## Package Guides

- [RoseLynn](RoseLynn/index.md)
- [RoseLynn.Analyzers](RoseLynn.Analyzers/index.md)
- [RoseLynn.CodeFixes](RoseLynn.CodeFixes/index.md)
- [RoseLynn.Generators](RoseLynn.Generators/index.md)
- [RoseLynn.Testing](RoseLynn.Testing/index.md)

## Notes

- The core package still primarily targets C# analysis workflows.
- Some analyzer-facing abstractions already have distinct C# and Visual Basic entry points where Roslyn expects them.
- When behavior is subtle, prefer the XML docs in the shipped APIs as the source of truth.
