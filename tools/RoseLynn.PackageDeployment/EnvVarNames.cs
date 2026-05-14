namespace RoseLynn.PackageDeployment;

/// <summary>Defines the environment variable names consumed by package deployment tooling.</summary>
internal static class EnvVarNames
{
    /// <summary>The variable that selects the named deployment environment.</summary>
    public const string EnvironmentName = "PACKAGEDEPLOYMENT_ENV_NAME";

    /// <summary>The variable that provides the NuGet source URL.</summary>
    public const string NuGetSourceUrl = "PACKAGEDEPLOYMENT_NUGET_SOURCE_URL";
    /// <summary>The variable that provides the NuGet API key.</summary>
    public const string NuGetApiKey = "PACKAGEDEPLOYMENT_NUGET_API_KEY";

    /// <summary>The variable that overrides the solution file path.</summary>
    public const string SolutionFile = "PACKAGEDEPLOYMENT_SOLUTION_FILE";
    /// <summary>The variable that selects the dotnet build configuration.</summary>
    public const string DotNetConfiguration = "PACKAGEDEPLOYMENT_DOTNET_CONFIGURATION";
    /// <summary>The variable that overrides the package output directory.</summary>
    public const string PackagesOutputDirectory = "PACKAGEDEPLOYMENT_PACKAGES_OUTPUT_DIRECTORY";
    /// <summary>The variable that toggles symbol package generation.</summary>
    public const string IncludeSymbols = "PACKAGEDEPLOYMENT_INCLUDE_SYMBOLS";
}

