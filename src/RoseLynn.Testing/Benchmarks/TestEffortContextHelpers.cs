using Garyon.Objects;
using System.Reflection;

namespace RoseLynn.Testing.Benchmarks;

[NeedsDocs]
public static class TestEffortContextHelpers
{
    public static string? GetCommitSha(Assembly assembly)
    {
        var versionInfo = AppVersionInfo.InformationalVersionForAssembly(assembly);
        return versionInfo?.CommitSha;
    }
}
