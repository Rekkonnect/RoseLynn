using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>
/// Provides an incremental generator implementation that only executes in the post-initialization
/// context of the incremental generation. It is mainly inspired by the need for developing 
/// generators for repetitive boilerplate code that does not rely on user input in any way.
/// </summary>
public abstract class PostInitializationIncrementalGenerator : IIncrementalGenerator
{
    /// <summary>
    /// The execution of the incremental generator. This is the only method that is registered
    /// for execution in the post-initialization context from this incremental geneartor, by
    /// default.
    /// </summary>
    /// <param name="context">The post-initialization context on which to execute.</param>
    protected abstract void Execute(IncrementalGeneratorPostInitializationContext context);

    /// <summary>
    /// Initializes the presence of the incremental generator in the compilation context.
    /// It registers the <seealso cref="Execute(IncrementalGeneratorPostInitializationContext)"/>
    /// method in the post-initialization phase.
    /// </summary>
    /// <param name="context">The generation context on which to initialize.</param>
    /// <remarks>
    /// Avoid overriding this method unless explicitly needed for missing configuration.
    /// Always ensure that the base method is invoked, as it registers the
    /// <seealso cref="Execute(IncrementalGeneratorPostInitializationContext)"/>
    /// method for execution in the post-initialization phase.
    /// </remarks>
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(Execute);
    }
}
