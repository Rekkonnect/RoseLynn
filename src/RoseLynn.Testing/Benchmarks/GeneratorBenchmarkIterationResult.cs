using System;

namespace RoseLynn.Testing.Benchmarks;

/// <summary>Represents the timing and memory measurements captured for a single benchmark iteration.</summary>
/// <param name="Index">The zero-based iteration index.</param>
/// <param name="DriverMilliseconds">The elapsed generator driver time for the iteration, in milliseconds.</param>
/// <param name="UnforcedCollectionMemory">The total memory reported before forcing a collection.</param>
/// <param name="ForcedCollectionMemory">The total memory reported after forcing a collection.</param>
/// <param name="GarbageMemory">The difference between the unforced and forced memory measurements.</param>
public sealed record GeneratorBenchmarkIterationResult(
    int Index,
    double DriverMilliseconds,
    long UnforcedCollectionMemory,
    long ForcedCollectionMemory,
    long GarbageMemory)
{
    /// <summary>Creates an iteration result from a measured driver duration and memory snapshot.</summary>
    /// <param name="index">The zero-based iteration index.</param>
    /// <param name="driverTime">The elapsed generator driver time for the iteration.</param>
    /// <param name="memoryUsage">The memory snapshot captured for the iteration.</param>
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
