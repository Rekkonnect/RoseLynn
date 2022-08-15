# UsingDirectiveStatementInfo (C#-exclusive)

This class encapsulates functionality related to handling quick using directive statement information, like its kind and the qualified name. 

## Creating instances

For traditional, non-alias using directive statements, the `Alias` property is ignored, and a constructor ignoring that property is provided, which can be used like so:
```csharp
var globalSystemUsing = new UsingDirectiveStatementInfo(UsingDirectiveKind.GlobalUsing, "System");
var globalSystemLinqUsing = new UsingDirectiveStatementInfo(UsingDirectiveKind.GlobalUsing, "System.Linq");
```

For aliases, it is recommended to use the `UsingDirectiveStatementInfo.LocalAlias` and `UsingDirectiveStatementInfo.GlobalAlias` factory methods, like so:
```csharp
var aliasXToSystem = UsingDirectiveStatementInfo.LocalAlias("X", "System");
var globalAliasCToSystemConsole = UsingDirectiveStatementInfo.GlobalAlias("C", "System.Console");
```

## Usings from `FullSymbolName`

There are quick factory methods provided for easily creating usings for required components, based on their symbol names. Examples are given below:

### `UsingForNamespaceSymbol`
Assume that `targetNamespace` reflects the `INamespaceSymbol` for `System.Collections.Generic`:
```csharp
var namespaceUsing = UsingDirectiveStatementInfo.UsingForNamespaceSymbol(targetNamespace.GetFullSymbolName());
```
`namespaceUsing` will reflect a `using System.Collections.Generic;` statement.

---

### `UsingForSymbol`
Assume that `type` reflects the `ITypeSymbol` for `System.Collections.Generic.List<T>`:
```csharp
var namespaceUsing = UsingDirectiveStatementInfo.UsingForSymbol(type.GetFullSymbolName());
```
`namespaceUsing` will reflect a `using System.Collections.Generic;` statement, which is the required namespace for using `List<T>`.

---

Now, consider a `ns` variable that reflects the `INamespaceSymbol` for `System.Collections.Generic`:
```csharp
var namespaceUsing = UsingDirectiveStatementInfo.UsingForSymbol(ns.GetFullSymbolName());
```
`namespaceUsing` will reflect a `using System.Collections;` statement, which is the required namespace for using the `Generic` namespace inside of it.

---

### `UsingStaticForTypeSymbol`
Assume that `type` reflects the `ITypeSymbol` for `System.Collections.Generic.List<T>`:
```csharp
var usingStatic = UsingDirectiveStatementInfo.UsingStaticForTypeSymbol(type.GetFullSymbolName(), new[] { "string" });
```
`usingStatic` will reflect a `using static System.Collections.Generic.List<string>;` statement.

---

### `UsingAliasForSymbol`
Assume that `type` reflects the `ITypeSymbol` for `System.Collections.Generic.List<T>`:
```csharp
var usingStatic = UsingDirectiveStatementInfo.UsingAliasForSymbol(type.GetFullSymbolName(), "StringList", new[] { "string" });
```
`usingStatic` will reflect a `using StringList = System.Collections.Generic.List<string>;` statement.

In the above two examples, **it is important to match the given symbol's arity, and providing type arguments when the type is generic. Passing a null array will be considered as an empty array.**

## Sorting and comparison

The class comes with a flexible `SortingComparer` that allows customizing the order of the comparers and which order is preferred for the usings by exposing the individual comparison parts.

The default behavior is the one that matches that of the IDE, like this:
- global over local
- using and using static over using alias (`using X = Y`)
- using over using static
- ascending qualified names or aliases
  - `using F;` will go over `using G;`
  - `using A = Z;` will go over `using B = Y;`, disregarding the qualified name in the using alias

### Custom sorting

Due to the sorting comparer's flexibility, one can easily create a custom comparer that abides to a defined ordering, like in the following example:
```csharp
public class CustomSortingComparer : UsingDirectiveStatementInfo.SortingComparer
{
    public static CustomSortingComparer Instance = new();

    private CustomSortingComparer() { }

    public override IComparer<UsingDirectiveStatementInfo>[] ComparerOrder
    {
        get => new IComparer<UsingDirectiveStatementInfo>[]
        {
            DescendingIdentifierSortKey.Instance,
            AscendingDirectiveKind.Instance,
            StaticOverNonStatic.Instance,
            GlobalOverLocal.Instance,
        };
    }
}
```
And just that simply, we've created a custom comparer that applies the following comparison rules:
- descending qualified names or aliases
  - `using Z;` will go over `using A;`
  - `using Z = A;` will go over `using X = B;`, disregarding the qualified name in the using alias
- using and using static over using alias (`using X = Y`)
- using static over using
- global over local
