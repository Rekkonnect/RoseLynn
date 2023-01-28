namespace RoseLynn.PackageDeployment;

public static class PublicationInfo
{
    public const bool IsTest = Environment == DeploymentEnvironment.Test;

    public const DeploymentEnvironment Environment =
#if TEST_PACKAGES
        DeploymentEnvironment.Test;
#elif DEBUG
        DeploymentEnvironment.Development;
#else
        DeploymentEnvironment.Production;
#endif

    public const string NuGetRepositoryPath =
#if TEST_PACKAGES
        "https://int.nugettest.org/";
#else
        "https://api.nuget.org/v3/index.json";
#endif

    public enum DeploymentEnvironment
    {
        Development,
        Test,
        Production,
    }
}
