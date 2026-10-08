param([Parameter(Mandatory=$true)][string]$PlanPath, [switch]$NoRestart)
$ErrorActionPreference = 'Stop'
$plan = Get-Content -LiteralPath $PlanPath -Raw -Encoding UTF8 | ConvertFrom-Json
$stage = [IO.Path]::GetFullPath((Split-Path -Parent $PlanPath))
$target = [IO.Path]::GetFullPath($plan.Target)
$newExe = Join-Path $stage 'new.exe'
$backup = Join-Path $stage 'previous.exe'
$ready = Join-Path $stage 'ready'
$failure = Join-Path $stage 'error.txt'
$installed = $false
function Get-ExecutableHash([string]$path) {
    $sha = [Security.Cryptography.SHA256]::Create()
    $stream = [IO.File]::OpenRead($path)
    try { return [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-', '') }
    finally { $stream.Dispose(); $sha.Dispose() }
}

try {
    $expectedUpdates = [IO.Path]::GetFullPath((Join-Path (Split-Path -Parent $target) 'Updates'))
    if ((Split-Path -Parent $stage) -ne $expectedUpdates -or (Split-Path -Leaf $target) -ne 'NxFileViewer.exe') { throw 'Unexpected update paths.' }
    $expectedHash = $plan.ExecutableHash
    if ((Get-ExecutableHash $newExe) -ne $expectedHash) { throw 'Prepared update hash mismatch.' }
    $oldProcess = $null
    try { $oldProcess = [Diagnostics.Process]::GetProcessById($plan.ProcessId) } catch [ArgumentException] { }
    Set-Content -LiteralPath $ready -Value 'ready'
    if ($oldProcess -and !$oldProcess.WaitForExit(120000)) { throw 'NxFileViewer did not exit; update cancelled.' }
    if ((Get-ExecutableHash $newExe) -ne $expectedHash) { throw 'Prepared update changed while waiting.' }
    $deadline = [DateTime]::UtcNow.AddSeconds(30)
    while ($true) {
        try {
            # Atomic replacement retains the previous EXE as a recovery backup.
            [IO.File]::Replace($newExe, $target, $backup)
            $installed = $true
            break
        } catch [IO.IOException] {
            if ([DateTime]::UtcNow -ge $deadline) { throw }
            Start-Sleep -Milliseconds 250
        }
    }
    Set-Content -LiteralPath (Join-Path $stage 'completed') -Value 'installed'
    if (!$NoRestart) {
        $newProcess = Start-Process -FilePath $target -WorkingDirectory (Split-Path -Parent $target) -WindowStyle Hidden -PassThru
        Start-Sleep -Seconds 2
        if ($newProcess.HasExited) { throw 'Updated application exited immediately; restoring previous version.' }
    }
} catch {
    $message = $_.Exception.Message
    if ($installed -and (Test-Path -LiteralPath $backup)) {
        try { [IO.File]::Replace($backup, $target, (Join-Path $stage 'failed.exe')) } catch { $message += "`r`nRollback: $($_.Exception.Message)" }
    }
    Set-Content -LiteralPath $failure -Value $message
    if (!$NoRestart -and (!$oldProcess -or $oldProcess.HasExited)) {
        try { Start-Process -FilePath $target -WorkingDirectory (Split-Path -Parent $target) -WindowStyle Hidden } catch { }
    }
    if ($NoRestart) { exit 1 }
    Add-Type -AssemblyName PresentationFramework
    [System.Windows.MessageBox]::Show($message, 'NxFileViewer Update') | Out-Null
    exit 1
}
