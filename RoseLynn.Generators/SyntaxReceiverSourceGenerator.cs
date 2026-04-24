using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>
/// Provides a <seealso cref="ConductorSourceGenerator"/> that uses an <seealso cref="ISyntaxReceiver"/>.
/// </summary>
/// <typeparam name="TSyntaxReceiver">
/// The type of the <seealso cref="ISyntaxReceiver"/> that will be registered for execution.
/// </typeparam>
public abstract class SyntaxReceiverSourceGenerator<TSyntaxReceiver> : ConductorSourceGenerator
    where TSyntaxReceiver : ISyntaxReceiver, new()
{
    /// <inheritdoc/>
    protected override void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications<TSyntaxReceiver>();
    }
}
