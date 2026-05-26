# Using Directive Statement Info

`UsingDirectiveStatementInfo` is a C#-focused value model for representing, sorting, and rendering `using` directives.

## What it captures

Each instance stores:

- `Kind`
- `Alias`
- `QualifiedName`

It also exposes convenience properties such as:

- `IsGlobal`
- `IsStatic`
- `DirectiveKind`
- `IdentifierSortKey`

## Creating instances

For ordinary imports:

```csharp
var globalSystemUsing = new UsingDirectiveStatementInfo(UsingDirectiveKind.GlobalUsing, "System");
var localLinqUsing = new UsingDirectiveStatementInfo(UsingDirectiveKind.Using, "System.Linq");
```

For alias directives, prefer the factories:

```csharp
var localAlias = UsingDirectiveStatementInfo.LocalAlias("X", "System");
var globalAlias = UsingDirectiveStatementInfo.GlobalAlias("C", "System.Console");
```

## Creating directives from symbol names

The type also provides factories built around `FullSymbolName`:

- `UsingForNamespaceSymbol(...)`
- `UsingForSymbol(...)`
- `UsingStaticForTypeSymbol(...)`
- `UsingAliasForSymbol(...)`

Example:

```csharp
var usingStatic = UsingDirectiveStatementInfo.UsingStaticForTypeSymbol(
    type.GetFullSymbolName()!,
    ["string"]);
```

When using the type-based factories for generic types, make sure the provided type arguments match the symbol arity.

## Rendering

`ToString()` emits valid C# syntax, including:

- `using X;`
- `global using X;`
- `using static X;`
- `using Alias = X;`

## Sorting

`UsingDirectiveStatementInfo.SortingComparer` reproduces the default RoseLynn ordering behavior, which is designed to match IDE-style grouping.

The default comparer order is:

- global before local
- importing directives before aliases
- non-static before static
- ascending identifier/alias key

You can customize the ordering by deriving from `SortingComparer` and changing `ComparerOrder`.

## Related APIs

- `UsingDirectiveStatementInfoList` for list-oriented operations and rendering
- `UsingsProviderBase.CreateForUsings(...)` for turning structured directives into a reusable provider
