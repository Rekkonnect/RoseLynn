# [RoseLynn](https://www.nuget.org/packages/RoseLynn/)

The RoseLynn package contains various abstractions and implementations common for various components of the Microsoft.CodeAnalysis package. It also contains utilities that were created in the process of implementing those abstractions. 

## Overview

### Syntax Tree Implementations
- [Type Parameter Constraint Clause Segmentation](clauseSegmentation.md)
- `IdentifierWithArity`
- `FullSymbolName`, containing information about the names of the containers of a symbol, useful for correctly resolving naming collisions
- [Using Directive Statement Info](usingDirectiveStatementInfo.md)
- [Usings Provider](usingsProvider.md)

### Semantics
- `CachedInfrequentSpecialSymbols`, containing infrequently used special symbols, such as the destructor `IMethodSymbol` of a `INamedTypeSymbol`.
- `AttributeListTarget`, representing the target of an `AttributeListSyntax`
- `TypeDeclarationInfo`

## APIs

### General

- Extensions
- Factories
  - `MetadataReference`
  - `SyntaxNode`
  - `Location`
- `AnalysisContextActionRegistrations`

### Syntax & Symbol Generalization
- `IdentifiableMemberDeclarationSyntaxExtensions`<br/>
  Contains extensions for `MemberDeclarationSyntax` nodes that may have an identifier. All extensions only apply to nodes of type (for unsupported types, default values are returned):
  - `BaseTypeDeclarationSyntax`
  - `DelegateDeclarationSyntax`
  - `MethodDeclarationSyntax`
  - `ConstructorDeclarationSyntax`
  - `PropertyDeclarationSyntax`
  - `EventDeclarationSyntax`
- `TypeParameterizableMemberDeclarationSyntaxExtensions`<br/>
  Contains extensions for `MemberDeclarationSyntax` nodes that may be generic. All extensions only apply to nodes of type (for unsupported types, default values are returned):
  - `TypeDeclarationSyntax`
  - `DelegateDeclarationSyntax`
  - `MethodDeclarationSyntax`
