using System.Globalization;
using Garyon.Functions;

namespace RoseLynn.PackageDeployment;

/// <summary>Builds deployment configuration from environment variables and solution defaults.</summary>
internal static class PublicationConfigLoader
{
    /// <summary>Loads the deployment configuration for the current environment.</summary>
    /// <param name="explicitName">An optional explicit environment name override.</param>
    /// <returns>The resolved deployment configuration.</returns>
    public static DeploymentEnvironmentConfig Load(string? explicitName = null)
    {
        var solutionRootDirectory = WorkspaceHelpers.GetThisSolutionRoot()
            ?? throw new InvalidOperationException("Failed to discover solution root.");
        var solutionRoot = solutionRootDirectory.FullName;

        string envName = GetRequired(EnvVarNames.EnvironmentName, explicitName);
        string nuGetSourceUrl = GetRequired(EnvVarNames.NuGetSourceUrl);
        string nuGetApiKey = GetRequired(EnvVarNames.NuGetApiKey);

        string solutionFile = Environment.GetEnvironmentVariable(EnvVarNames.SolutionFile)
            ?? DiscoverSolutionFile(solutionRoot);

        string dotnetConfiguration = Environment.GetEnvironmentVariable(EnvVarNames.DotNetConfiguration)
            ?? "Release";

        string packagesOutputDirectory = Environment.GetEnvironmentVariable(EnvVarNames.PackagesOutputDirectory)
            ?? Path.Combine(solutionRoot, "artifacts", "packages");

        bool includeSymbols = ParseBoolean(Environment.GetEnvironmentVariable(EnvVarNames.IncludeSymbols)) ?? false;

        if (!Path.IsPathRooted(solutionFile))
            solutionFile = Path.Combine(solutionRoot, solutionFile);

        if (!Path.IsPathRooted(packagesOutputDirectory))
            packagesOutputDirectory = Path.Combine(solutionRoot, packagesOutputDirectory);

        if (!File.Exists(solutionFile))
            throw new FileNotFoundException("The configured solution file was not found.", solutionFile);

        return new DeploymentEnvironmentConfig(
            envName,
            nuGetSourceUrl,
            new Secrets(nuGetApiKey),
            solutionFile,
            dotnetConfiguration,
            packagesOutputDirectory,
            includeSymbols
        );
    }

    private static string DiscoverSolutionFile(string solutionRoot)
    {
        var slnx = Directory.EnumerateFiles(solutionRoot, "*.slnx", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (slnx is not null)
            return slnx;

        var sln = Directory.EnumerateFiles(solutionRoot, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (sln is not null)
            return sln;

        throw new InvalidOperationException($"No solution file (.slnx or .sln) was found in '{solutionRoot}'. Set {EnvVarNames.SolutionFile} to override.");
    }

    private static string GetRequired(string name, string? overrideValue = null)
    {
        var value = overrideValue ?? Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Missing required environment variable '{name}'.");
        return value.Trim();
    }

    private static bool? ParseBoolean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (bool.TryParse(value.Trim(), out var result))
            return result;

        if (int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
            return integer != 0;

        return null;
    }
}
