using Microsoft.CodeAnalysis;
using System;

namespace RoseLynn.Testing;

/// <summary>
/// This dummy type acts as an interface between the two source generator interfaces
/// to allow testing capabilities only provided by <seealso cref="ISourceGenerator"/>.
/// </summary>
/// <remarks>
/// Do not ever consider using this source generator as a real generator.
/// All source generator implementation methods throw.
/// </remarks>
public sealed class InterfacingSourceGenerator : ISourceGenerator, IIncrementalGenerator
{
    void ISourceGenerator.Execute(GeneratorExecutionContext context) => throw new NotImplementedException();
    void ISourceGenerator.Initialize(GeneratorInitializationContext context) => throw new NotImplementedException();

    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context) => throw new NotImplementedException();
}
