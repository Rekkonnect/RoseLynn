# Custom Diagnostic Markup Syntax

RoseLynn extends the `Microsoft.CodeAnalysis.Testing` markup shape with a shorthand that is convenient in rule-specific fixtures.

## Implicit expected diagnostic ID

Instead of always repeating the rule ID, a fixture can use:

```text
{|*:<node>|}
```

The `*` means "use the fixture's `TestedDiagnosticRule`".

## Example

```csharp
{|*:int|} number = 1;
```

When the surrounding fixture resolves `TestedDiagnosticRule` to `RSLN0001`, the markup is treated as if that rule had been written explicitly.

## When this is useful

- large rule-focused test suites
- fixtures where every marked diagnostic is expected to be the same rule
- reducing noisy repeated IDs in test source strings
