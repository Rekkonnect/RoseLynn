# [RoseLynn](https://www.nuget.org/packages/RoseLynn/)

The RoseLynn package contains various abstractions and implementations common for various components of the Microsoft.CodeAnalysis package. It also contains utilities that were created in the process of implementing those abstractions. 

## Overview

### Syntax Tree Implementations
- [Type Parameter Constraint Clause Segmentation](clauseSegmentation.md)

### Metadata Loading Mechanisms
- [Associated Property Container](associatedPropertyContainer.md)
- [Default Instance Container](defaultInstanceContainer.md)
- `LoadedAssemblyInformation` provides access to a collection of *all* the available and loaded types in the currently executing program, which ranges in tens of thousands. This should be wisely used.

## APIs

### General

- Extensions
- `MetadataReference` Factory
- Syntax Factory Methods

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
