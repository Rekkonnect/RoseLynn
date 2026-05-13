using System.Xml.Linq;

namespace RoseLynn.PackageDeployment;

internal static class CsprojPackability
{
    public static async Task<bool> IsPackableAsync(string projectFilePath)
    {
        var contents = await File.ReadAllTextAsync(projectFilePath);
        var document = XDocument.Parse(contents);

        var isPackableValue = document
            .Descendants("IsPackable")
            .Select(static e => e.Value?.Trim())
            .FirstOrDefault(static v => !string.IsNullOrWhiteSpace(v));

        if (isPackableValue is null)
            return true;

        return !string.Equals(isPackableValue, "false", StringComparison.OrdinalIgnoreCase);
    }
}
