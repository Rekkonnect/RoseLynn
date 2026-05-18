#!/usr/bin/dotnet run --file
#:package CliWrap@*
#:package Garyon@*

using CliWrap;
using CliWrap.Buffered;
using Garyon.Functions;
using System.Diagnostics;

var openBrowser = args.Contains("-i");

using var tokenSource = CreateCancelKeyTokenSource();
var cancellationToken = tokenSource.Token;

Console.WriteLine("Running tests with coverage");

var solutionDirectory = WorkspaceHelpers.GetThisSolutionRoot()
    ?? throw new InvalidOperationException("Could not locate the solution root.");
var solutionFilePath = FindSolutionFile(solutionDirectory)
    ?? throw new InvalidOperationException($"Could not locate a solution file under '{solutionDirectory.FullName}'.");
var coverageArtifactsDirectory = new DirectoryInfo(Path.Combine(solutionDirectory.FullName, "artifacts", "coverage"));
var artifactsOutputDirectory = new DirectoryInfo(Path.Combine(coverageArtifactsDirectory.FullName, "results"));

RecreateDirectory(artifactsOutputDirectory);

var dotnetTestRunnerDirectory = CreateDotnetTestRunnerDirectory(coverageArtifactsDirectory);
var rawCoverageFilePath = Path.Combine(coverageArtifactsDirectory.FullName, "raw-coverage.cobertura.xml");
const string mergedCoverageFileName = "solution-coverage.cobertura.xml";

var testResult = await RunCommand(
    fileName: "dotnet-coverage",
    arguments:
    [
        "collect",
        "dotnet",
        "test",
        solutionFilePath.FullName,
        "--results-directory", artifactsOutputDirectory.FullName,
        "-o", rawCoverageFilePath,
        "-f", "cobertura",
    ],
    workingDirectory: dotnetTestRunnerDirectory,
    timeout: TimeSpan.FromMinutes(5),
    cancellationToken: cancellationToken);

if (!testResult.IsSuccess && !File.Exists(rawCoverageFilePath))
{
    PrintProcessOutput(testResult);
    return testResult.ExitCode;
}

if (!testResult.IsSuccess)
{
    Console.Error.WriteLine($"Test execution completed with failures (exit code {testResult.ExitCode}), but coverage data was collected. Continuing to generate reports.");
    PrintProcessOutput(testResult);
}

Console.WriteLine("Merging coverage files");

if (!artifactsOutputDirectory.Exists)
{
    Console.Error.WriteLine($"Coverage results directory was not created: {artifactsOutputDirectory.FullName}");
    return -10925;
}

if (!File.Exists(rawCoverageFilePath))
{
    Console.Error.WriteLine($"Coverage collection did not create the expected report: {rawCoverageFilePath}");
    return -10926;
}

var mergeResult = await RunCommand(
    fileName: "dotnet-coverage",
    arguments:
        ["merge", rawCoverageFilePath, "--remove-input-files", "-o", mergedCoverageFileName, "-f", "cobertura"],
    workingDirectory: artifactsOutputDirectory,
    timeout: null,
    cancellationToken: cancellationToken);

if (!mergeResult.IsSuccess)
{
    PrintProcessOutput(mergeResult);
    return mergeResult.ExitCode;
}

Console.WriteLine("Generating coverage reports");
const string coverageReportSubdirectoryName = "coverage-report";
var coverageReportDirectory = new DirectoryInfo(Path.Combine(artifactsOutputDirectory.FullName, coverageReportSubdirectoryName));

var reportResult = await RunCommand(
    fileName: "reportgenerator",
    arguments:
    [
        $"-reports:{mergedCoverageFileName}",
        $"-targetdir:{coverageReportSubdirectoryName}",
        "-reporttypes:Html_Dark,Badges",
        "-assemblyfilters:-Garyon*;-*.Test", 
        // For now we exclude generated files because they are not present in
        // the disk, which pollutes the output with tons of errors.
        // The generated files themselves might contain uncovered code that does
        // not matter for this report.
        "-filefilters:-*.g.cs",
    ],
    workingDirectory: artifactsOutputDirectory,
    timeout: TimeSpan.FromMinutes(1),
    cancellationToken: cancellationToken);

if (!reportResult.IsSuccess)
{
    PrintProcessOutput(reportResult);
    return reportResult.ExitCode;
}

Console.WriteLine("Copying coverage badges to .github directory");

IReadOnlyList<string> syncedFileNames =
[
    "badge_linecoverage.svg",
    "badge_methodcoverage.svg",
];

const string githubImagesDirectoryName = ".github/images/";
var githubImagesDirectory = new DirectoryInfo(Path.Combine(solutionDirectory.FullName, githubImagesDirectoryName));

SyncFiles(syncedFileNames, coverageReportDirectory, githubImagesDirectory);

if (openBrowser)
{
    Console.WriteLine("Opening coverage report index in browser");
    var indexFile = new FileInfo(Path.Combine(coverageReportDirectory.FullName, "index.html"));
    OpenPath(indexFile.FullName);
}

return testResult.IsSuccess ? 0 : testResult.ExitCode;

static FileInfo? FindSolutionFile(DirectoryInfo solutionDirectory)
{
    var solutionPath = solutionDirectory
        .EnumerateFiles("*.slnx", SearchOption.TopDirectoryOnly)
        .Concat(solutionDirectory.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly))
        .OrderBy(static f => f.Name, StringComparer.OrdinalIgnoreCase)
        .FirstOrDefault();

    return solutionPath;
}

static DirectoryInfo CreateDotnetTestRunnerDirectory(DirectoryInfo coverageArtifactsDirectory)
{
    var runnerDirectory = new DirectoryInfo(Path.Combine(coverageArtifactsDirectory.FullName, "mtp-runner"));
    runnerDirectory.Create();

    var globalJsonPath = Path.Combine(runnerDirectory.FullName, "global.json");
    const string globalJsonContents = """
        {
          "test": {
            "runner": "Microsoft.Testing.Platform"
          }
        }
        """;

    File.WriteAllText(globalJsonPath, globalJsonContents);
    return runnerDirectory;
}

static void RecreateDirectory(DirectoryInfo directory)
{
    if (directory.Exists)
        directory.Delete(true);

    directory.Create();
}

static void SyncFiles(
    IReadOnlyList<string> fileNames,
    DirectoryInfo source,
    DirectoryInfo target)
{
    // Ignore the command if the directory does not exist
    if (!source.Exists)
    {
        return;
    }

    target.Create();
    foreach (var file in fileNames)
    {
        SyncFile(file, source, target);
    }
}

static void SyncFile(
    string fileName,
    DirectoryInfo source,
    DirectoryInfo target)
{
    var sourceFile = new FileInfo(Path.Combine(source.FullName, fileName));
    var targetFile = new FileInfo(Path.Combine(target.FullName, fileName));
    if (!sourceFile.Exists)
    {
        targetFile.Delete();
        return;
    }

    sourceFile.CopyTo(targetFile.FullName, true);
}

static void OpenPath(string path)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true,
        },
    };

    process.Start();
}

static void PrintProcessOutput(RunResult runResult)
{
    if (!string.IsNullOrWhiteSpace(runResult.StandardOutput))
    {
        Console.Out.WriteLine(runResult.StandardOutput);
    }

    if (!string.IsNullOrWhiteSpace(runResult.StandardError))
    {
        Console.Error.WriteLine(runResult.StandardError);
    }
}

static CancellationTokenSource CreateCancelKeyTokenSource()
{
    var tokenSource = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        tokenSource.Cancel();
    };
    return tokenSource;
}

static async Task<RunResult> RunCommand(
    string fileName,
    IReadOnlyList<string> arguments,
    DirectoryInfo workingDirectory,
    TimeSpan? timeout,
    CancellationToken cancellationToken)
{
    var command = Cli
        .Wrap(fileName)
        .WithWorkingDirectory(workingDirectory.FullName)
        .WithArguments(arguments)
        .WithValidation(CommandResultValidation.None);

    using var timeoutTokenSource = timeout is { } t ? new CancellationTokenSource(t) : null;
    using var linkedTokenSource = timeoutTokenSource is not null
        ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token)
        : CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

    try
    {
        BufferedCommandResult result = await command.ExecuteBufferedAsync(linkedTokenSource.Token);
        return new RunResult(result.ExitCode, result.StandardOutput, result.StandardError);
    }
    catch (OperationCanceledException) when (timeoutTokenSource?.IsCancellationRequested is true)
    {
        return RunResult.Timeout(fileName, arguments, timeout!.Value);
    }
    catch (OperationCanceledException)
    {
        return RunResult.Cancelled(fileName, arguments);
    }
    catch (Exception ex)
    {
        return RunResult.StartFailure(fileName, arguments, ex);
    }
}

readonly record struct RunResult(int ExitCode, string StandardOutput, string StandardError)
{
    public bool IsSuccess => ExitCode == 0;

    public static RunResult Timeout(string fileName, IReadOnlyList<string> arguments, TimeSpan timeout)
        => new(-10930, string.Empty, $"Command timed out after {timeout.TotalSeconds:0} seconds: {fileName} {string.Join(' ', arguments)}");

    public static RunResult Cancelled(string fileName, IReadOnlyList<string> arguments)
        => new(-10931, string.Empty, $"Command cancelled: {fileName} {string.Join(' ', arguments)}");

    public static RunResult StartFailure(string fileName, IReadOnlyList<string> arguments, Exception ex)
        => new(-10932, string.Empty, $"Failed to start command: {fileName} {string.Join(' ', arguments)}{Environment.NewLine}{ex}\nInner exception: {ex.InnerException}");
}
