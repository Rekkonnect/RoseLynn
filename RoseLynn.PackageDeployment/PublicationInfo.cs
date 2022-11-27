// COMMENT THIS LINE TO AVOID PUSHING TO TEST API
//#define TEST_PACKAGES

namespace RoseLynn.PackageDeployment;

public static class PublicationInfo
{
    public const bool IsTest =
#if TEST_PACKAGES
        true;
#else
        false;
#endif

    public const string NuGetRepositoryPath =
#if TEST_PACKAGES
        "https://int.nugettest.org/";
#else
        "https://api.nuget.org/v3/index.json";
#endif

}
