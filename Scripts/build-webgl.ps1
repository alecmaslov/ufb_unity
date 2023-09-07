# Copy-Item -Path "E:\UnityProjects\UFB\Builds\WebGL\*" -Destination "E:\UFB\ufb-web" -Recurse -Force
$unityPath = "E:\Unity\2022.3.0f1\Editor\Unity.exe"
$projectPath = "E:\UnityProjects\UFB"
$repoPath = "E:\UFB\ufb-web"


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
$logFile = "$projectPath\Builds\Logs\unity_build.log"


[Environment]::SetEnvironmentVariable($unityPath, $buildPath, [System.EnvironmentVariableTarget]::Process)

# Build Unity project (Replace UNITY_PATH and UNITY_PROJECT_PATH with actual paths)
& $unityPath -batchmode -nographics -quit -projectPath $projectPath -executeMethod UFB.Project.Build.BuildWebGL -logFile $logFile
Start-Sleep -Seconds 10

Write-Host "Checking log file for errors..."


# Check if Unity build was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Unity build successful, copying build to repo."

    # Copy build to separate repo
    Copy-Item -Path "$buildPath\*" -Destination $repoPath -Recurse

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

} else {
    Write-Host "Unity build failed. Skipping copy and git operations."
}
