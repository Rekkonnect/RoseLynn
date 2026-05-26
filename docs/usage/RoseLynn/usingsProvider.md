# Usings Provider

`UsingsProviderBase` is a lightweight helper for prepending using directives to source snippets, especially in tests.

## Core members

- `DefaultNecessaryUsings`
- `Separator`
- `WithUsings(string)`
- `WithUsings(string, string separator)`
- `UsingsProviderBase.WithUsings(string original, string usings, string separator = ...)`

## Simple custom provider

```csharp
public sealed class SystemUsingsProvider : UsingsProviderBase
{
    public override string DefaultNecessaryUsings =>
"""
using System;
using System.Collections.Generic;
using System.Linq;
""";
}
```

## Example

```csharp
var provider = new SystemUsingsProvider();
var code = "public class C { }";
var fullCode = provider.WithUsings(code);
```

## Factory helpers

For ad hoc cases, you do not need a dedicated subclass:

- `UsingsProviderBase.ForUsings(...)`
- `UsingsProviderBase.CreateForUsings(...)`
- `VariableUsingsProvider`
- `UsingsProviderBase.Default`

`CreateForUsings(...)` is especially useful when you already have a `UsingDirectiveStatementInfo` sequence and want RoseLynn to render it into a provider.

## Relationship to `UsingDirectiveStatementInfo`

If you want structured, sortable, C#-aware using directives instead of raw strings, build the directives first with [Using Directive Statement Info](usingDirectiveStatementInfo.md) and then create a provider from them.

## Notes

- this API prepends text; it does not parse or merge existing using directives
- the separator is configurable and defaults to `\r\n`
