# [RoseLynn.Testing](https://www.nuget.org/packages/RoseLynn.Testing/)

`RoseLynn.Testing` contains helpers for analyzer tests, code-fix tests, generator tests, diagnostic markup parsing, and generator benchmarking.

## Core pages

- [Analyzer Test Fixture](analyzerTestFixture.md)
- [Diagnostic Markup Style Handler](diagnosticMarkupStyle.md)
- [Custom Diagnostic Markup Syntax](customMarkupSyntax.md)

## Additional useful APIs

### Generator tests

- `CSharpSourceGeneratorTestEx<TSourceGenerator>`
- `CSharpGeneratorDriverTest`
- `CSharpSingleGeneratorDriverTest<TSourceGenerator>`
- `GeneratorTestHelpers`
- `GeneralGeneratorTestHelpers`

### Benchmarking

- `BaseGeneratorBenchmarkTest<TSourceGenerator>`
- `GeneratorBenchmarkParameters`
- `GeneratorBenchmarkRunResults`
- `GeneratorBenchmarkIterationResult`

### Compilation/test scaffolding

- `BaseDiagnosticTests...`
- `BaseCodeFixDiagnosticTests...`
- `CSharpCompilationTest`
- `CompilationExtensions`
- `CSharpCompilationExtensions`
