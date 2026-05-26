# [RoseLynn.CSharp](https://www.nuget.org/packages/RoseLynn.CSharp/)

`RoseLynn.CSharp` extends [RoseLynn.Core](../RoseLynn.Core/index.md) with C#-specific helpers. It carries `Microsoft.CodeAnalysis.CSharp` as a private dependency so consumers are not forced to take it transitively.

## Recommended Starting Points

### Syntax helpers

- [Type Parameter Constraint Clause Segmentation](clauseSegmentation.md)
- [Using Directive Statement Info](usingDirectiveStatementInfo.md)
- [Usings Provider](usingsProvider.md)

### Symbols and names

- [Symbol Name Helpers](symbolNames.md)
- [Signature Views](signatureViews.md)

## Other Useful Areas

- `SyntaxFactory`-backed extension methods for constructing and rewriting syntax nodes
- Attribute helpers for reading and creating C# attribute syntax
- Type and member declaration helpers for common declaration patterns
- `IMethodSymbol` extensions including primary constructor detection
- `IPropertyReferenceOperation` extensions for accessing property syntax nodes
- Compilation extensions including `WithNullableContextOptions`
