param(
    [string]$ReferencePath = "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/05_Noviciado_Corregido_V4.png",
    [string]$GeneratedCleanPath = "Assets/Project/UI/Dimension2/Novitiate/Generated/D2_Novitiate_ImageGenClean_v1.png",
    [string]$OutputDirectory = "Assets/Project/UI/Dimension2/Novitiate/Generated"
)

Add-Type -AssemblyName System.Drawing

$projectPath = (Resolve-Path '.').Path
$referenceFullPath = (Resolve-Path -LiteralPath $ReferencePath).Path
$generatedFullPath = (Resolve-Path -LiteralPath $GeneratedCleanPath).Path
$outputFullPath = [IO.Path]::GetFullPath((Join-Path $projectPath $OutputDirectory))
if (-not $outputFullPath.StartsWith($projectPath + [IO.Path]::DirectorySeparatorChar)) {
    throw 'La salida de Noviciado debe permanecer dentro del proyecto.'
}
[IO.Directory]::CreateDirectory($outputFullPath) | Out-Null

$source = [Drawing.Bitmap]::FromFile($referenceFullPath)
$cleanSource = [Drawing.Bitmap]::FromFile($generatedFullPath)
try {
    if ($source.Width -ne 941 -or $source.Height -ne 1672) {
        throw "Referencia inesperada: $($source.Width)x$($source.Height)."
    }

    $clean = New-Object Drawing.Bitmap($source.Width, $source.Height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [Drawing.Graphics]::FromImage($clean)
    try {
        $graphics.DrawImage($source, 0, 0, $source.Width, $source.Height)
        $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        # Sustituir solamente superficies con texto dinámico. Iconos, marcos y arte canónico
        # permanecen idénticos a V4; los valores y etiquetas pertenecen a Unity.
        $regions = @(
            @(120,20,760,85),
            @(24,103,893,103),
            @(240,180,460,125),
            @(48,625,845,212),
            @(101,826,738,147),
            @(49,976,844,83),
            @(119,1065,703,126),
            @(156,1200,630,96),
            @(50,1312,844,134),
            @(18,1600,905,67)
        )
        foreach ($region in $regions) {
            $destination = New-Object Drawing.Rectangle($region[0], $region[1], $region[2], $region[3])
            $sourceRect = New-Object Drawing.Rectangle(
                [int]($region[0] * $cleanSource.Width / $source.Width),
                [int]($region[1] * $cleanSource.Height / $source.Height),
                [int]($region[2] * $cleanSource.Width / $source.Width),
                [int]($region[3] * $cleanSource.Height / $source.Height)
            )
            $graphics.DrawImage($cleanSource, $destination, $sourceRect, [Drawing.GraphicsUnit]::Pixel)
        }
    }
    finally { $graphics.Dispose() }

    try {
        $clean.Save((Join-Path $outputFullPath 'D2_Novitiate_CleanBasePlate_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $clean.Dispose() }

    $icons = New-Object Drawing.Bitmap($source.Width, $source.Height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $iconGraphics = [Drawing.Graphics]::FromImage($icons)
    try { $iconGraphics.Clear([Drawing.Color]::Transparent) } finally { $iconGraphics.Dispose() }
    try {
        $iconRegions = @(
            @(49,126,52,64), @(273,126,49,65), @(477,126,53,64),
            @(84,690,86,124), @(370,716,43,67),
            @(207,861,32,34), @(184,1000,29,30)
        )
        foreach ($region in $iconRegions) {
            $destination = New-Object Drawing.Rectangle($region[0], $region[1], $region[2], $region[3])
            $iconGraphics = [Drawing.Graphics]::FromImage($icons)
            try { $iconGraphics.DrawImage($source, $destination, $destination, [Drawing.GraphicsUnit]::Pixel) }
            finally { $iconGraphics.Dispose() }
        }
        $icons.Save((Join-Path $outputFullPath 'D2_Novitiate_StaticIconOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $icons.Dispose() }
}
finally {
    $source.Dispose()
    $cleanSource.Dispose()
}

Get-ChildItem -LiteralPath $outputFullPath -Filter 'D2_Novitiate_*_v1.png' |
    Select-Object Name, Length
