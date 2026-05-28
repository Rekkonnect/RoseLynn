using System.Runtime.CompilerServices;

namespace RoseLynn.Core;

/// <summary>Provides pre-built <seealso cref="QualifiedSymbolNameMatch"/> instances for well-known BCL types.</summary>
public static class KnownSymbolNameMatches
{
    // System
    /// <summary>Match for <see cref="System.Type"/>.</summary>
    public static QualifiedSymbolNameMatch SystemType
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Type),
            "System");

    // System.Linq
    /// <summary>Match for <see cref="System.Linq.ParallelQuery"/>.</summary>
    public static QualifiedSymbolNameMatch SystemLinqParallelQuery
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(ParallelQuery),
            "System.Linq");

    /// <summary>Match for <see cref="System.Linq.ParallelEnumerable"/>.</summary>
    public static QualifiedSymbolNameMatch SystemLinqParallelEnumerable
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(ParallelEnumerable),
            "System.Linq");

    // System.Runtime.CompilerServices
    /// <summary>Match for <see cref="System.Runtime.CompilerServices.ICriticalNotifyCompletion"/>.</summary>
    public static QualifiedSymbolNameMatch SystemRuntimeCompilerServicesICriticalNotifyCompletion
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(ICriticalNotifyCompletion),
            "System.Runtime.CompilerServices");

    /// <summary>Match for <see cref="System.Runtime.CompilerServices.INotifyCompletion"/>.</summary>
    public static QualifiedSymbolNameMatch SystemRuntimeCompilerServicesINotifyCompletion
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(INotifyCompletion),
            "System.Runtime.CompilerServices");

    // System.Threading
    /// <summary>Match for <c>System.Threading.Lock</c>.</summary>
    public static QualifiedSymbolNameMatch SystemThreadingLock
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            "Lock",
            "System.Threading");

    /// <summary>Match for <see cref="System.Threading.Monitor"/>.</summary>
    public static QualifiedSymbolNameMatch SystemThreadingMonitor
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Monitor),
            "System.Threading");

    /// <summary>Match for <see cref="System.Threading.Thread"/>.</summary>
    public static QualifiedSymbolNameMatch SystemThreadingThread
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Thread),
            "System.Threading");

    /// <summary>Match for <see cref="System.Threading.SemaphoreSlim"/>.</summary>
    public static QualifiedSymbolNameMatch SystemThreadingSemaphoreSlim
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(SemaphoreSlim),
            "System.Threading");

    /// <summary>Match for <see cref="System.Threading.WaitHandle"/>.</summary>
    public static QualifiedSymbolNameMatch SystemThreadingWaitHandle
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(WaitHandle),
            "System.Threading");

    // System.Threading.Tasks
    /// <summary>Match for <see cref="System.Threading.Tasks.Parallel"/>.</summary>
    public static QualifiedSymbolNameMatch SystemThreadingTasksParallel
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Parallel),
            "System.Threading.Tasks");

    /// <summary>Match for <see cref="System.Threading.Tasks.Task"/>.</summary>
    public static QualifiedSymbolNameMatch SystemThreadingTasksTask
        => QualifiedSymbolNameMatch.WithNameAndNamespaces(
            nameof(Task),
            "System.Threading.Tasks");
}
