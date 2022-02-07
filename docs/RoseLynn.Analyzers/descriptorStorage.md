# Diagnostic Descriptor Storage

The type `DiagnosticDescriptorStorageBase` provides the ability to store all the `DiagnosticDescriptor` instances that the analyzer makes use of, that have the same ID prefix.

Descriptor storages do not support diagnostic IDs ending in not 4 digits. The prefix may be of variable length, but the suffix of the ID must be 4 decimal digits.

## Initializing Storage

Creating a storage requires creating a new sealed class inheriting `DiagnosticDescriptorStorageBase` and overriding the required members.

- `BaseRuleDocsURI`: a string representing the base URI for the documentation reference
- `DiagnosticIDPrefix`: the prefix of the rule IDs this storage will store
- `ResourceManager`: the `ResourceManager` that contains the resource strings for the `DiagnosticDescriptor` instances that will be created

## Storing Descriptors

To store a `DiagnosticDescriptor`, call the `CreateDiagnosticDescriptor` method which will not only create the new `DiagnosticDescriptor` instance with the provided properties, but also store it. 

For example:
```csharp
CreateDiagnosticDescriptor(0001, "Example Category", DiagnosticSeverity.Error, typeof(ExampleAnalyzer));
```

will create and store a new diagnostic with rule ID `{Prefix}0001`, with category "Example Category", have a severity of `Error` and be assigned to being reported by `ExampleAnalyzer`.

The storage supports setting a default analyzer for each created `DiagnosticDescriptor`. For initialization of the storage with several diagnostics, it can be especially handy, as demonstrated from this [mocking](../../RoseLynn.Analyzers.Test/MockedResources/MockStorage.cs) example:

```csharp
public const string MockCategory = "Mock";

protected override string DiagnosticIDPrefix => "MOCK";

// ...

public MockStorage()
{
    SetDefaultDiagnosticAnalyzer<MockAnalyzer>();

    CreateDiagnosticDescriptor(0001, MockCategory, DiagnosticSeverity.Error);
    CreateDiagnosticDescriptor(0002, MockCategory, DiagnosticSeverity.Hidden);
    CreateDiagnosticDescriptor(1001, MockCategory, DiagnosticSeverity.Info);
    
    SetDefaultDiagnosticAnalyzer<AnotherMockAnalyzer>();

    CreateDiagnosticDescriptor(2001, MockCategory, DiagnosticSeverity.Info);
}
```

in the above example, the diagnostics `MOCK0001`, `MOCK0002` and `MOCK1001` are declared as being reported by `MockAnalyzer`, while `MOCK2001` is assigned to `AnotherMockAnalyzer`.

The storage does not support multiple analyzers reporting the same diagnostic. Only one analyzer can therefore claim to report each diagnostic.

Additionally, categories can be mapped to default `DiagnosticSeverity` values, like so:

```csharp
public const string MappedCategory = "Mapped";

protected override DiagnosticSeverity? GetDefaultSeverity(string category)
{
    return category switch
    {
        MappedCategory => DiagnosticSeverity.Warning,
        _ => null,
    };
}
```

This enables using the simpler `CreateDiagnosticDescriptor` overload, which only accepts the ID and the category of the `DiagnosticDescriptor`:

```csharp
CreateDiagnosticDescriptor(3000, MappedCategory);
```

As a result, the created `DiagnosticDescriptor` will use the default `DiagnosticSeverity` for the `MappedCategory`, which is `DiagnosticSeverity.Warning`. This is still overridable by explicitly specifying the `DiagnosticSeverity` for the individual `DiagnosticDescriptor`, as usual:

```csharp
CreateDiagnosticDescriptor(3001, MappedCategory, DiagnosticSeverity.Info);
```

The `DiagnosticDescriptor` with ID 3001 will have `DiagnosticSeverity.Info`, instead of the default `DiagnosticSeverity.Warning`, as mapped. 3000 will retain its `DiagnosticSeverity.Warning`, and all other `DiagnosticDescriptor` instances will do so, unless explicitly specified otherwise.

### Storage Mechanism

The storage internally uses a bucket system that groups rule IDs based on their major and minor parts, as demonstrated:
```
  C S 0 1 0 1
 |   |   |   |
  PRE MAJ MIN

PRE = prefix
MAJ = major
MIN = minor
```
Due to the storage's structure, it is best to use as few different major groups as possible. For example, assume some diagnostics are implemented like so:
- CS0001
- CS0002
- CS0101 - something related to CS0001
- CS0102 - something related to CS0101

The better solution is to name them with the same major group:
- CS0001
- CS0002
- CS0011, or CS0003
- CS0012, or CS0004

The reasoning behind this is that a 100-element array is created for each major  group, and multiple analyzers could easily stack up and use much excess memory.

## Providing Descriptor Help Strings

Strings are retrieved through the provided `ResourceManager`. Most of the times, this is the instance that is returned from the `ResourceManager` property of the backing class created for the `.resx` resource file in the project. Such a file should contain resource strings for each diagnostic that is supported and stored in the storage.

Every diagnostic makes use of at least 2 resource strings:
- `{RuleID}_Title`
- `{RuleID}_Description` (optional)
- `{RuleID}_MessageFormat`

For example, the diagnostic with rule ID CS0101 requires the resource strings:
- `CS0101_Title`
- `CS0101_MessageFormat`

`CS0101_Description` is not mandatory. Not having created such a resource will not cause any issues.

## Retrieving Descriptors

The storage provides a `this` accessor:
```csharp
var storage = MockStorage.Instance;
var diagnostic0001 = storage[0001]; // returns the diagnostic with ID MOCK0001
var diagnostic0002 = storage["MOCK0002"]; // returns the diagnostic with ID MOCK0002

var invalid = storage["INV1340"]; // returns null, since the diagnostic ID has a different prefix than the one it supports
```

There also is the ability to retrieve all the diagnostics that a specific analyzer supports, by using the `GetDiagnosticDescriptors` methods.

Additionally, it is possible to construct a dictionary mapping every analyzer type to all of its stored supported diagnostics with `GetDiagnosticDescriptorsByAnalyzersImmutable`.
