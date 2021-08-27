#nullable enable

namespace RoseLynn.Testing
{
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

        public static DiagnosticIndicatorInfo StartUnboundDiagnostic(string start) => new Constructed(start);
        public static DiagnosticIndicatorInfo UnboundDiagnostic(string start, string end) => new Constructed(start, end);
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
}
