using System;
using System.Threading;
using System.Threading.Tasks;

namespace RoseLynn.Testing;

/// <summary>Provides a base class for compilation-based efforts that expose specialized execution methods instead of the default test runner entry point.</summary>
public abstract class BaseCSharpCompilationEffort : BaseCSharpCompilationTest
{
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    [Obsolete("Do not invoke RunAsync on the effort; instead use the specialized methods the target class provides.", error: true)]
    protected sealed override Task RunImplAsync(CancellationToken cancellationToken)
    {
        throw new NotSupportedException("""
            Do not invoke RunAsync on the effort;
            instead use the specialized methods the target class provides.
            """);
    }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
}
