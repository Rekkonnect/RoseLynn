using System;

namespace RoseLynn.Testing.Benchmarks;

/// <summary>Defines the stopping conditions and collection options for a generator benchmark run.</summary>
public record GeneratorBenchmarkParameters
{
    /// <summary>Gets or sets the minimum total generator driver time to collect before stopping.</summary>
    public TimeSpan MinDriverTime { get; set; }
    /// <summary>Gets or sets the maximum total generator driver time allowed before stopping.</summary>
    public TimeSpan MaxDriverTime { get; set; }

    /// <summary>Gets or sets the minimum number of iterations to execute.</summary>
    public int MinIterations { get; set; }
    /// <summary>Gets or sets the maximum number of iterations to execute.</summary>
    public int MaxIterations { get; set; }

    /// <summary>Gets or sets the maximum forced-collection memory value allowed before stopping.</summary>
    public long MaxMemory { get; set; } = long.MaxValue;

    /// <summary>Gets or sets whether each iteration should force a garbage collection before measuring memory.</summary>
    public bool ForceGarbageCollection { get; set; }

    /// <summary>Gets or sets <see cref="MinDriverTime"/> as a value in seconds.</summary>
    public double MinDriverSeconds
    {
        get => MinDriverTime.TotalSeconds;
        set => MinDriverTime = TimeSpan.FromSeconds(value);
    }

    /// <summary>Gets or sets <see cref="MaxDriverTime"/> as a value in seconds.</summary>
    public double MaxDriverSeconds
    {
        get => MaxDriverTime.TotalSeconds;
        set => MaxDriverTime = TimeSpan.FromSeconds(value);
    }
}
