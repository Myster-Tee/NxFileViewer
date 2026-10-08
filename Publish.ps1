<#
    Builds standard and compressed self-contained x64/x86 application ZIPs and a separate optional firmware-hashes ZIP.
    Usage: ./Publish.ps1 [-OutputDirectory Publish]
#>
param([string]$OutputDirectory = "Publish")

$ErrorActionPreference = "Stop"
$AppName = "NxFileViewer"
$ProjectPath = Join-Path $PSScriptRoot "src/NxFileViewer/NxFileViewer.csproj"
$AppVersion = (Select-Xml -LiteralPath $ProjectPath -XPath "//Project/PropertyGroup/Version").Node.InnerText
$OutDirRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot $OutputDirectory))
New-Item -ItemType Directory -Path $OutDirRoot -Force | Out-Null
# Isolated staging leaves existing extracted releases and user keys untouched.
$StagingRoot = Join-Path $OutDirRoot (".publish-" + [Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $StagingRoot | Out-Null
try {
    foreach ($Architecture in @("x64", "x86")) {
        foreach ($WithRuntime in @($false, $true)) {
            $Variant = if ($WithRuntime) { "_self-contained" } else { "" }
            $ReleaseName = "${AppName}_v${AppVersion}${Variant}_${Architecture}"
            $ReleaseDir = Join-Path $StagingRoot $ReleaseName
            Write-Host "Publishing $ReleaseName"
            dotnet publish $ProjectPath -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=$WithRuntime -c Release --self-contained $WithRuntime -r "win-$Architecture" -o $ReleaseDir
            if ($LASTEXITCODE -ne 0) { throw "Publishing $Architecture failed ($LASTEXITCODE)." }
            if (Test-Path -LiteralPath (Join-Path $ReleaseDir "fw/hashes")) {
                throw "Firmware hashes must not be included in application archives."
            }
            Compress-Archive -LiteralPath $ReleaseDir -DestinationPath (Join-Path $OutDirRoot "$ReleaseName.zip") -Force
        }
    }

    # The add-on extracts to fw/hashes next to either architecture's executable.
    $HashRoot = Join-Path $StagingRoot "firmware-hashes"
    $HashDirectory = Join-Path $HashRoot "fw/hashes"
    New-Item -ItemType Directory -Path $HashDirectory -Force | Out-Null
    $HashFiles = @(Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot "fw/hashes") -Filter "*.json" -File)
    if ($HashFiles.Count -eq 0) { throw "No firmware hash references found." }
    foreach ($HashFile in $HashFiles) {
        Copy-Item -LiteralPath $HashFile.FullName -Destination $HashDirectory
    }
    Compress-Archive -LiteralPath (Join-Path $HashRoot "fw") -DestinationPath (Join-Path $OutDirRoot "${AppName}_v${AppVersion}_firmware-hashes.zip") -Force
}
finally {
    $ResolvedStaging = [IO.Path]::GetFullPath($StagingRoot)
    if ($ResolvedStaging.StartsWith($OutDirRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase) -and
        [IO.Path]::GetFileName($ResolvedStaging).StartsWith(".publish-")) {
        Remove-Item -LiteralPath $ResolvedStaging -Recurse -Force
    }
}
