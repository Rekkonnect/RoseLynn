# Stored Descriptor Diagnostic Analyzer

The type `StoredDescriptorDiagnosticAnalyzer` utilizes a [diagnostic descriptor storage](descriptorStorage.md) to back the supported diagnostics for that analyzer without requiring to explicitly declare them through the `SupportedDiagnostics` property.

## Declaring Analyzer

Instead of `SupportedDiagnostics`, `DiagnosticDescriptorStorage` is required to override when declaring a new analyzer class. Upon initializing a new instance of the analyzer, the supported diagnostics are automatically captured from the provided storage instance using the `GetDiagnosticDescriptors(Type)` method.

There are 2 distinct analyzer types, one for each Roslyn-supported language (C# and Visual Basic). Currently, none of them provide any specific functionality, and act decoratively.

The analyzer must also have the `DiagnosticAnalyzerAttribute` with the specified supported language names. The base classes do not automatically provide it due to Roslyn attempting to initialize an instance of the abstract classes themselves, an [open suggestion issue](https://github.com/dotnet/roslyn/issues/56340).

It is however advised to use the dedicated language-specific types as they may provide that functionality in future releases.
