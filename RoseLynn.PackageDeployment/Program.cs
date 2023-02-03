using Garyon.Functions;
using RoseLynn.PackageDeployment;
using System.Text.RegularExpressions;

string apiKey = Secrets.Instance.ApiKey;
var paths = GetLatestRoseLynnNuGetPackages(out var highestVersion).ToArray();

WriteHeader();
WriteDiscoveredPaths(paths, highestVersion);

foreach (var path in paths)
{
    try
    {
        await NuGetHelpers.UploadPackage(PublicationInfo.NuGetRepositoryPath, path.FullName, apiKey);
        ConsoleUtilities.WriteLineWithColor($"Uploaded package: {path.Name}", ConsoleColor.Green);
    }
    catch (Exception ex)
    {
        ConsoleUtilities.WriteLineWithColor($"Failed to upload package: {path.Name}", ConsoleColor.Magenta);
        ConsoleUtilities.WriteExceptionInfo(ex);
    }
}

static void WriteHeader()
{
    ConsoleUtilities.WriteWithColor("Deploying all latest RoseLynn packages to the ", ConsoleColor.Blue);
    WritePublicationEnvironment();
    ConsoleUtilities.WriteLineWithColor(" environment", ConsoleColor.Blue);
    Console.WriteLine();
}
static void WritePublicationEnvironment()
{
    // I know what I'm doing
#pragma warning disable IDE0035 // Unreachable code detected
#pragma warning disable CS0162 // Unreachable code detected
    switch (PublicationInfo.Environment)
    {
        case PublicationInfo.DeploymentEnvironment.Development:
            ConsoleUtilities.WriteWithColor("DEVELOPMENT", ConsoleColor.Red);
            break;

        case PublicationInfo.DeploymentEnvironment.Test:
            ConsoleUtilities.WriteWithColor("TEST", ConsoleColor.Yellow);
            break;

        case PublicationInfo.DeploymentEnvironment.Production:
            ConsoleUtilities.WriteWithColor("PRODUCTION", ConsoleColor.Green);
            break;
    }
#pragma warning restore CS0162 // Unreachable code detected
#pragma warning restore IDE0035 // Unreachable code detected
}

static void WriteDiscoveredPaths(IReadOnlyCollection<FileInfo> fileInfos, Version highestVersion)
{
    ConsoleUtilities.WriteWithColor("Discovered the following NuGet packages with highest version ", ConsoleColor.Yellow);
    ConsoleUtilities.WriteLineWithColor(highestVersion.ToString(), ConsoleColor.Cyan);

    foreach (var fileInfo in fileInfos)
    {
        ConsoleUtilities.WriteWithColor("- ", ConsoleColor.Gray);
        ConsoleUtilities.WriteLineWithColor(fileInfo.Name, ConsoleColor.Magenta);
    }
    Console.WriteLine();
}

static Version HighestVersion(IEnumerable<FileInfo> fileInfos)
{
    return fileInfos.Select(NuGetPackageFileInfo.FromFileInfo)
                    .Max(v => v.Version)!;
}
static IEnumerable<FileInfo> GetLatestRoseLynnNuGetPackages(out Version highestVersion)
{
    var allPackages = GetRoseLynnNuGetPackages()
                        .Where(p => !p.Name.Contains("RoseLynn.InternalGenerators"))
                        .ToList();
    highestVersion = HighestVersion(allPackages);
    // Lambda won't work without it
    var highestVersionLocal = highestVersion;
    return allPackages.Where(p => NuGetPackageFileInfo.ParseVersion(p) == highestVersionLocal)
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
