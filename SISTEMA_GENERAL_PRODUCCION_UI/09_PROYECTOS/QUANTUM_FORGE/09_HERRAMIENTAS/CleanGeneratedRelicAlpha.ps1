param(
    [Parameter(Mandatory = $true)]
    [string]$InputPath,
    [Parameter(Mandatory = $true)]
    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$resolvedInput = (Resolve-Path -LiteralPath $InputPath).Path
$source = [System.Drawing.Bitmap]::FromFile($resolvedInput)
$bitmap = [System.Drawing.Bitmap]::new($source)
$source.Dispose()

try {
    for ($y = 0; $y -lt $bitmap.Height; $y++) {
        for ($x = 0; $x -lt $bitmap.Width; $x++) {
            $color = $bitmap.GetPixel($x, $y)
            $minimum = [math]::Min($color.R, [math]::Min($color.G, $color.B))
            $maximum = [math]::Max($color.R, [math]::Max($color.G, $color.B))
            $spread = $maximum - $minimum
            $alpha = 255

            if ($minimum -ge 225 -and $spread -le 18) {
                $alpha = 0
            }
            elseif ($minimum -ge 205 -and $spread -le 28) {
                $alpha = [int][math]::Max(0, [math]::Min(255, (225 - $minimum) / 20 * 255))
            }
            elseif ($minimum -ge 220 -and $spread -le 45) {
                $alpha = [int][math]::Max(0, [math]::Min(255, (235 - $minimum) / 15 * 255))
            }

            if ($alpha -ne 255) {
                $bitmap.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($alpha, $color.R, $color.G, $color.B))
            }
        }
    }

    $destinationDirectory = Split-Path -Parent $OutputPath
    if (-not [string]::IsNullOrWhiteSpace($destinationDirectory)) {
        [System.IO.Directory]::CreateDirectory($destinationDirectory) | Out-Null
    }
    $bitmap.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Write-Output "ALPHA_CLEAN_PASS: $OutputPath"
}
finally {
    $bitmap.Dispose()
}
