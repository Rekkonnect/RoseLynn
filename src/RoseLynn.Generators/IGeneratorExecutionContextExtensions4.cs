using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>
/// Provides handy extensions for the <see cref="IGeneratorExecutionContext"/> type.
/// </summary>
/// <remarks>
/// Adds on top of the extensions provided in
/// <seealso cref="IGeneratorExecutionContextExtensions"/>.
/// </remarks>
public static class IGeneratorExecutionContextExtensions4
{
    /// <summary>
    /// Creates a <seealso cref="RoseSourceProductionContext"/> wrapping the given
    /// <seealso cref="SourceProductionContext"/>.
    /// </summary>
    /// <param name="context">
    /// The <seealso cref="SourceProductionContext"/> to wrap into the resulting
    /// <seealso cref="RoseSourceProductionContext"/>.
    /// </param>
    /// <returns>
    /// A <seealso cref="RoseSourceProductionContext"/> wrapping the given
    /// <seealso cref="SourceProductionContext"/>.
    /// </returns>
    public static RoseSourceProductionContext RoseContext(this SourceProductionContext context)
    {
        return new(context);
    }
}
