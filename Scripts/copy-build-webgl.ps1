# Copy-Item -Path "E:\UnityProjects\UFB\Builds\WebGL\*" -Destination "E:\UFB\ufb-web" -Recurse -Force
$unityPath = "E:\Unity\2022.3.0f1\Editor\Unity.exe"
$projectPath = "E:\UnityProjects\UFB"
$repoPath = "E:\UFB\ufb-web"
$localPath = "E:\UFB\ufb-web-local"


# Get cwd, set it to a varaible

$cwd = Get-Location

# Function to prompt user for Yes or No
function Prompt-User {
    param (
        [string]$Message
    )
    $input = ""
    while ($input -ne "Y" -and $input -ne "N") {
        $input = Read-Host -Prompt "$Message (Y/N)"
    }
    return $input
}

$buildPath = "$projectPath\Builds\WebGL"


# Copy build to separate repo
Copy-Item -Path "$buildPath\*" -Destination $repoPath -Recurse -Force
Copy-Item -Path "$buildPath\*" -Destination $localPath -Recurse -Force

Write-Host "Copied build to repo and local."


# Change directory to the repository folder
Set-Location $repoPath

# Git add and commit (Make sure git is in your PATH or specify full path to git)
git add .
git commit -m "New build at $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"

# Ask user for permission to push to dev
$userInput = Prompt-User "Do you want to push to dev?"

if ($userInput -eq "Y") {
    git push origin dev
}
else {
    Write-Host "Skipped pushing to dev."
}

# Change directory back to the original directory
Set-Location $cwd