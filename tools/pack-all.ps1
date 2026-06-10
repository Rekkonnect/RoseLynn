<#
.SYNOPSIS
    Bumps the package version in Directory.Build.props, packs all projects, and imports them to the local NuGet registry using lng.
.PARAMETER Version
    The explicit version to set. If not specified, the patch version will be automatically incremented.
.EXAMPLE
    .\tools\pack-all.ps1
    (Automatically increments patch version, packs, and imports to local NuGet registry)
.EXAMPLE
    .\tools\pack-all.ps1 -Version 0.9.1
    (Sets version to 0.9.1, packs, and imports to local NuGet registry)
#>
param (
    [string]$Version
)

$ErrorActionPreference = "Stop"

# Get solution root directory
$SolutionRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$PropsPath = Join-Path $SolutionRoot "Directory.Build.props"
$SlnxPath = Join-Path $SolutionRoot "RoseLynn.slnx"
$OutputDirectory = Join-Path $SolutionRoot "artifacts\packages"

Write-Host "--- RoseLynn Release Packager ---" -ForegroundColor Cyan

# Check if Directory.Build.props exists
if (-not (Test-Path $PropsPath)) {
    Write-Error "Could not find Directory.Build.props at: $PropsPath"
}

# Read current version from Directory.Build.props (preserving UTF-8 BOM if present)
$PropsContent = [System.IO.File]::ReadAllText($PropsPath)
if ($PropsContent -match '<Version>(?<version>[\d\.]+)</Version>') {
    $CurrentVersion = $Matches['version']
    Write-Host "Current version: $CurrentVersion" -ForegroundColor Gray
} else {
    Write-Error "Could not parse current version from Directory.Build.props"
}

# Calculate new version if not explicitly supplied
$NewVersion = $Version
if ([string]::IsNullOrWhiteSpace($NewVersion)) {
    $VersionObject = [System.Version]$CurrentVersion
    $Build = $VersionObject.Build
    if ($Build -lt 0) {
        $Build = 0
    }
    $NewVersion = [System.Version]::new($VersionObject.Major, $VersionObject.Minor, ($Build + 1)).ToString()
    Write-Host "Auto-incrementing patch version..." -ForegroundColor Gray
}

# Validate version string format (e.g. 0.9.0 or 1.2.3.4)
if (-not ($NewVersion -match '^\d+\.\d+(\.\d+)?(\.\d+)?$')) {
    Write-Error "Invalid version format: '$NewVersion'. Expected format: Major.Minor.Patch.Revision"
}

Write-Host "Setting version to: $NewVersion" -ForegroundColor Green

# Update Directory.Build.props (writing UTF-8 with BOM to preserve encoding/copyright symbols)
$UpdatedPropsContent = $PropsContent -replace '<Version>[\d\.]+</Version>', "<Version>$NewVersion</Version>"
$Utf8WithBom = New-Object System.Text.UTF8Encoding $true
[System.IO.File]::WriteAllText($PropsPath, $UpdatedPropsContent, $Utf8WithBom)

# Ensure clean output directory
if (Test-Path $OutputDirectory) {
    Write-Host "Cleaning output directory: $OutputDirectory" -ForegroundColor Gray
    Remove-Item -Path $OutputDirectory -Recurse -Force | Out-Null
}
New-Item -Path $OutputDirectory -ItemType Directory -Force | Out-Null

# Build the solution in Release configuration
# TreatWarningsAsErrors is disabled to ensure missing XML documentation does not fail compilation during release packaging
Write-Host "Building solution in Release configuration..." -ForegroundColor Cyan
& dotnet build $SlnxPath -c Release -p:TreatWarningsAsErrors=false
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet build failed with exit code $LASTEXITCODE"
}

# Pack projects
Write-Host "Packing projects to $OutputDirectory..." -ForegroundColor Cyan
& dotnet pack $SlnxPath -c Release -o $OutputDirectory --nologo -p:TreatWarningsAsErrors=false
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet pack failed with exit code $LASTEXITCODE"
}

# Check for generated package files
$Packages = Get-ChildItem -Path $OutputDirectory -Filter "*.nupkg"
if ($Packages.Count -eq 0) {
    Write-Warning "No .nupkg files found in $OutputDirectory to import."
    exit 0
}

# Check if 'lng' is available in the user's path
if (-not (Get-Command lng -ErrorAction SilentlyContinue)) {
    Write-Error "The 'lng' tool (LocalNuGet) was not found in your PATH. Please ensure it is installed and added to PATH."
}

# Import packages using lng
Write-Host "Importing $($Packages.Count) packages to local NuGet registry using 'lng'..." -ForegroundColor Cyan
$PackagePaths = $Packages.FullName

& lng add-packages $PackagePaths
if ($LASTEXITCODE -ne 0) {
    Write-Error "lng add-packages failed with exit code $LASTEXITCODE"
}

Write-Host "Release process completed successfully!" -ForegroundColor Green
