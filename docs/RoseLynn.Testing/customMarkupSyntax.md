# Custom Diagnostic Markup Syntax

## Microsoft.CodeAnalysis.Testing

### Implicit Expected Diagnostic ID

The markup syntax for the style is extended to also support not explicitly specifying the tested diagnostic ID, and implying it from the `TestedDiagnosticRule` property in an [analyzer test fixture](analyzerTestFixture.md).

The syntax is as follows:
```
{|*:<node>|}
```

Where `<node>` contains the syntax node that the diagnostic will be reported on.

For example, this is how to declare expecting a reported diagnostic on the type of the declared variable:

```csharp
{|*:int|} number = 1;
```

The specific expected diagnostic depends on the `TestedDiagnosticRule` property.
