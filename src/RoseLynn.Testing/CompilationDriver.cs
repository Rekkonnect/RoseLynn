using Microsoft.CodeAnalysis;

namespace RoseLynn.Testing;

/// <summary>Pairs a <seealso cref="Compilation"/> with its associated <seealso cref="GeneratorDriver"/> for incremental generation testing.</summary>
/// <param name="Compilation">The compilation snapshot.</param>
/// <param name="Driver">The generator driver that operates on the compilation.</param>
public record struct CompilationDriver(Compilation Compilation, GeneratorDriver Driver);
