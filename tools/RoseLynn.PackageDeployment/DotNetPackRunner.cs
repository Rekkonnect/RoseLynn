using CliWrap;
using CliWrap.Buffered;
using Spectre.Console;

namespace RoseLynn.PackageDeployment;

internal static class DotNetPackRunner
{
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
