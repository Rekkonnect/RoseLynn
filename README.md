# RoseLynn

Additional tools for Roslyn-based applications.

RoseLynn is a family of MIT-licensed helper libraries for analyzers, code fixes, source generators, workspaces, and analyzer tests. The packages target `netstandard2.0` and `net10.0`.

## Packages

- [RoseLynn.Core](https://www.nuget.org/packages/RoseLynn.Core/) ([usage](docs/usage/RoseLynn.Core/index.md))
- [RoseLynn.CSharp](https://www.nuget.org/packages/RoseLynn.CSharp/) ([usage](docs/usage/RoseLynn.CSharp/index.md))
- [RoseLynn.VisualBasic](https://www.nuget.org/packages/RoseLynn.VisualBasic/) ([usage](docs/usage/RoseLynn.VisualBasic/index.md))
- [RoseLynn.CrossLanguage](https://www.nuget.org/packages/RoseLynn.CrossLanguage/) ([usage](docs/usage/RoseLynn.CrossLanguage/index.md))
- [RoseLynn.Analyzers](https://www.nuget.org/packages/RoseLynn.Analyzers/) ([usage](docs/usage/RoseLynn.Analyzers/index.md))
- [RoseLynn.CodeFixes](https://www.nuget.org/packages/RoseLynn.CodeFixes/) ([usage](docs/usage/RoseLynn.CodeFixes/index.md))
- [RoseLynn.Generators](https://www.nuget.org/packages/RoseLynn.Generators/) ([usage](docs/usage/RoseLynn.Generators/index.md))
- [RoseLynn.Writing](https://www.nuget.org/packages/RoseLynn.Writing/) ([usage](docs/usage/RoseLynn.Writing/index.md))
- [RoseLynn.Testing](https://www.nuget.org/packages/RoseLynn.Testing/) ([usage](docs/usage/RoseLynn.Testing/index.md))
- [RoseLynn.Bundle.Analysis](https://www.nuget.org/packages/RoseLynn.Bundle.Analysis/) — convenience bundle for `RoseLynn.Analyzers`, `RoseLynn.CodeFixes`, and `RoseLynn.Generators`

## Installation

```shell
dotnet add package RoseLynn.Core
```

Add the package that matches the Roslyn surface you are building against. Most analyzer projects will want `RoseLynn.Analyzers` and `RoseLynn.CodeFixes`; generator projects will want `RoseLynn.Generators`; test projects will want `RoseLynn.Testing`.

## Additional information

- [Documentation](docs/index.md)
- [Changelogs](changelogs/README.md)
- [Third-party notices](THIRD-PARTY-NOTICES.md)

## License

RoseLynn is licensed under the [MIT License](LICENSE). The 0.6.x releases were also MIT-licensed; the 1.0.0 release continues that license for the split package family.
