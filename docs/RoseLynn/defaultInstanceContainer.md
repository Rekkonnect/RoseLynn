# Default Instance Container

The `DefaultInstanceContainer<TBase>` type provides the ability to initialize default instances of all types that inherit `TBase`, provided that each type meets some criteria.

## Defining Container Rules

The base class is abstract, meaning that a custom class inheriting the base one must be made to initialize instances of the container.

Assume a container type that initializes default instances of types inheriting `Implementation`. Types containing "Invalid" in their name will be ignored.

All types must contain a constructor with the following signature:
```csharp
new(int[] array)
```

Here's the code defining such a container:

```csharp
public sealed class ImplementationContainer : DefaultInstanceContainer<Implementation>
{
    protected override object[] GetDefaultInstanceArguments()
    {
        return new object[] { Array.Empty<int>() };
    }

    protected override bool IsValidInstanceType(Type type)
    {
        return !type.Name.Contains("Invalid");
    }
}
```

## Initializing Container Instances

Upon initializing a new container instance, all the discovered types are filtered, and for each filtered type a new default instance is created by invoking the constructor with the specified default instance arguments. This means that types outside the defining assembly can also be used, if they meet the criteria.

## Getting Default Instances

The `GetDefaultInstance` methods are used to retrieve the initialized default instances of the types that meet the criteria. If the requested type has no initialized default instance, `default` is returned instead.

In the above example, assume the following types (that all inherit from `Implementation` and provide the requested constructor):
- `ImplementationA`
- `InvalidImplementation`
- `ImplementationInvalid`
- `ImplementationB`

The types `ImplementationA` and `ImplmentationB` will have a new default instance initialized, while the rest will be ignored.
