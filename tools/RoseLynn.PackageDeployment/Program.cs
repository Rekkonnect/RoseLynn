using Spectre.Console.Cli;

namespace RoseLynn.PackageDeployment;

/// <summary>Application entry point for the package deployment CLI.</summary>
internal static class Program
{
    /// <summary>Runs the package deployment command-line application.</summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The process exit code.</returns>
    public static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<DeployCommand>();
        app.Configure(static config =>
        {
            config.SetApplicationName("RoseLynn.PackageDeployment");
            config.ValidateExamples();
            config.PropagateExceptions();
        });

        return await app.RunAsync(args);
    }

    // Note: command implementation lives in DeployCommand.cs
}
