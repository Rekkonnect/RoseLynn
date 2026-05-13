using System;

namespace RoseLynn.Testing.Benchmarks;

[NeedsDocs]
public record GeneratorBenchmarkParameters
{
    public TimeSpan MinDriverTime { get; set; }
    public TimeSpan MaxDriverTime { get; set; }

    public int MinIterations { get; set; }
    public int MaxIterations { get; set; }

    public long MaxMemory { get; set; } = long.MaxValue;

    public bool ForceGarbageCollection { get; set; }

    public double MinDriverSeconds
    {
        get => MinDriverTime.TotalSeconds;
        set => MinDriverTime = TimeSpan.FromSeconds(value);
    }

    public double MaxDriverSeconds
    {
        get => MaxDriverTime.TotalSeconds;
        set => MaxDriverTime = TimeSpan.FromSeconds(value);
    }
}
