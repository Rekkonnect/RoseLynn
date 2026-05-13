#!/usr/bin/dotnet run --file
// TODO: Change the Garyon dependency, but otherwise retain the exact same behavior
#:project ../../Garyon/Garyon.csproj
#:package CliWrap@*

using CliWrap;
using CliWrap.Buffered;
using Garyon.Extensions;
using Garyon.Functions;

var openBrowser = args.Contains("-i");

using var tokenSource = CreateCancelKeyTokenSource();
var cancellationToken = tokenSource.Token;

Console.WriteLine("Running tests with coverage");

var solutionDirectory = WorkspaceHelpers.GetThisSolutionRoot()
    ?? throw new InvalidOperationException("Could not locate the solution root.");
const string resultSubdirectory = "artifacts/coverage/results/";

var testResult = await RunCommand(
    fileName: "dotnet",
    arguments:
    [
        "test",
        "--solution", "./",
        "--coverage",
        "--results-directory", $"./{resultSubdirectory}",
    ],
    workingDirectory: solutionDirectory,
    timeout: TimeSpan.FromSeconds(20),
    cancellationToken: cancellationToken);

if (!testResult.IsSuccess)
{
    PrintProcessOutput(testResult);
    return testResult.ExitCode;
}

Console.WriteLine("Merging coverage files");

var artifactsOutputDirectory = solutionDirectory.Subdirectory(resultSubdirectory);
if (!artifactsOutputDirectory.Exists)
{
    Console.Error.WriteLine($"Coverage results directory was not created: {artifactsOutputDirectory.FullName}");
    return -10925;
}

const string mergedCoverageFileName = "solution-coverage.cobertura.xml";

var mergeResult = await RunCommand(
    fileName: "dotnet-coverage",
    arguments:
    [
        "merge",
        "*.coverage",
        "--remove-input-files",
        "-o", mergedCoverageFileName,
        "-f", "cobertura",
    ],
    workingDirectory: solutionDirectory,
    timeout: null,
    cancellationToken: cancellationToken);

if (!mergeResult.IsSuccess)
{
    PrintProcessOutput(mergeResult);
    return mergeResult.ExitCode;
}

Console.WriteLine("Generating coverage reports");
const string coverageReportSubdirectoryName = "coverage-report";
var coverageReportDirectory = artifactsOutputDirectory.Subdirectory(coverageReportSubdirectoryName);

var reportResult = await RunCommand(
    fileName: "reportgenerator",
    arguments:
    [
        $"-reports:{mergedCoverageFileName}",
        $"-targetdir:{coverageReportSubdirectoryName}",
        "-reporttypes:Html_Dark,Badges",
        // For now we exclude generated files because they are not present in
        // the disk, which pollutes the output with tons of errors.
        // The generated files themselves might contain uncovered code that does
        // not matter for this report.
        "-filefilters:-*.g.cs",
    ],
    workingDirectory: artifactsOutputDirectory,
    timeout: TimeSpan.FromSeconds(10),
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
var githubImagesDirectory = solutionDirectory.Subdirectory(githubImagesDirectoryName);

SyncFiles(syncedFileNames, coverageReportDirectory, githubImagesDirectory);

if (openBrowser)
{
    Console.WriteLine("Opening coverage report index in browser");
    var indexFile = coverageReportDirectory.File("index.html");
    ProcessUtilities.OpenUrl(indexFile.FullName);
}

return 0;

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

    target.CreateSafe();
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
    var sourceFile = source.File(fileName);
    var targetFile = target.File(fileName);
    if (!sourceFile.Exists)
    {
        targetFile.Delete();
        return;
    }

    sourceFile.CopyTo(targetFile, true);
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
        .WithArguments(arguments);

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
