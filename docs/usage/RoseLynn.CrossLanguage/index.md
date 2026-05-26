# [RoseLynn.CrossLanguage](https://www.nuget.org/packages/RoseLynn.CrossLanguage/)

`RoseLynn.CrossLanguage` is a convenience bundle package. It has no source of its own — it simply depends on both [RoseLynn.CSharp](../RoseLynn.CSharp/index.md) and [RoseLynn.VisualBasic](../RoseLynn.VisualBasic/index.md), so a single reference gives you full C# and Visual Basic coverage.

## When to use this package

- analyzers or code fixes that handle both C# and VB syntax trees
- tools that need to work across multi-language solutions
- any project that would otherwise reference both `RoseLynn.CSharp` and `RoseLynn.VisualBasic` individually

## Package contents

This package contains no compiled code. Its only purpose is to express the dependency on both language-specific packages. All APIs are documented in the individual package guides linked above.
