namespace RoseLynn.PackageDeployment;

#nullable disable

public sealed class Secrets
{
    public static Secrets Instance { get; } = new();

    private Secrets()
    {
        Load();
    }

    public string ApiKey { get; private set; }

    private void Load()
    {
        ApiKey = GetApiKey();
    }

    private static string GetApiKey()
    {
        var apiKeyPath = GetApiKeySourcePath();
        return File.ReadAllText(apiKeyPath);
    }

    private static string GetApiKeySourcePath()
    {
        return PublicationInfo.IsTest switch
        {
            true => "Secrets/testapikey.txt",
            false => "Secrets/apikey.txt",
        };
    }
}
