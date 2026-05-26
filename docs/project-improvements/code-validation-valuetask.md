# Code Validation Should Return `ValueTask`

## Status

Deferred. Do not implement yet.

## Current trouble

Current code-validation abstractions force synchronous wrappers in some tests, which leads to patterns like `GetAwaiter().GetResult()`.

In TUnit-heavy test code, this works against clean async assertion flow.

## Proposed improvement

Change code-validation APIs to return `ValueTask` instead of forcing synchronous `void`-style validation entry points.

Target effect:
- allow direct async test bodies
- align better with TUnit async assertion style
- remove need for `GetAwaiter().GetResult()` in validation helpers

## Why defer

This likely causes broad breaking changes across testing abstractions and downstream call sites.

Likely impact areas:
- base diagnostic test abstractions
- code-fix test abstractions
- helper wrappers built on top of validation methods
- existing tests that assume synchronous validation entry points

## Future work outline

Possible migration path:
1. map all validation entry points returning `void`
2. design async-first replacements returning `ValueTask`
3. decide whether to keep temporary compatibility shims
4. update internal callers
5. update package consumers carefully, with release notes if public surface changes

## Reason for note

This is worth tracking now because it improves developer ergonomics and better showcases RoseLynn testing around Roslyn tooling, but implementation should wait until breaking-change budget is acceptable.
