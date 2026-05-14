using Garyon.Objects;
using System.Reflection;

namespace RoseLynn.Testing.Benchmarks;

/// <summary>Provides helpers for extracting repository metadata for benchmark reporting.</summary>
public static class TestEffortContextHelpers
{
    /// <summary>Gets the commit SHA embedded in an assembly's informational version.</summary>
    /// <param name="assembly">The assembly whose version metadata will be inspected.</param>
    /// <returns>The commit SHA if the informational version contains one; otherwise, <see langword="null"/>.</returns>
    public static string? GetCommitSha(Assembly assembly)
    {
        var versionInfo = AppVersionInfo.InformationalVersionForAssembly(assembly);
        return versionInfo?.CommitSha;
    }
}
