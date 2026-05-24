using System.Runtime.CompilerServices;

namespace RoseLynn;

public static class KnownSymbolNameMatches
{
    // System
    public static QualifiedSymbolNameMatch SystemType
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Type),
            "System");

    // System.Linq
    public static QualifiedSymbolNameMatch SystemLinqParallelQuery
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(ParallelQuery),
            "System.Linq");

    public static QualifiedSymbolNameMatch SystemLinqParallelEnumerable
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(ParallelEnumerable),
            "System.Linq");

    // System.Runtime.CompilerServices
    public static QualifiedSymbolNameMatch SystemRuntimeCompilerServicesICriticalNotifyCompletion
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(ICriticalNotifyCompletion),
            "System.Runtime.CompilerServices");

    public static QualifiedSymbolNameMatch SystemRuntimeCompilerServicesINotifyCompletion
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(INotifyCompletion),
            "System.Runtime.CompilerServices");

    // System.Threading
    public static QualifiedSymbolNameMatch SystemThreadingLock
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            "Lock",
            "System.Threading");

    public static QualifiedSymbolNameMatch SystemThreadingMonitor
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Monitor),
            "System.Threading");

    public static QualifiedSymbolNameMatch SystemThreadingThread
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Thread),
            "System.Threading");

    public static QualifiedSymbolNameMatch SystemThreadingSemaphoreSlim
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(SemaphoreSlim),
            "System.Threading");

    public static QualifiedSymbolNameMatch SystemThreadingWaitHandle
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(WaitHandle),
            "System.Threading");

    // System.Threading.Tasks
    public static QualifiedSymbolNameMatch SystemThreadingTasksParallel
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Parallel),
            "System.Threading.Tasks");

    public static QualifiedSymbolNameMatch SystemThreadingTasksTask
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Task),
            "System.Threading.Tasks");
}
