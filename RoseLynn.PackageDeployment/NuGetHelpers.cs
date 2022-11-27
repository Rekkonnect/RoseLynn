using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace RoseLynn.PackageDeployment;

// This code was copied from
// https://gist.github.com/evillegas92/b10bbb8b5ed9657579e6c2d13609e261
// And the packages are intentionally left behind so that the code works
// Due to lack of documentation and time, no research was performed so as
// to set a system up supporting the latest version (6.4.0 at the time of writing)
// Hopefully in the future, once things are cleared out, this will be upgraded
// to use the newer versions for better performance and guidance
internal static class NuGetHelpers
{
    public async static Task UploadPackage(string nugetRepositoryUrl, string packagePath, string apiKey)
    {
        var providers = new List<Lazy<INuGetResourceProvider>>();
        providers.AddRange(Repository.Provider.GetCoreV3());
        var packageSource = new PackageSource(nugetRepositoryUrl);
        var sourceRepository = new SourceRepository(packageSource, providers);

        using var sourceCacheContext = new SourceCacheContext();
        var uploadResource = await sourceRepository.GetResourceAsync<PackageUpdateResource>();
        await uploadResource.Push(packagePath, null, 480, false, (param) => apiKey, null, NullLogger.Instance);
    }
}
