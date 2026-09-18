param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectPath
)

$ErrorActionPreference = 'Stop'
$failures = [System.Collections.Generic.List[string]]::new()

try {
    $resolvedProject = (Resolve-Path -LiteralPath $ProjectPath).Path
}
catch {
    Write-Output 'UNITY BATCH PREFLIGHT: BLOCKED'
    Write-Output "- No se pudo resolver el proyecto: $ProjectPath"
    exit 2
}

try {
    $editors = @(Get-CimInstance Win32_Process -Filter "Name='Unity.exe'" -ErrorAction Stop)
    $hubProcesses = @(Get-CimInstance Win32_Process -Filter "Name='Unity Hub.exe'" -ErrorAction Stop)
}
catch {
    Write-Output 'UNITY BATCH PREFLIGHT: BLOCKED'
    Write-Output '- No se pudieron inspeccionar los procesos de Unity y Hub con permisos normales.'
    exit 2
}

foreach ($editor in $editors) {
    $command = [string]$editor.CommandLine
    if ($command -like "*$resolvedProject*") {
        $failures.Add("Unity ya usa el proyecto objetivo (PID $($editor.ProcessId)).")
    }
    else {
        $failures.Add("Otra instancia de Unity está activa (PID $($editor.ProcessId)); esperar antes del batch.")
    }
}

foreach ($hub in $hubProcesses) {
    if ([string]$hub.CommandLine -match '(?i)(^|\s)-runTests(\s|$)') {
        $failures.Add("Unity Hub conserva una prueba -runTests (PID $($hub.ProcessId)).")
    }
}

$lockPath = Join-Path $resolvedProject 'Temp\UnityLockfile'
if (Test-Path -LiteralPath $lockPath) {
    $failures.Add('El proyecto conserva Temp/UnityLockfile; comprobar primero si el Editor sigue activo.')
}

if ($failures.Count -gt 0) {
    Write-Output 'UNITY BATCH PREFLIGHT: BLOCKED'
    $failures | ForEach-Object { Write-Output ("- " + $_) }
    exit 1
}

Write-Output 'UNITY BATCH PREFLIGHT: PASS (exclusividad global, Hub y lock verificados)'
exit 0
