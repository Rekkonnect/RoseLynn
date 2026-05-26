# [RoseLynn.Generators](https://www.nuget.org/packages/RoseLynn.Generators/)

`RoseLynn.Generators` contains helpers for both classic source generators and incremental generators.

## Useful APIs

### Incremental generator helpers

- `GeneratorExtensions.ForAttributeWithMetadataName(...)`
- `GeneratorExtensions.ForGroupedValuesFromAttribute(...)`
- `GeneratorExtensions.RegisterDistinctSourceOutput(...)`
- `GeneratorExtensions.WhereNotNull(...)`
- `GeneratorExtensions.RunExecutionStep(...)`
- `GeneratorExtensions.ColdStart(...)`
- `PostInitializationIncrementalGenerator`

These helpers mainly reduce boilerplate around attribute discovery, grouping, testable generator-driver execution, and post-initialization-only generators.

### Classic source generator helpers

- `GeneratorInitializationContextExtensions.RegisterForSyntaxNotifications<TReceiver>()`
- `GeneratorInitializationContextExtensions.RegisterForSyntaxNotificationsContext<TReceiver>()`
- `GeneratorExecutionContextExtensions.GetCompilationLanguage()`
- `GeneratorExecutionContextExtensions.ReportDiagnostic(...)`

### Code writing

- `CSharpCodeWriter`

`CSharpCodeWriter` helps generate structured C# code while tracking indentation, comma-separated lists, nested block scopes, type scopes, and parameter declarations from signature views.

### Driver result aggregation

- `AggregateGeneratorDriverResult`
- `AggregateGeneratorResult`
- `GeneratorDriverCompilationExecutionStep`

These types are useful in diagnostics, debugging, and benchmark scenarios where you want the driver run result and timing information together.
