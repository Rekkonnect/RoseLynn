namespace RoseLynn.Testing.Benchmarks;

/// <summary>Captures memory measurements before and after an optional forced garbage collection.</summary>
/// <param name="UnforcedCollectionMemory">The total managed memory reported without forcing a collection.</param>
/// <param name="ForcedCollectionMemory">The total managed memory reported after forcing a collection.</param>
public readonly record struct CollectionMemorySnapshot(
    long UnforcedCollectionMemory,
    long ForcedCollectionMemory)
{
    /// <summary>Gets the amount of memory reclaimed between the unforced and forced measurements.</summary>
    public long GarbageMemory => UnforcedCollectionMemory - ForcedCollectionMemory;

    /// <summary>Collects memory measurements before and after forcing a garbage collection.</summary>
    /// <returns>The captured memory snapshot.</returns>
    public static CollectionMemorySnapshot Collect()
    {
        var unforced = GC.GetTotalMemory(false);
        var forced = GC.GetTotalMemory(true);
        return new(unforced, forced);
    }

    /// <summary>Captures the current memory usage without forcing a garbage collection.</summary>
    /// <returns>The captured memory snapshot.</returns>
    public static CollectionMemorySnapshot Current()
    {
        var unforced = GC.GetTotalMemory(false);
        return new(unforced, unforced);
    }

    /// <summary>Collects a memory snapshot with or without forcing a garbage collection.</summary>
    /// <param name="forceCollection">Whether a forced collection should be performed before the second measurement.</param>
    /// <returns>The captured memory snapshot.</returns>
    public static CollectionMemorySnapshot Collect(bool forceCollection)
    {
        return forceCollection ? Collect() : Current();
    }
}
