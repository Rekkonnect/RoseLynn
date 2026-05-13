using Spectre.Console;

namespace RoseLynn.PackageDeployment;

internal sealed class PackageDeployment
{
    private readonly DeploymentEnvironmentConfig _config;

    public PackageDeployment(DeploymentEnvironmentConfig config)
    {
        _config = config;
    }

    public async Task RunAsync(bool dryRun)
    {
        var solutionDirectory = Path.GetDirectoryName(_config.SolutionFilePath)!;

        AnsiConsole.MarkupLine($"[grey]Discovering projects from[/] [bold]{Markup.Escape(_config.SolutionFilePath)}[/]...");
        var discoveredProjects = await SolutionProjectDiscovery.GetProjectPathsAsync(_config.SolutionFilePath);
        var projects = discoveredProjects
            .Where(static p => p.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var existingProjects = projects.Where(File.Exists).ToArray();
        var packableProjects = await GetPackableProjectsAsync(existingProjects);

        if (packableProjects.Length is 0)
        {
            AnsiConsole.MarkupLine("[yellow]No packable projects found.[/]");
            return;
        }

        WriteProjects(packableProjects, solutionDirectory);

        PrepareOutputDirectory(_config.PackagesOutputDirectory);

        await AnsiConsole.Progress()
            .AutoClear(false)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .StartAsync(async context =>
            {
                var task = context.AddTask("[green]Packing projects[/]", new ProgressTaskSettings { MaxValue = packableProjects.Length });

                foreach (var project in packableProjects)
                {
                    task.Description = $"[green]Packing[/] {Markup.Escape(Path.GetFileNameWithoutExtension(project))}";
                    await DotNetPackRunner.PackAsync(project, _config.PackagesOutputDirectory, _config.DotNetConfiguration, _config.IncludeSymbols);
                    task.Increment(1);
                }
            });

        var packages = DiscoverPackages(_config.PackagesOutputDirectory, _config.IncludeSymbols);
        if (packages.Length is 0)
        {
            AnsiConsole.MarkupLine("[yellow]No packages found after packing.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Built packages:[/] {packages.Length}");

        if (dryRun)
        {
            AnsiConsole.MarkupLine("[yellow]Dry run:[/] skipping publish.");
            return;
        }

        AnsiConsole.MarkupLine($"[grey]Publishing to[/] [bold]{Markup.Escape(_config.NuGetSourceUrl)}[/]...");
        await NuGetHelpers.UploadPackages(_config.NuGetSourceUrl, packages, _config.Secrets.NuGetApiKey);
        AnsiConsole.MarkupLine("[green]Publish complete.[/]");
    }

    private static void PrepareOutputDirectory(string outputDirectory)
    {
        if (Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, true);

        Directory.CreateDirectory(outputDirectory);
    }

    private static string[] DiscoverPackages(string outputDirectory, bool includeSymbols)
    {
        var patterns = includeSymbols
            ? new[] { "*.nupkg", "*.snupkg" }
            : new[] { "*.nupkg" };

        return patterns
            .SelectMany(p => Directory.EnumerateFiles(outputDirectory, p, SearchOption.TopDirectoryOnly))
            .Where(p => !p.EndsWith(".snupkg", StringComparison.OrdinalIgnoreCase) || includeSymbols)
            .OrderBy(static p => p, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void WriteProjects(string[] packableProjects, string solutionDirectory)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Packable projects[/]");

        foreach (var project in packableProjects)
        {
            var relative = Path.GetRelativePath(solutionDirectory, project);
            table.AddRow(Markup.Escape(relative));
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static async Task<string[]> GetPackableProjectsAsync(string[] existingProjects)
    {
        var packabilityTasks = existingProjects.Select(async path => (Path: path, IsPackable: await CsprojPackability.IsPackableAsync(path)));
        var results = await Task.WhenAll(packabilityTasks);

        return results
            .Where(static r => r.IsPackable)
            .Select(static r => r.Path)
            .OrderBy(static p => p, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
