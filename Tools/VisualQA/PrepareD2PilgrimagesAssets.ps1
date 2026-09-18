param(
    [string]$ReferencePath = "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/04_Peregrinaciones_Corregidas_V4.png",
    [string]$OutputDirectory = "Assets/Project/UI/Dimension2/Pilgrimages/Generated"
)

Add-Type -AssemblyName System.Drawing

$projectPath = (Resolve-Path '.').Path
$referenceFullPath = (Resolve-Path -LiteralPath $ReferencePath).Path
$outputFullPath = [IO.Path]::GetFullPath((Join-Path $projectPath $OutputDirectory))
if (-not $outputFullPath.StartsWith($projectPath + [IO.Path]::DirectorySeparatorChar)) {
    throw 'La salida de Peregrinaciones debe permanecer dentro del proyecto.'
}
[IO.Directory]::CreateDirectory($outputFullPath) | Out-Null

$source = [Drawing.Bitmap]::FromFile($referenceFullPath)
try {
    if ($source.Width -ne 941 -or $source.Height -ne 1672) {
        throw "Referencia inesperada: $($source.Width)x$($source.Height)."
    }

    function New-TransparentBitmap {
        $bitmap = New-Object Drawing.Bitmap($source.Width, $source.Height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [Drawing.Graphics]::FromImage($bitmap)
        try { $graphics.Clear([Drawing.Color]::Transparent) } finally { $graphics.Dispose() }
        return $bitmap
    }

    function Copy-FeatheredRegion([Drawing.Bitmap]$target, [int]$x, [int]$y, [int]$width, [int]$height, [int]$feather = 2) {
        $left = [Math]::Max(0, $x)
        $top = [Math]::Max(0, $y)
        $right = [Math]::Min($source.Width, $x + $width)
        $bottom = [Math]::Min($source.Height, $y + $height)
        for ($py = $top; $py -lt $bottom; $py++) {
            for ($px = $left; $px -lt $right; $px++) {
                $distance = [Math]::Min([Math]::Min($px - $left, $right - 1 - $px), [Math]::Min($py - $top, $bottom - 1 - $py))
                $alphaFactor = if ($feather -le 0) { 1.0 } else { [Math]::Min(1.0, ($distance + 1.0) / ($feather + 1.0)) }
                $color = $source.GetPixel($px, $py)
                $alpha = [int][Math]::Round($color.A * $alphaFactor)
                if ($alpha -gt 0) {
                    $target.SetPixel($px, $py, [Drawing.Color]::FromArgb($alpha, $color.R, $color.G, $color.B))
                }
            }
        }
    }

    $icons = New-TransparentBitmap
    try {
        $regions = @(
            @(37,29,81,55), @(47,90,55,68), @(312,88,47,72), @(529,87,43,75), @(708,91,56,68),
            @(49,218,118,193), @(48,496,120,191), @(48,775,121,191), @(47,1040,125,200), @(46,1311,127,199),
            @(624,876,32,59), @(510,1088,77,116), @(506,1410,45,49),
            @(50,1561,76,56), @(178,1561,73,56), @(302,1558,102,61), @(466,1528,72,59),
            @(575,1528,77,59), @(696,1530,72,57), @(823,1527,73,61)
        )
        foreach ($region in $regions) { Copy-FeatheredRegion $icons $region[0] $region[1] $region[2] $region[3] 2 }

        $rowYs = @(
            @(254,284,314,344,374), @(536,566,596,626,656), @(819,849,879,909,939),
            @(1079,1109,1139,1169,1199), @(1355,1385,1415,1445,1475)
        )
        foreach ($group in $rowYs) {
            foreach ($rowY in $group) { Copy-FeatheredRegion $icons 177 $rowY 25 29 1 }
        }
        $icons.Save((Join-Path $outputFullPath 'D2_Pilgrimages_StaticIconOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $icons.Dispose() }

    $selection = New-TransparentBitmap
    try {
        # Tarjeta Corta: conservar solamente los bordes luminosos, no su texto ni arte.
        Copy-FeatheredRegion $selection 25 187 435 20 3
        Copy-FeatheredRegion $selection 25 463 435 10 2
        Copy-FeatheredRegion $selection 25 187 24 286 3
        Copy-FeatheredRegion $selection 436 187 24 286 3
        # Pestaña Peregrinaciones seleccionada.
        Copy-FeatheredRegion $selection 270 1546 165 12 2
        Copy-FeatheredRegion $selection 270 1645 165 12 2
        Copy-FeatheredRegion $selection 270 1546 13 111 2
        Copy-FeatheredRegion $selection 422 1546 13 111 2
        $selection.Save((Join-Path $outputFullPath 'D2_Pilgrimages_ShortSelectionOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $selection.Dispose() }
}
finally { $source.Dispose() }

Get-ChildItem -LiteralPath $outputFullPath -Filter 'D2_Pilgrimages_*_v1.png' |
    Select-Object Name, Length
