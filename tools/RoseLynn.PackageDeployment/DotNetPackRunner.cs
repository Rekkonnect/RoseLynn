using CliWrap;
using CliWrap.Buffered;
using Spectre.Console;

namespace RoseLynn.PackageDeployment;

/// <summary>Runs <c>dotnet pack</c> and surfaces failures in a deployment-friendly form.</summary>
internal static class DotNetPackRunner
{
    /// <summary>Packs a project into the target output directory.</summary>
    /// <param name="projectFilePath">The project file to pack.</param>
    /// <param name="outputDirectory">The output directory for generated packages.</param>
    /// <param name="configuration">The build configuration to pack.</param>
    /// <param name="includeSymbols">Whether symbol and source packages should be included.</param>
    public static async Task PackAsync(string projectFilePath, string outputDirectory, string configuration, bool includeSymbols)
    {
        Directory.CreateDirectory(outputDirectory);

        var args = new List<string>
        {
            "pack",
            projectFilePath,
            "-c",
            configuration,
            "-o",
            outputDirectory,
            "--nologo",
        };

        if (includeSymbols)
        {
            args.Add("--include-symbols");
            args.Add("--include-source");
        }

        var result = await Cli.Wrap("dotnet")
            .WithArguments(args)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        if (!string.IsNullOrWhiteSpace(result.StandardOutput))
            AnsiConsole.WriteLine(result.StandardOutput.TrimEnd());

        if (result.ExitCode != 0)
        {
            if (!string.IsNullOrWhiteSpace(result.StandardError))
                AnsiConsole.WriteLine(result.StandardError.TrimEnd());

            throw new InvalidOperationException($"dotnet pack failed with exit code {result.ExitCode}.");
        }
    }
}
