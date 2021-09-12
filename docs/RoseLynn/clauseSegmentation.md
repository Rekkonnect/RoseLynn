# Type Parameter Constraint Clause Segmentation

The type `TypeParameterConstraintClauseSegmentation` provides the ability to organize constraint clauses and easily modify them while maintaining syntax order.

## Features

### Building

- Add new constraints from `TypeParameterConstraintSyntax` nodes
  - Specifically add new type constraints from `TypeConsrtaintSyntax` nodes
- Manually modify the constraint nodes for each of the segments in the constraint clause

### Exporting

- Create a `SeparatedSyntaxList` out of the current constraint clause
- Create a new `TypeParameterConstraintClauseSyntax` for a given `IdentifierNameSyntax`, or a `string` representing one
- Modify a `TypeParameterConstraintClauseSyntax` with the `WithConstraints` method, applying the constraints currently contained in the instance

## Future Considerations

### Additional Implementations

- Individually retrieve the syntax nodes related to the type parameter symbols and the interface symbols respectively
- Easily adjust fixed constraints (`class`, `struct`, `Enum`, etc.) without requiring the user create blank nodes
- Improvements revolving around usage of the semantic model

### Breaking Changes

For consistency across the rest of the API, the type might be renamed to `...Storage` in the future. Do keep in mind that RoseLynn is currently not in a concrete build state, as hinted by the current version numbers.