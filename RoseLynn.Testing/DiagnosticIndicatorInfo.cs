#nullable enable

namespace RoseLynn.Testing;

/// <summary>Contains information about the diagnostic indicators of a diagnostic markup style.</summary>
public abstract class DiagnosticIndicatorInfo
{
    /// <summary>The string indicator that a diagnostic is expected to start at that point.</summary>
    public abstract string Start { get; }
    /// <summary>The string indicator that a diagnostic is expected to end at that point.</summary>
    /// <remarks>Specify <see langword="null"/> or <seealso cref="string.Empty"/> if the framework does not support ending indicators.</remarks>
    public abstract string? End { get; }
    /// <summary>The string indicator that a bound diagnostic with the specified diagnostic ID is expected to start at that point.</summary>
    /// <remarks>Specify <see langword="null"/> or <seealso cref="string.Empty"/> if the framework does not support bound diagnostic indicators.</remarks>
    public abstract string? BoundStart { get; }
    /// <summary>The string indicator that a bound diagnostic with the specified diagnostic ID is expected to end at that point.</summary>
    /// <remarks>Specify <see langword="null"/> or <seealso cref="string.Empty"/> if the framework does not support bound diagnostic indicators.</remarks>
    public abstract string? BoundEnd { get; }

    /// <summary>Initializes a new instance of <seealso cref="DiagnosticIndicatorInfo"/> with only the starting part of an unbound diagnostic indicator.</summary>
    /// <param name="start">The starting part of the unbound diagnostic indicator.</param>
    /// <returns>The <seealso cref="DiagnosticIndicatorInfo"/> instance that only contains the provided starting unbound diagnostic indicator.</returns>
    public static DiagnosticIndicatorInfo StartUnboundDiagnostic(string start) => new Constructed(start);

    /// <summary>Initializes a new instance of <seealso cref="DiagnosticIndicatorInfo"/> with only the unbound diagnostic indicator.</summary>
    /// <param name="start">The starting part of the unbound diagnostic indicator.</param>
    /// <param name="end">The ending part of the unbound diagnostic indicator.</param>
    /// <returns>The <seealso cref="DiagnosticIndicatorInfo"/> instance that only contains the provided unbound diagnostic indicator parts.</returns>
    public static DiagnosticIndicatorInfo UnboundDiagnostic(string start, string end) => new Constructed(start, end);

    /// <summary>Initializes a new instance of <seealso cref="DiagnosticIndicatorInfo"/> with both the unbound and the bound diagnostic indicators.</summary>
    /// <param name="start">The starting part of the unbound diagnostic indicator.</param>
    /// <param name="end">The ending part of the unbound diagnostic indicator.</param>
    /// <param name="boundStart">The starting part of the bound diagnostic indicator.</param>
    /// <param name="boundEnd">The ending part of the bound diagnostic indicator.</param>
    /// <returns>The <seealso cref="DiagnosticIndicatorInfo"/> instance that contains the provided unbound and bound diagnostic indicators.</returns>
    public static DiagnosticIndicatorInfo AnyDiagnostic(string start, string end, string boundStart, string boundEnd) => new Constructed(start, end, boundStart, boundEnd);

    private sealed class Constructed : DiagnosticIndicatorInfo
    {
        public override string Start { get; }
        public override string? End { get; }
        public override string? BoundStart { get; }
        public override string? BoundEnd { get; }

        public Constructed(string start, string? end = null, string? boundStart = null, string? boundEnd = null)
        {
            Start = start;
            End = end;
            BoundStart = boundStart;
            BoundEnd = boundEnd;
        }
    }
}
