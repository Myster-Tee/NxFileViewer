<#
    PowerShell script for generating binaries assets

    - Publish command
        
        PowerShell ./Publish.ps1


    - To allow PowerShell script execution, run PowerShell as Administrator and run

        Set-ExecutionPolicy Unrestricted

    - To restore default policy, run the command

        Set-ExecutionPolicy RemoteSigned

    - Documentation

        dotnet publish     :    https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish
        runtime identifier :    https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
#>

$OutDirRoot="Publish"
$AppName = "NxFileViewer"
$ProjectPath="src/NxFileViewer/NxFileViewer.csproj"

$AppVersion= Select-Xml -Path $ProjectPath -XPath "//Project/PropertyGroup/Version" | Select-Object -ExpandProperty Node | Select-Object -ExpandProperty InnerText
Write-Host "${AppName} version read: ${AppVersion}"

# Cleaning and initialize output folder
if (Test-Path $OutDirRoot) {
     Write-Host "Deleting output folder ""${OutDirRoot}""."
    Remove-Item ${OutDirRoot} -Recurse -ErrorAction Stop
}
New-Item -ItemType Directory -Path "${OutDirRoot}"

dotnet clean $ProjectPath

$Releases =
@(
    @{
        Suffix    = "_x64"
        Options = @( "-p:PublishSingleFile=true", "-c", "Release", "--no-self-contained", "-r", "win-x64" )
    },
    @{
        Suffix    = "_x86"
        Options = @( "-p:PublishSingleFile=true", "-c", "Release", "--no-self-contained", "-r", "win-x86" )
    }

);

Foreach($Release in $Releases) 
{
    $ReleaseName = "${AppName}_v${AppVersion}_$($Release.Suffix)"
    $ZipFileName = "${ReleaseName}.zip"

    Write-Host "============================================================================" -ForegroundColor Blue
    Write-Host "> Publishing ${ZipFileName}" -ForegroundColor Blue

    $ReleaseDir="${OutDirRoot}/${ReleaseName}/"
    
    # =============== #
    # ===> Build <=== #
    dotnet publish $ProjectPath $Release.Options -o $ReleaseDir
    # ===> Build <=== #
    # =============== #

    Compress-Archive -Path $ReleaseDir -DestinationPath "$OutDirRoot/$ZipFileName"
    Remove-Item $ReleaseDir -Recurse
}