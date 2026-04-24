using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>
/// Provides a <seealso cref="ConductorSourceGenerator"/> that uses an <seealso cref="ISyntaxContextReceiver"/>.
/// </summary>
/// <typeparam name="TContextReceiver">
/// The type of the <seealso cref="ISyntaxContextReceiver"/> that will be registered for execution.
/// </typeparam>
public abstract class ContextReceiverSourceGenerator<TContextReceiver> : ConductorSourceGenerator
    where TContextReceiver : ISyntaxContextReceiver, new()
{
    /// <inheritdoc/>
    protected override void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotificationsContext<TContextReceiver>();
    }
}
