# Determine the current script directory
$currentScriptPath = $PSScriptRoot

# Get the parent directory
$parentDirectory = Get-Item "$currentScriptPath/.." | Resolve-Path | Select-Object -ExpandProperty Path

# Check if the GithubPagesHelper directory exists
$targetDirectory = Join-Path -Path $parentDirectory -ChildPath "GithubPagesHelper"

if (Test-Path -Path $targetDirectory) {
    # Remove the directory
    Remove-Item -Path $targetDirectory -Recurse -Force
    Write-Host "Directory deleted: $targetDirectory" -ForegroundColor Green
} else {
    Write-Host "Target directory does not exist: $targetDirectory" -ForegroundColor Yellow
}

# Wait for user confirmation
Write-Host "Press any key to continue..."
[System.Console]::ReadKey() | Out-Null
