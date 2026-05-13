using System.Xml.Linq;

namespace RoseLynn.PackageDeployment;

internal static class SolutionProjectDiscovery
{
    public static async Task<IReadOnlyList<string>> GetProjectPathsAsync(string solutionFilePath)
    {
        if (!File.Exists(solutionFilePath))
            throw new FileNotFoundException("Solution file not found.", solutionFilePath);

        var extension = Path.GetExtension(solutionFilePath);
        var solutionDirectory = Path.GetDirectoryName(solutionFilePath)!;

        return extension.ToLowerInvariant() switch
        {
            ".slnx" => await GetFromSlnxAsync(solutionFilePath, solutionDirectory),
            ".sln" => await GetFromSlnAsync(solutionFilePath, solutionDirectory),
            _ => throw new InvalidOperationException($"Unsupported solution file extension: {extension}"),
        };
    }

    private static async Task<IReadOnlyList<string>> GetFromSlnxAsync(string solutionFilePath, string solutionDirectory)
    {
        var contents = await File.ReadAllTextAsync(solutionFilePath);
        var document = XDocument.Parse(contents);

        var projects = document
            .Descendants("Project")
            .Select(static e => e.Attribute("Path")?.Value)
            .Where(static p => !string.IsNullOrWhiteSpace(p))
            .Select(p => Path.GetFullPath(Path.Combine(solutionDirectory, p!)))
            .ToArray();

        return projects;
    }

    private static async Task<IReadOnlyList<string>> GetFromSlnAsync(string solutionFilePath, string solutionDirectory)
    {
        var projects = new List<string>();
        var lines = await File.ReadAllLinesAsync(solutionFilePath);
        foreach (var line in lines)
        {
            // Project("{GUID}") = "Name", "path\to\project.csproj", "{GUID}"
            var trimmed = line.TrimStart();
            if (!trimmed.StartsWith("Project(", StringComparison.Ordinal))
                continue;

            var comma1 = trimmed.IndexOf(',');
            if (comma1 < 0)
                continue;

            var comma2 = trimmed.IndexOf(',', comma1 + 1);
            if (comma2 < 0)
                continue;

            var pathPart = trimmed[(comma1 + 1)..comma2].Trim().Trim('"');
            if (!pathPart.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                continue;

            projects.Add(Path.GetFullPath(Path.Combine(solutionDirectory, pathPart)));
        }

        return projects;
    }
}
