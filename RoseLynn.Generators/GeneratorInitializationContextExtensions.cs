using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Provides helper extensions for the <seealso cref="GeneratorInitializationContext"/> type.</summary>
public static class GeneratorInitializationContextExtensions
{
    /// <summary>Registers a constructable type instance for syntax notifications, provided a <seealso cref="GeneratorInitializationContext"/>.</summary>
    /// <typeparam name="TReceiver">The constructable <seealso cref="ISyntaxContextReceiver"/> type.</typeparam>
    /// <param name="context">The <seealso cref="GeneratorInitializationContext"/> to register the constructed type instance for receiving syntax notifications.</param>
    public static void RegisterForSyntaxNotificationsContext<TReceiver>(this GeneratorInitializationContext context)
        where TReceiver : ISyntaxContextReceiver, new()
    {
        context.RegisterForSyntaxNotifications(static () => new TReceiver());
    }
    /// <summary>Registers a constructable type instance for syntax notifications, provided a <seealso cref="GeneratorInitializationContext"/>.</summary>
    /// <typeparam name="TReceiver">The constructable <seealso cref="ISyntaxReceiver"/> type.</typeparam>
    /// <param name="context">The <seealso cref="GeneratorInitializationContext"/> to register the constructed type instance for receiving syntax notifications.</param>
    public static void RegisterForSyntaxNotifications<TReceiver>(this GeneratorInitializationContext context)
        where TReceiver : ISyntaxReceiver, new()
    {
        context.RegisterForSyntaxNotifications(static () => new TReceiver());
    }
}
