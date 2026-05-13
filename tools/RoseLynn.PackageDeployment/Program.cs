using Garyon.Functions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RoseLynn.PackageDeployment;

internal static class Program
{
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
