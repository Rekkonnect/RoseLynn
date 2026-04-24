using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

#nullable enable annotations

/// <summary>
/// Provides an <seealso cref="ISourceGenerator"/> based on a <seealso cref="GeneratorExecutionConductor"/>.
/// </summary>
public abstract class ConductorSourceGenerator : ISourceGenerator
{
    /// <summary>
    /// Gets the <seealso cref="GeneratorExecutionConductor"/> that will have been initialized before every
    /// invocation of <seealso cref="Execute"/>.
    /// </summary>
    protected GeneratorExecutionConductor ExecutionConductor { get; private set; }

    void ISourceGenerator.Execute(GeneratorExecutionContext context)
    {
        ExecutionConductor = new(context);
        Execute();
        ExecutionConductor.FinalizeClearGeneratorExecution();
    }

    /// <summary>
    /// Executes the generation of the source.
    /// </summary>
    /// <remarks>
    /// This method is invoked after the <see cref="ExecutionConductor"/> is initialized.
    /// The execution operation can be based on the property instead of relying on the passed context parameter.
    /// </remarks>
    protected abstract void Execute();

    void ISourceGenerator.Initialize(GeneratorInitializationContext context) => Initialize(context);

    /// <summary>
    /// Initializes the execution of the generator based on a <seealso cref="GeneratorInitializationContext"/>.
    /// </summary>
    protected abstract void Initialize(GeneratorInitializationContext context);
}
