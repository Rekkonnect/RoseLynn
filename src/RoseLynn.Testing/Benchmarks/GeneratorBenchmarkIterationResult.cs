using System;

namespace RoseLynn.Testing.Benchmarks;

[NeedsDocs]
public sealed record GeneratorBenchmarkIterationResult(
    int Index,
    double DriverMilliseconds,
    long UnforcedCollectionMemory,
    long ForcedCollectionMemory,
    long GarbageMemory)
{
    public GeneratorBenchmarkIterationResult(
        int index,
        TimeSpan driverTime,
        CollectionMemorySnapshot memoryUsage)
        : this(
            index,
            driverTime.TotalMilliseconds,
            memoryUsage.UnforcedCollectionMemory,
            memoryUsage.ForcedCollectionMemory,
            memoryUsage.GarbageMemory)
    {
    }
}
