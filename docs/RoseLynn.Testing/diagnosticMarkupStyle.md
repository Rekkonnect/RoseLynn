# Diagnostic Markup Style Handler

The `DiagnosticMarkupCodeHandler` type provides the ability to handle diagnostic markup syntax in code strings used in test fixtures.

## Features

- Get diagnostic spans
- Remove all markup indicators from code
- Add markup indicators at specified locations
- Convert between styles

## Provided Supported Styles

- [Microsoft.CodeAnalysis.Testing](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Analyzer.Testing/)
- [Gu.Roslyn.Asserts](https://www.nuget.org/packages/Gu.Roslyn.Asserts/)

## Supporting Custom Markup Style

The handler type is extensible so that it supports defining custom markup styles, much like how the supported ones are implemented.

Let's consider an example markup style that encloses the node within `$`, meaning that a node marked with a diagnostic appears as `$<node>$`:

```csharp
public sealed class CustomDiagnosticMarkupCodeHandler : DiagnosticMarkupCodeHandler
{
    /// <inheritdoc/>
    protected override DiagnosticIndicatorInfo GetIndicatorInfo()
    {
        return DiagnosticIndicatorInfo.UnboundDiagnostic("$", "$");
    }

    /// <inheritdoc/>
    public override string RemoveMarkup(string code)
    {
        return code.Remove("$");
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override string MarkupDiagnostic(string rawStringNode)
    {
        return $"${rawStringNode}$";
    }
}
```

Normally, the `RemoveMarkup` and the `MarkupDiagnostic` methods do not have to be overridden, however, they can trivially improve performance slightly. As general advice, it is preferred to only implement the `GetIndicatorInfo` method (for the above example), which resorts to using the base methods that already account for the declared diagnostic indicators of all types.
