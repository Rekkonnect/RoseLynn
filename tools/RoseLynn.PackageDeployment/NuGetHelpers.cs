using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;

namespace RoseLynn.PackageDeployment;

// This code was copied from
// https://gist.github.com/evillegas92/b10bbb8b5ed9657579e6c2d13609e261
// The package has been upgraded, and the code is thus modified
// However it is more than likely that there is redunadnt or inefficient code
// Due to lack of documentation, as long as this get the job done, avoid touching
// Once Microsoft become good boys, and we get even *some* documentation,
// this code might be very subject to change
internal static class NuGetHelpers
{
    public async static Task UploadPackages(string nugetRepositoryUrl, IList<string> packagePaths, string apiKey)
    {
        var providers = Repository.Provider.GetCoreV3();
        var packageSource = new PackageSource(nugetRepositoryUrl);
        var sourceRepository = new SourceRepository(packageSource, providers);

        // Using the cache might be useful; even though it is not directly used anywhere
        // Would love to have some documentation about how this whole black magic thing works
        using var sourceCacheContext = new SourceCacheContext();
        var uploadResource = await sourceRepository.GetResourceAsync<PackageUpdateResource>();
        await uploadResource.Push(packagePaths, null, 480, false, _ => apiKey, null, false, false, null, NullLogger.Instance);
    }
    public async static Task UploadPackage(string nugetRepositoryUrl, string packagePath, string apiKey)
    {
        var packagePaths = new[] { packagePath };
        await UploadPackages(nugetRepositoryUrl, packagePaths, apiKey);
    }
}
