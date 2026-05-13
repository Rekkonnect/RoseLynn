using System;

namespace RoseLynn.Testing.Benchmarks;

[NeedsDocs]
public readonly record struct CollectionMemorySnapshot(
    long UnforcedCollectionMemory,
    long ForcedCollectionMemory)
{
    public long GarbageMemory => UnforcedCollectionMemory - ForcedCollectionMemory;

    public static CollectionMemorySnapshot Collect()
    {
        var unforced = GC.GetTotalMemory(false);
        var forced = GC.GetTotalMemory(true);
        return new(unforced, forced);
    }

    public static CollectionMemorySnapshot Current()
    {
        var unforced = GC.GetTotalMemory(false);
        return new(unforced, unforced);
    }

    public static CollectionMemorySnapshot Collect(bool forceCollection)
    {
        return forceCollection ? Collect() : Current();
    }
}
