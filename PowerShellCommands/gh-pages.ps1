# Set variables
$branchName = "gh-pages"
$commandDir = Get-Location
$projectDir = (Get-Item -Path $commandDir).Parent.FullName
$tempRepoDir = "$projectDir\GithubPagesHelper\TempRepo"
$sourceDir = "$projectDir\GithubPagesHelper\GithubPagesBuild"

# Check if the parent directory is a Git repository
Write-Host "Checking if the parent directory is a Git repository..."
if (Test-Path "$projectDir\.git") {
    Write-Host "Git repository detected. Starting operations..."

    # Clean up or create the temporary directory
    if (Test-Path $tempRepoDir) {
        Write-Host "Cleaning up the TempRepo directory..."
        Remove-Item -Path $tempRepoDir -Recurse -Force
    }
    Write-Host "Creating the TempRepo directory..."
    New-Item -ItemType Directory -Path $tempRepoDir -Force | Out-Null

    # Navigate to the temporary directory
    Set-Location -Path $tempRepoDir

    # Get the remote URL of the current repository
    $currentRepoUrl = git -C $projectDir config --get remote.origin.url
    Write-Host "Checking if the branch '$branchName' exists in the remote repository: $currentRepoUrl"

    # Check if the remote branch exists
    $branchExists = git ls-remote --heads $currentRepoUrl $branchName | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }

    if ($branchExists) {
        Write-Host "The branch '$branchName' exists. Cloning it..."
        git clone --branch $branchName --single-branch $currentRepoUrl . 2>&1 | Out-Null
        Write-Host "Successfully cloned the $branchName branch!"
    } else {
        Write-Host "The branch '$branchName' does not exist. Creating an orphan branch..."
        git clone --no-checkout $currentRepoUrl . 2>&1 | Out-Null
        git checkout --orphan $branchName
        Write-Host "Orphan branch '$branchName' created."
    }

    # Clear the current branch and copy new contents
    Write-Host "Clearing the branch and copying new contents..."
    git rm -rf . 2>&1 | Out-Null

    if (Test-Path $sourceDir) {
        # Copy contents from GithubPagesBuild to the temporary directory
        Copy-Item -Path "$sourceDir\*" -Destination $tempRepoDir -Recurse -Force
        git add .
        git commit -m "Deploy updates to the $branchName branch"
        git push origin $branchName --force
        Write-Host "Successfully pushed contents to the $branchName branch!"
    } else {
        Write-Host "Error: GithubPagesBuild directory does not exist!"
    }

    Write-Host "Operation complete. Returned to the original directory."
} else {
    Write-Host "Error: The parent directory is not a Git repository!"
}

# Wait for user input before exiting
Write-Host "Press any key to continue..."
[System.Console]::ReadKey() | Out-Null
