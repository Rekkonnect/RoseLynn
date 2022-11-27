using Garyon.Functions;
using RoseLynn.PackageDeployment;
using System.Text.RegularExpressions;

string apiKey = Secrets.Instance.ApiKey;
var paths = GetLatestRoseLynnNuGetPackages();
foreach (var path in paths)
{
    try
    {
        await NuGetHelpers.UploadPackage(PublicationInfo.NuGetRepositoryPath, path.FullName, apiKey);
        ConsoleUtilities.WriteWithColor($"Uploaded package: {path.Name}", ConsoleColor.Green);
    }
    catch (Exception ex)
    {
        ConsoleUtilities.WriteWithColor($"Failed to upload package: {path.Name}", ConsoleColor.Magenta);
        ConsoleUtilities.WriteExceptionInfo(ex);
    }
}

static Version HighestVersion(IEnumerable<FileInfo> fileInfos)
{
    return fileInfos.Select(NuGetPackageFileInfo.FromFileInfo)
                    .Max(v => v.Version)!;
}
static IEnumerable<FileInfo> GetLatestRoseLynnNuGetPackages()
{
    var allPackages = GetRoseLynnNuGetPackages().ToList();
    var highestVersion = HighestVersion(allPackages);
    return allPackages.Where(p => NuGetPackageFileInfo.ParseVersion(p) == highestVersion)
                      .ToList();
}
static IEnumerable<FileInfo> GetRoseLynnNuGetPackages()
{
    var roseLynnDirectory = new DirectoryInfo(HardProjectInfo.ProjectPath);
    return roseLynnDirectory.EnumerateFiles("RoseLynn*.nupkg", SearchOption.AllDirectories);
}

readonly partial struct NuGetPackageFileInfo
{
    private static readonly Regex nugetPackagePattern = NuGetPackagePattern();

    public FileInfo FileInfo { get; private init; }
    public Version Version { get; private init; }

    public static NuGetPackageFileInfo FromFileInfo(FileInfo fileInfo)
    {
        return new()
        {
            FileInfo = fileInfo,
            Version = ParseVersion(fileInfo),
        };
    }
    public static Version ParseVersion(FileInfo fileInfo)
    {
        var match = nugetPackagePattern.Match(fileInfo.Name);
        if (!match.Success)
            return null!;

        var versionString = match.Groups["version"].Value;
        return Version.Parse(versionString);
    }

    [GeneratedRegex(@"\.(?'version'\d[\d\.]*)(?'alphaSuffix'\-[\-\w\d]*)?\.nupkg")]
    private static partial Regex NuGetPackagePattern();
}
