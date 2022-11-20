namespace RoseLynn.Generators;

/// <summary>Provides a receiver that filters received instances.</summary>
public abstract class FilteringReceiverBase<TReceived>
{
    /// <summary>Performs the filtering process and acts on the received instance if it passes the filter.</summary>
    /// <param name="syntaxNode">The visited syntax node.</param>
    protected virtual void OnVisitSyntaxNode(TReceived syntaxNode)
    {
        syntaxNode.ExecuteFiltered(Filter, OnVisitFilteredSyntaxNode);
    }

    /// <summary>Filters the received instance.</summary>
    /// <param name="syntaxNode">The received instance to filter.</param>
    /// <returns>
    /// <see langword="true"/> if the received instance is important for the purposes of this receiver,
    /// otherwise <see langword="false"/>.
    /// </returns>
    protected abstract bool Filter(TReceived syntaxNode);
    /// <summary>Acts on a filtered received instance.</summary>
    /// <param name="syntaxNode">The received instance that passed through the filter.</param>
    protected abstract void OnVisitFilteredSyntaxNode(TReceived syntaxNode);
}
