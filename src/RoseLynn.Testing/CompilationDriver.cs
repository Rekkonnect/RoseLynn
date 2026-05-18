using Microsoft.CodeAnalysis;

namespace RoseLynn.Testing;

public record struct CompilationDriver(Compilation Compilation, GeneratorDriver Driver);
