# Usings Provider

The `UsingsProviderBase` type provides the ability to standarize a collection of usings that should appear before the testing code.

## Initializing Usings Provider

The `DefaultNecessaryUsings` property determines the usings that will be prepended to the provided code in the `WithUsings` methods.

An example implementation of a usings provider is the following:

```csharp
public sealed class SystemUsingsProvider : UsingsProviderBase
{
    public override string DefaultNecessaryUsings =>
@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static System.Console;
";
}
```

The usings provider does not perform any syntactical analysis on the usings. In other words, the provided string in `DefaultNecessaryUsings` is only retrieved and passed away in the `WithUsings` methods.

## Using Usings Provider

In the following example, the `SystemUsingsProvider` is used to prepend its usings on the same code, freeing the user from needing to manually copy the usings in the code.

```csharp
var provider = new SystemUsingsProvider();
var sampleCode = 
@"
public class C
{
    // assume some implementation
    public IEnumerable<int> GetValues() => null;
}
";
var fullCode = provider.WithUsings(sampleCode);
```

The resulting string that is stored in `fullCode` is:

```csharp

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static System.Console;

public class C
{
    // assume some implementation
    public IEnumerable<int> GetValues() => null;
}

```

*The resulting code snippet is not trimmed. This is due to the new lines in the verbatim string literal.*

### Upcoming Features

- Resulting code trimming
- Factory methods for creating usings
