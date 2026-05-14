namespace RoseLynn.PackageDeployment;

/// <summary>Holds sensitive deployment secrets resolved from configuration.</summary>
/// <param name="NuGetApiKey">The API key used to publish packages.</param>
internal sealed record Secrets(string NuGetApiKey);

