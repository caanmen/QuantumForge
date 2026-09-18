param(
    [string]$ReferencePath = "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/08_Pacto_Lugar_De_Vinculo_Corregido_V4.png",
    [string]$GeneratedCleanPath = "Assets/Project/UI/Dimension2/Bond/Generated/D2_Bond_ImageGenClean_v1.png",
    [string]$OutputDirectory = "Assets/Project/UI/Dimension2/Bond/Generated"
)

Add-Type -AssemblyName System.Drawing
$projectPath = (Resolve-Path '.').Path
$referenceFullPath = (Resolve-Path -LiteralPath $ReferencePath).Path
$generatedFullPath = (Resolve-Path -LiteralPath $GeneratedCleanPath).Path
$outputFullPath = [IO.Path]::GetFullPath((Join-Path $projectPath $OutputDirectory))
if (-not $outputFullPath.StartsWith($projectPath + [IO.Path]::DirectorySeparatorChar)) {
    throw 'La salida del Lugar de Vínculo debe permanecer dentro del proyecto.'
}
[IO.Directory]::CreateDirectory($outputFullPath) | Out-Null

$source = [Drawing.Bitmap]::FromFile($referenceFullPath)
$cleanSource = [Drawing.Bitmap]::FromFile($generatedFullPath)
try {
    if ($source.Width -ne 941 -or $source.Height -ne 1672 -or
        $cleanSource.Width -ne 941 -or $cleanSource.Height -ne 1672) {
        throw 'La referencia y la edición limpia deben medir 941x1672.'
    }

    $base = New-Object Drawing.Bitmap($source.Width, $source.Height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [Drawing.Graphics]::FromImage($base)
    try {
        $graphics.DrawImage($source, 0, 0, $source.Width, $source.Height)
        $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

        # Sustituye únicamente las zonas cuyo texto o cifra pertenece al controlador.
        # La geometría, iconos y ornamentación continúan procediendo de la V4.
        $regions = @(
            @(115,18,720,70),
            @(20,105,900,92),
            @(393,300,155,118), @(108,500,160,123),
            @(674,500,165,123), @(388,597,165,78),
            @(171,775,165,106), @(608,775,175,106),
            @(43,900,862,205),
            @(70,1128,300,94), @(557,1128,285,94),
            @(195,1246,550,69),
            @(43,1328,862,160),
            @(20,1566,900,84)
        )
        foreach ($region in $regions) {
            $rect = New-Object Drawing.Rectangle($region[0],$region[1],$region[2],$region[3])
            $graphics.DrawImage($cleanSource, $rect, $rect, [Drawing.GraphicsUnit]::Pixel)
        }

        # Los separadores y el chevrón verde son arte canónico fijo de la V4.
        # Se reponen después de limpiar el panel sin arrastrar el copy dinámico.
        $detailDecorations = @(
            @(68,958,432,14),
            @(68,976,78,72)
        )
        foreach ($decoration in $detailDecorations) {
            $rect = New-Object Drawing.Rectangle(
                $decoration[0],$decoration[1],$decoration[2],$decoration[3])
            $graphics.DrawImage($source, $rect, $rect, [Drawing.GraphicsUnit]::Pixel)
        }

        # Amplía levemente la placa central limpia para que PACTO ESTABLECIDO
        # conserve aire interior sin reducir el cuerpo tipográfico canónico.
        $centerPlaqueSource = New-Object Drawing.Rectangle(388,597,165,78)
        $centerPlaqueDestination = New-Object Drawing.Rectangle(378,592,185,88)
        $graphics.DrawImage(
            $cleanSource, $centerPlaqueDestination, $centerPlaqueSource,
            [Drawing.GraphicsUnit]::Pixel)
    }
    finally { $graphics.Dispose() }

    try {
        $base.Save((Join-Path $outputFullPath 'D2_Bond_CleanBasePlate_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $base.Dispose() }
}
finally {
    $source.Dispose()
    $cleanSource.Dispose()
}

Get-Item -LiteralPath (Join-Path $outputFullPath 'D2_Bond_CleanBasePlate_v1.png') |
    Select-Object Name,Length
