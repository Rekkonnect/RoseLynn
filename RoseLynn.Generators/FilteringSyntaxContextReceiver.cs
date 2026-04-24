using Microsoft.CodeAnalysis;

namespace RoseLynn.Generators;

/// <summary>Provides an <seealso cref="ISyntaxContextReceiver"/> that filters visited syntax nodes.</summary>
public abstract class FilteringSyntaxContextReceiver : FilteringReceiverBase<GeneratorSyntaxContext>, ISyntaxContextReceiver
{
    void ISyntaxContextReceiver.OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        OnVisitSyntaxNode(context);
    }
}

/// <summary>
/// Provides an <seealso cref="ISyntaxContextReceiver"/> that filters visited syntax nodes
/// using a <seealso cref="GeneratorSyntaxContextWrapper"/>.
/// </summary>
/// <typeparam name="TContextWrapper">The type of the <seealso cref="GeneratorSyntaxContextWrapper"/> to use.</typeparam>
public abstract class FilteringSyntaxContextReceiver<TContextWrapper> : FilteringSyntaxContextReceiver
    where TContextWrapper : GeneratorSyntaxContextWrapper
{
    /// <inheritdoc/>
    protected override void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        var wrapper = CreateWrapper(context);
        wrapper.ExecuteFiltered(Filter, OnVisitFilteredSyntaxNode);
    }

    /// <inheritdoc/>
    /// <remarks>This method uses <seealso cref="Filter(TContextWrapper)"/>.</remarks>
    protected sealed override bool Filter(GeneratorSyntaxContext context)
    {
        var wrapper = CreateWrapper(context);
        return Filter(wrapper);
    }
    /// <inheritdoc/>
    /// <remarks>This method uses <seealso cref="OnVisitFilteredSyntaxNode(TContextWrapper)"/>.</remarks>
    protected sealed override void OnVisitFilteredSyntaxNode(GeneratorSyntaxContext context)
    {
        var wrapper = CreateWrapper(context);
        OnVisitFilteredSyntaxNode(wrapper);
    }

    /// <summary>Filters the visited syntax node.</summary>
    /// <param name="context">The context with the information of the visited syntax node to filter.</param>
    /// <returns>
    /// <see langword="true"/> if the visited syntax node is important for the purposes of this syntax context receiver,
    /// otherwise <see langword="false"/>.
    /// </returns>
    protected abstract bool Filter(TContextWrapper context);
    /// <summary>Acts on a filtered syntax node.</summary>
    /// <param name="context">The context with the information of the visited node that passed through the filter.</param>
    protected abstract void OnVisitFilteredSyntaxNode(TContextWrapper context);

    /// <summary>
    /// Creates a <typeparamref name="TContextWrapper"/> out of a provided <seealso cref="GeneratorSyntaxContext"/>.
    /// </summary>
    /// <param name="context">The <seealso cref="GeneratorSyntaxContext"/> to wrap.</param>
    /// <returns>An instance of <typeparamref name="TContextWrapper"/> to use.</returns>
    protected abstract TContextWrapper CreateWrapper(GeneratorSyntaxContext context);
}
