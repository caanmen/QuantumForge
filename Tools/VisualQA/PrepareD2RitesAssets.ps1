param(
    [string]$ReferencePath = "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/06_Ritos_Del_Santuario_Corregidos_V4.png",
    [string]$GeneratedCleanPath = "Assets/Project/UI/Dimension2/Rites/Generated/D2_Rites_ImageGenNeutralClean_v1.png",
    [string]$OutputDirectory = "Assets/Project/UI/Dimension2/Rites/Generated"
)

Add-Type -AssemblyName System.Drawing
$projectPath = (Resolve-Path '.').Path
$referenceFullPath = (Resolve-Path -LiteralPath $ReferencePath).Path
$generatedFullPath = (Resolve-Path -LiteralPath $GeneratedCleanPath).Path
$outputFullPath = [IO.Path]::GetFullPath((Join-Path $projectPath $OutputDirectory))
if (-not $outputFullPath.StartsWith($projectPath + [IO.Path]::DirectorySeparatorChar)) {
    throw 'La salida de Ritos debe permanecer dentro del proyecto.'
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
        # Sólo las superficies con contenido dinámico proceden de la edición limpia.
        $regions = @(
            @(130,20,680,80),
            @(104,110,150,90), @(342,110,150,90), @(550,110,125,90), @(768,110,145,90),
            @(280,216,385,62),
            @(38,520,175,105), @(224,520,153,105), @(397,520,153,105), @(570,520,153,105), @(742,520,165,105),
            @(150,688,650,90), @(160,785,625,56), @(130,840,675,82),
            @(240,945,460,58),
            @(145,1004,425,82), @(580,1005,300,82),
            @(145,1092,425,82), @(580,1093,300,82),
            @(215,1195,510,90),
            @(150,1300,730,102),
            @(18,1590,905,76)
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
    try { $clean.Save((Join-Path $outputFullPath 'D2_Rites_CleanBasePlate_v1.png'), [Drawing.Imaging.ImageFormat]::Png) }
    finally { $clean.Dispose() }

    function New-TransparentBitmap {
        $bitmap = New-Object Drawing.Bitmap($source.Width, $source.Height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $g = [Drawing.Graphics]::FromImage($bitmap)
        try { $g.Clear([Drawing.Color]::Transparent) } finally { $g.Dispose() }
        return $bitmap
    }

    $neutral = New-TransparentBitmap
    try {
        $g = [Drawing.Graphics]::FromImage($neutral)
        try {
            $g.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $rect = New-Object Drawing.Rectangle(24,286,194,365)
            $cleanRect = New-Object Drawing.Rectangle(
                [int](24*$cleanSource.Width/$source.Width), [int](286*$cleanSource.Height/$source.Height),
                [int](194*$cleanSource.Width/$source.Width), [int](365*$cleanSource.Height/$source.Height))
            $g.DrawImage($cleanSource, $rect, $cleanRect, [Drawing.GraphicsUnit]::Pixel)
        }
        finally { $g.Dispose() }
        $neutral.Save((Join-Path $outputFullPath 'D2_Rites_WelcomeNeutralOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $neutral.Dispose() }

    $targets = @(
        @('Offering',218,294,164,347), @('Path',390,294,166,347),
        @('Novitiate',565,294,164,347), @('Respect',737,294,166,347)
    )
    foreach ($target in $targets) {
        $selection = New-TransparentBitmap
        try {
            $g = [Drawing.Graphics]::FromImage($selection)
            try {
                $g.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
                $x=[int]$target[1]; $y=[int]$target[2]; $w=[int]$target[3]; $h=[int]$target[4]
                $bottomY = $y + $h - 24
                $rightX = $x + $w - 22
                $g.DrawImage($source, (New-Object Drawing.Rectangle($x,$y,$w,22)), (New-Object Drawing.Rectangle(25,290,190,22)), [Drawing.GraphicsUnit]::Pixel)
                $g.DrawImage($source, (New-Object Drawing.Rectangle($x,$bottomY,$w,24)), (New-Object Drawing.Rectangle(25,617,190,24)), [Drawing.GraphicsUnit]::Pixel)
                $g.DrawImage($source, (New-Object Drawing.Rectangle($x,$y,22,$h)), (New-Object Drawing.Rectangle(25,290,22,351)), [Drawing.GraphicsUnit]::Pixel)
                $g.DrawImage($source, (New-Object Drawing.Rectangle($rightX,$y,22,$h)), (New-Object Drawing.Rectangle(193,290,22,351)), [Drawing.GraphicsUnit]::Pixel)
            }
            finally { $g.Dispose() }
            $selection.Save((Join-Path $outputFullPath ("D2_Rites_"+$target[0]+"SelectionOverlay_v1.png")), [Drawing.Imaging.ImageFormat]::Png)
        }
        finally { $selection.Dispose() }
    }
}
finally { $source.Dispose(); $cleanSource.Dispose() }

Get-ChildItem -LiteralPath $outputFullPath -Filter 'D2_Rites_*_v1.png' | Select-Object Name,Length
