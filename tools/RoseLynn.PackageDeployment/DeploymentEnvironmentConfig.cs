namespace RoseLynn.PackageDeployment;

/// <summary>Represents the resolved configuration for a package deployment environment.</summary>
/// <param name="Name">The deployment environment name.</param>
/// <param name="NuGetSourceUrl">The NuGet feed URL to publish to.</param>
/// <param name="Secrets">The secrets used during deployment.</param>
/// <param name="SolutionFilePath">The solution file that defines the projects to pack.</param>
/// <param name="DotNetConfiguration">The dotnet build configuration to use.</param>
/// <param name="PackagesOutputDirectory">The output directory for generated packages.</param>
/// <param name="IncludeSymbols">Whether symbol packages should also be produced.</param>
internal sealed record DeploymentEnvironmentConfig(
    string Name,
    string NuGetSourceUrl,
    Secrets Secrets,
    string SolutionFilePath,
    string DotNetConfiguration,
    string PackagesOutputDirectory,
    bool IncludeSymbols
);

