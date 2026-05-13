namespace RoseLynn.PackageDeployment;

internal static class EnvVarNames
{
    public const string EnvironmentName = "PACKAGEDEPLOYMENT_ENV_NAME";

    public const string NuGetSourceUrl = "PACKAGEDEPLOYMENT_NUGET_SOURCE_URL";
    public const string NuGetApiKey = "PACKAGEDEPLOYMENT_NUGET_API_KEY";

    public const string SolutionFile = "PACKAGEDEPLOYMENT_SOLUTION_FILE";
    public const string DotNetConfiguration = "PACKAGEDEPLOYMENT_DOTNET_CONFIGURATION";
    public const string PackagesOutputDirectory = "PACKAGEDEPLOYMENT_PACKAGES_OUTPUT_DIRECTORY";
    public const string IncludeSymbols = "PACKAGEDEPLOYMENT_INCLUDE_SYMBOLS";
}

