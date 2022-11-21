using Microsoft.CodeAnalysis;
using System.Threading;

namespace RoseLynn.Generators;

/// <summary>Wraps a generator context allowing for custom information to be mapped to the specific instance.</summary>
public abstract record GeneratorContextWrapperBase
{
    /// <summary>Determines whether the additional information has been fetched.</summary>
    public bool HasFetchedInformation { get; private set; }

    /// <summary>
    /// Fetches the additional information that is to be mapped to the specified generator context.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that will be used to notify the operation's cancellation.</param>
    /// <remarks>
    /// This method is not automatically invoked, and may be used for lazily loading that additional information.
    /// If the operation is cancelled through the provided <seealso cref="CancellationToken"/>, <seealso cref="HasFetchedInformation"/>
    /// will remain <see langword="false"/>, regardless of the operations that have been successfully executed.
    /// It is then recommended to always check for <seealso cref="HasFetchedInformation"/> before retrieving any additional information
    /// that may have been stored in the instance.
    /// </remarks>
    public virtual void FetchAdditionalInformation(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        HasFetchedInformation = true;
    }
}

/// <summary>Wraps a <seealso cref="GeneratorSyntaxContext"/> allowing for custom information to be mapped to the specified context.</summary>
/// <param name="Context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
public abstract record GeneratorSyntaxContextWrapper(GeneratorSyntaxContext Context)
    : GeneratorContextWrapperBase
{ }

/// <summary>Wraps a <seealso cref="GeneratorExecutionContext"/> allowing for custom information to be mapped to the specified context.</summary>
/// <param name="Context">The <seealso cref="GeneratorExecutionContext"/> to wrap.</param>
public abstract record GeneratorExecutionContextWrapper(GeneratorExecutionContext Context)
    : GeneratorContextWrapperBase
{ }

/// <summary>Wraps a <seealso cref="GeneratorInitializationContext"/> allowing for custom information to be mapped to the specified context.</summary>
/// <param name="Context">The <seealso cref="GeneratorInitializationContext"/> to wrap.</param>
public abstract record GeneratorInitializationContextWrapper(GeneratorInitializationContext Context)
    : GeneratorContextWrapperBase
{ }

/// <summary>Wraps a <seealso cref="GeneratorPostInitializationContext"/> allowing for custom information to be mapped to the specified context.</summary>
/// <param name="Context">The <seealso cref="GeneratorPostInitializationContext"/> to wrap.</param>
public abstract record GeneratorPostInitializationContextWrapper(GeneratorPostInitializationContext Context)
    : GeneratorContextWrapperBase
{ }
