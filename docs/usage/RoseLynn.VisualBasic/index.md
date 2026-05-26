# [RoseLynn.VisualBasic](https://www.nuget.org/packages/RoseLynn.VisualBasic/)

`RoseLynn.VisualBasic` extends [RoseLynn.Core](../RoseLynn.Core/index.md) with Visual Basic-specific helpers. It carries `Microsoft.CodeAnalysis.VisualBasic` as a private dependency so consumers are not forced to take it transitively.

## Recommended Starting Points

- `KeywordIdentifierExtensions` for checking and handling VB keyword identifiers that require escaping

## Notes

- This package mirrors the design of [RoseLynn.CSharp](../RoseLynn.CSharp/index.md); VB-specific utilities follow the same patterns as their C# counterparts where applicable.
- For projects that target both C# and Visual Basic analysis, consider depending on [RoseLynn.CrossLanguage](../RoseLynn.CrossLanguage/index.md) instead.
