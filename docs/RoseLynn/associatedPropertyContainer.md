# Associated Property Container

The `AssociatedPropertyContainer` type provides a mechanism to store property associativity information with specific enum values.

## Defining Associations

The first step is to define an attribute class that inherits the `IAssociatedEnumValueAttribute<T>` interface, let the following example:

```csharp
public class ExampleAssociativityAttribute : IAssociatedEnumValueAttribute<ExampleEnum>
{
    public ExampleEnum AssociatedValue { get; }

    public ExampleAssociativityAttribute(ExampleEnum value) => AssociatedValue = value;
}
```

Then, declare the desired properties with the attributes associating them to chosen values:

```csharp
public enum ExampleEnum
{
    Valid,
    Invalid,
    Unknown
}

public class ExampleType
{
    [ExampleAssociativity(ExampleEnum.Valid)]
    public string Property1 { get; set; }
    [ExampleAssociativity(ExampleEnum.Invalid)]
    public string Property2 { get; set; }
    [ExampleAssociativity(ExampleEnum.Unknown)]
    public string Property3 { get; set; }
}
```

**NOTE**: every property cannot be associated to more than one distinct values from the same enum type (this restriction does not apply for different enum types). Also, every enum value cannot be associated to more than one properties.

## Using Associations

After having declared the components and the associations, a container instance is ready to be initialized.

```csharp
var container = new AssociatedPropertyContainer(typeof(ExampleType));
```

Now, after having initialized the container, the associated property for a given enum value can be retrieved, like so:

```csharp
var associatedProperty = container.GetAssociatedProperty(ExampleEnum.Valid);
// The returned value is a PropertyInfo object representing the Property1 property
```
