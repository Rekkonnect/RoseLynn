# Type Parameter Constraint Clause Segmentation

`TypeParameterConstraintClauseSegmentation` helps inspect and rebuild C# generic constraint clauses while preserving C# ordering rules.

## What it models

The segmentation breaks a constraint clause into the slots that matter for valid output:

- `KeywordOrClassConstraint`
- `DelegateOrEnumConstraint`
- `InterfaceOrTypeParameterConstraints`
- `NewConstraint`

## Construction

You can build it from either:

- a `TypeParameterConstraintClauseSyntax`
- an existing sequence of `TypeParameterConstraintSyntax`

Both constructors take a `SemanticModel`, because type constraints need semantic classification to distinguish class, interface, enum/delegate, and type-parameter cases correctly.

## Main operations

- `AddConstraint(...)`
- `AddConstraints(...)`
- `AddTypeConstraint(...)`
- `ToSeparatedSyntaxList()`
- `ToConstraintClause(string)`
- `ToConstraintClause(IdentifierNameSyntax)`
- `WithTheseConstraints(...)`

## Typical use

Use this type when you need to rewrite or append generic constraints without manually reimplementing C# constraint ordering rules.

## Current limitation

Interface constraints and type-parameter constraints currently share one collection. If you need those categories separately, that is still future work in the implementation.
