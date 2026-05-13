namespace RoseLynn.PackageDeployment;

internal sealed record DeploymentEnvironmentConfig(
    string Name,
    string NuGetSourceUrl,
    Secrets Secrets,
    string SolutionFilePath,
    string DotNetConfiguration,
    string PackagesOutputDirectory,
    bool IncludeSymbols
);

