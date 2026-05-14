using System.ComponentModel;
using DotNetEnv;
using Garyon.Functions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RoseLynn.PackageDeployment;

/// <summary>Defines command-line options for the package deployment command.</summary>
internal sealed class DeploySettings : CommandSettings
{
    [CommandOption("-e|--env <NAME>")]
    [Description("Loads .env.<NAME> from the PackageDeployment project directory.")]
    public string? EnvName { get; init; }

    [CommandOption("--env-file <PATH>")]
    [Description("Loads an explicit env file path (relative to the PackageDeployment project directory unless absolute).")]
    public string? EnvFilePath { get; init; }

    [CommandOption("--list-env")]
    [Description("Lists available .env.* files in the PackageDeployment project directory.")]
    public bool ListEnvironments { get; init; }

    [CommandOption("--dry-run")]
    [Description("Packs projects but does not publish packages.")]
    public bool DryRun { get; init; }

    [CommandOption("--no-override-env")]
    [Description("Does not override already-set environment variables.")]
    public bool NoOverrideExistingEnvVars { get; init; }
}

/// <summary>Loads deployment configuration, packs projects, and optionally publishes the resulting packages.</summary>
internal sealed class DeployCommand : AsyncCommand<DeploySettings>
{
    /// <summary>Executes the deployment command for the provided settings.</summary>
    /// <param name="context">The command execution context.</param>
    /// <param name="settings">The parsed deployment settings.</param>
    /// <returns>An exit code representing success or failure.</returns>
    public override async Task<int> ExecuteAsync(CommandContext context, DeploySettings settings)
    {
        try
        {
            var solutionRootDirectory = WorkspaceHelpers.GetThisSolutionRoot()
                ?? throw new InvalidOperationException("Failed to discover solution root.");
            var solutionRoot = solutionRootDirectory.FullName;

            var projectDirectory = Path.Combine(solutionRoot, "tools", "RoseLynn.PackageDeployment");
            if (!Directory.Exists(projectDirectory))
                throw new DirectoryNotFoundException($"Expected PackageDeployment project directory not found: '{projectDirectory}'.");

            if (settings.ListEnvironments)
            {
                ListEnvironmentFiles(projectDirectory);
                return 0;
            }

            var envFilePath = ResolveEnvFilePath(projectDirectory, settings.EnvName, settings.EnvFilePath);
            AnsiConsole.MarkupLine($"[grey]Loading environment:[/] [bold]{Markup.Escape(envFilePath)}[/]");

            var envContents = await File.ReadAllTextAsync(envFilePath);

            var clobberExisting = !settings.NoOverrideExistingEnvVars;
            Env.LoadContents(envContents, new LoadOptions(setEnvVars: true, clobberExistingVars: clobberExisting, onlyExactPath: true));

            if (settings.EnvName is not null)
                Environment.SetEnvironmentVariable(EnvVarNames.EnvironmentName, settings.EnvName);

            var config = PublicationConfigLoader.Load(settings.EnvName);
            RenderConfig(config, envFilePath);

            var deployment = new PackageDeployment(config);
            await deployment.RunAsync(settings.DryRun);
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
            return 1;
        }
    }

    private static void RenderConfig(DeploymentEnvironmentConfig config, string envFilePath)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Setting[/]")
            .AddColumn("[bold]Value[/]");

        table.AddRow("Environment", Markup.Escape(config.Name));
        table.AddRow("Env file", Markup.Escape(envFilePath));
        table.AddRow("NuGet source", Markup.Escape(config.NuGetSourceUrl));
        table.AddRow("Solution", Markup.Escape(config.SolutionFilePath));
        table.AddRow("Configuration", Markup.Escape(config.DotNetConfiguration));
        table.AddRow("Output", Markup.Escape(config.PackagesOutputDirectory));
        table.AddRow("Symbols", config.IncludeSymbols ? "[green]yes[/]" : "[grey]no[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static string ResolveEnvFilePath(string projectDirectory, string? envName, string? explicitEnvFilePath)
    {
        string envFilePath;

        if (!string.IsNullOrWhiteSpace(explicitEnvFilePath))
            envFilePath = explicitEnvFilePath!;
        else if (!string.IsNullOrWhiteSpace(envName))
            envFilePath = $".env.{envName}";
        else
            envFilePath = ".env";

        if (!Path.IsPathRooted(envFilePath))
            envFilePath = Path.Combine(projectDirectory, envFilePath);

        return envFilePath;
    }

    private static void ListEnvironmentFiles(string projectDirectory)
    {
        var envFiles = Directory.EnumerateFiles(projectDirectory, ".env.*", SearchOption.TopDirectoryOnly)
            .OrderBy(static p => p, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (envFiles.Length is 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No `.env.*` files found in[/] [bold]{Markup.Escape(projectDirectory)}[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Available environments[/]");

        foreach (var file in envFiles)
            table.AddRow(Markup.Escape(Path.GetFileName(file)));

        AnsiConsole.Write(table);
    }
}

