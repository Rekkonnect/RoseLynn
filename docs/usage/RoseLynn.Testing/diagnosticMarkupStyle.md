# Diagnostic Markup Style Handler

`DiagnosticMarkupCodeHandler` is the central utility for working with diagnostic markup embedded in test source strings.

## What it can do

- detect marked-up diagnostic spans
- remove markup from code
- add markup to raw code at known spans
- convert markup from one framework style to another

## Built-in style

RoseLynn currently ships the static `DiagnosticMarkupCodeHandler.MicrosoftCodeAnalysis` instance, backed by `MicrosoftCodeAnalysisDiagnosticMarkupCodeHandler`.

It supports the standard `Microsoft.CodeAnalysis.Testing` forms:

- `[|node|]`
- `{|RuleId:node|}`

## Main APIs

- `GetDiagnosticMarkedUpSpans(...)`
- `RemoveMarkup(...)`
- `MarkupDiagnostic(...)`
- `MarkupDiagnostics(...)`
- `ConvertMarkup(...)`

## Custom markup styles

You can support another framework or an in-house notation by deriving from `DiagnosticMarkupCodeHandler` and implementing `GetIndicatorInfo()`. Override parsing or emission methods only when the markup format needs more than simple start/end indicators.

## Example

```csharp
var handler = DiagnosticMarkupCodeHandler.MicrosoftCodeAnalysis;
var spans = handler.GetDiagnosticMarkedUpSpans(markupCode, out var cleanCode);
```

That gives you both:

- the unmarked code
- the expected diagnostic spans, including bound diagnostic IDs when present
