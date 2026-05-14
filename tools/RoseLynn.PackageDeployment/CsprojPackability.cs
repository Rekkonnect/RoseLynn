using System.Xml.Linq;

namespace RoseLynn.PackageDeployment;

/// <summary>Determines whether a project file is packable based on its MSBuild properties.</summary>
internal static class CsprojPackability
{
    /// <summary>Checks whether a project should produce NuGet packages.</summary>
    /// <param name="projectFilePath">The project file to inspect.</param>
    /// <returns><see langword="true"/> if the project is packable; otherwise, <see langword="false"/>.</returns>
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
