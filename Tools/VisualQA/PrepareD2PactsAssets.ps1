param(
    [string]$ReferencePath = "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/07_Pactos_De_Civilizacion_Corregidos_V4.png",
    [string]$GeneratedCleanPath = "Assets/Project/UI/Dimension2/Pacts/Generated/D2_Pacts_ImageGenNeutralClean_v1.png",
    [string]$OutputDirectory = "Assets/Project/UI/Dimension2/Pacts/Generated"
)

Add-Type -AssemblyName System.Drawing
$projectPath = (Resolve-Path '.').Path
$referenceFullPath = (Resolve-Path -LiteralPath $ReferencePath).Path
$generatedFullPath = (Resolve-Path -LiteralPath $GeneratedCleanPath).Path
$outputFullPath = [IO.Path]::GetFullPath((Join-Path $projectPath $OutputDirectory))
if (-not $outputFullPath.StartsWith($projectPath + [IO.Path]::DirectorySeparatorChar)) {
    throw 'La salida de Pactos debe permanecer dentro del proyecto.'
}
[IO.Directory]::CreateDirectory($outputFullPath) | Out-Null

$source = [Drawing.Bitmap]::FromFile($referenceFullPath)
$cleanSource = [Drawing.Bitmap]::FromFile($generatedFullPath)
try {
    if ($source.Width -ne 941 -or $source.Height -ne 1672 -or
        $cleanSource.Width -ne 941 -or $cleanSource.Height -ne 1672) {
        throw "La referencia y la edición limpia deben medir 941x1672."
    }

    $base = New-Object Drawing.Bitmap($source.Width, $source.Height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [Drawing.Graphics]::FromImage($base)
    try {
        $graphics.DrawImage($source, 0, 0, $source.Width, $source.Height)
        $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        # Únicamente los textos y cifras cambiantes se sustituyen por superficies limpias.
        $regions = @(
            @(130,20,680,70),
            @(125,98,170,78), @(340,98,155,78), @(560,98,145,78), @(765,98,160,78),
            @(65,350,345,75), @(520,350,360,75),
            @(65,595,365,75), @(505,595,385,75), @(385,748,400,78),
            @(245,866,455,55),
            @(55,1095,600,75),
            @(82,1172,555,58), @(82,1228,555,62), @(82,1287,555,58),
            @(58,1347,575,84), @(78,1427,555,48),
            @(675,1200,220,240),
            @(18,1585,905,70)
        )
        foreach ($region in $regions) {
            $rect = New-Object Drawing.Rectangle($region[0],$region[1],$region[2],$region[3])
            $graphics.DrawImage($cleanSource, $rect, $rect, [Drawing.GraphicsUnit]::Pixel)
        }
    }
    finally { $graphics.Dispose() }
    try { $base.Save((Join-Path $outputFullPath 'D2_Pacts_CleanBasePlate_v1.png'), [Drawing.Imaging.ImageFormat]::Png) }
    finally { $base.Dispose() }

    function New-TransparentBitmap {
        $bitmap = New-Object Drawing.Bitmap($source.Width, $source.Height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $g = [Drawing.Graphics]::FromImage($bitmap)
        try { $g.Clear([Drawing.Color]::Transparent) } finally { $g.Dispose() }
        return $bitmap
    }

    # Recupera los pictogramas pequeños de la V4 que la limpieza generativa dejó
    # parcialmente borrados. Cada recorte conserva su posición original exacta.
    $detailIcons = New-TransparentBitmap
    try {
        $g = [Drawing.Graphics]::FromImage($detailIcons)
        try {
            $iconRegions = @(
                @(66,1178,48,55), @(66,1235,48,54), @(66,1290,48,55)
            )
            foreach ($iconRegion in $iconRegions) {
                $rect = New-Object Drawing.Rectangle($iconRegion[0],$iconRegion[1],$iconRegion[2],$iconRegion[3])
                $g.DrawImage($source,$rect,$rect,[Drawing.GraphicsUnit]::Pixel)
            }
        }
        finally { $g.Dispose() }
        $detailIcons.Save((Join-Path $outputFullPath 'D2_Pacts_StaticDetailIconsOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $detailIcons.Dispose() }

    $lockRequirementIcons = New-TransparentBitmap
    try {
        $g = [Drawing.Graphics]::FromImage($lockRequirementIcons)
        try {
            $iconRegions = @(
                @(687,1240,39,38), @(687,1279,39,39), @(687,1318,39,39),
                @(687,1357,39,39), @(687,1396,45,40)
            )
            foreach ($iconRegion in $iconRegions) {
                $rect = New-Object Drawing.Rectangle($iconRegion[0],$iconRegion[1],$iconRegion[2],$iconRegion[3])
                $g.DrawImage($source,$rect,$rect,[Drawing.GraphicsUnit]::Pixel)
            }
        }
        finally { $g.Dispose() }
        $lockRequirementIcons.Save((Join-Path $outputFullPath 'D2_Pacts_LockRequirementIconsOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $lockRequirementIcons.Dispose() }

    # Conserva los dos botones de la referencia, pero limpia su texto para que TMP
    # siga siendo el único propietario del estado y de la acción.
    $actionButtons = New-TransparentBitmap
    try {
        $g = [Drawing.Graphics]::FromImage($actionButtons)
        try {
            $leftButton = New-Object Drawing.Rectangle(54,1348,270,87)
            $rightButton = New-Object Drawing.Rectangle(337,1348,291,87)
            $g.DrawImage($source,$leftButton,$leftButton,[Drawing.GraphicsUnit]::Pixel)
            $g.DrawImage($source,$rightButton,$rightButton,[Drawing.GraphicsUnit]::Pixel)
            $g.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $g.DrawImage($source,
                (New-Object Drawing.Rectangle(76,1371,226,39)),
                (New-Object Drawing.Rectangle(76,1359,226,12)),
                [Drawing.GraphicsUnit]::Pixel)
            $g.DrawImage($source,
                (New-Object Drawing.Rectangle(361,1371,243,39)),
                (New-Object Drawing.Rectangle(361,1359,243,12)),
                [Drawing.GraphicsUnit]::Pixel)
        }
        finally { $g.Dispose() }
        $actionButtons.Save((Join-Path $outputFullPath 'D2_Pacts_ActionButtonsOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $actionButtons.Dispose() }

    # La V4 trae Hospedaje seleccionado. Este overlay neutral permite seleccionar otro ID.
    $hospitalityNeutral = New-TransparentBitmap
    try {
        $g = [Drawing.Graphics]::FromImage($hospitalityNeutral)
        try {
            $rect = New-Object Drawing.Rectangle(34,195,420,245)
            $g.DrawImage($cleanSource, $rect, $rect, [Drawing.GraphicsUnit]::Pixel)
        }
        finally { $g.Dispose() }
        $hospitalityNeutral.Save((Join-Path $outputFullPath 'D2_Pacts_HospitalityNeutralOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $hospitalityNeutral.Dispose() }

    # Mueve únicamente el bisel dorado del marco seleccionado; el arte de cada Pacto queda intacto.
    $targets = @(
        @('OpenPath',474,196,430,244),
        @('Consecration',35,447,420,235),
        @('SilentVow',474,447,430,235),
        @('InnerDoor',157,690,625,174)
    )
    foreach ($target in $targets) {
        $selection = New-TransparentBitmap
        try {
            $g = [Drawing.Graphics]::FromImage($selection)
            try {
                $x=[int]$target[1]; $y=[int]$target[2]; $w=[int]$target[3]; $h=[int]$target[4]
                $rightX=$x+$w-22; $bottomY=$y+$h-22
                $g.DrawImage($source,(New-Object Drawing.Rectangle($x,$y,$w,22)),(New-Object Drawing.Rectangle(37,198,414,22)),[Drawing.GraphicsUnit]::Pixel)
                $g.DrawImage($source,(New-Object Drawing.Rectangle($x,$bottomY,$w,22)),(New-Object Drawing.Rectangle(37,416,414,22)),[Drawing.GraphicsUnit]::Pixel)
                $g.DrawImage($source,(New-Object Drawing.Rectangle($x,$y,22,$h)),(New-Object Drawing.Rectangle(37,198,22,240)),[Drawing.GraphicsUnit]::Pixel)
                $g.DrawImage($source,(New-Object Drawing.Rectangle($rightX,$y,22,$h)),(New-Object Drawing.Rectangle(429,198,22,240)),[Drawing.GraphicsUnit]::Pixel)
            }
            finally { $g.Dispose() }
            $selection.Save((Join-Path $outputFullPath ("D2_Pacts_"+$target[0]+"SelectionOverlay_v1.png")), [Drawing.Imaging.ImageFormat]::Png)
        }
        finally { $selection.Dispose() }
    }

    # Cubre el primer espacio horneado con una copia neutral del segundo cuando Hospedaje deja de estar activo.
    $slotNeutral = New-TransparentBitmap
    try {
        $g = [Drawing.Graphics]::FromImage($slotNeutral)
        try {
            $destination = New-Object Drawing.Rectangle(88,916,372,170)
            $emptySource = New-Object Drawing.Rectangle(487,916,372,170)
            $g.DrawImage($source,$destination,$emptySource,[Drawing.GraphicsUnit]::Pixel)
        }
        finally { $g.Dispose() }
        $slotNeutral.Save((Join-Path $outputFullPath 'D2_Pacts_HospitalitySlotNeutralOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $slotNeutral.Dispose() }

    # Estado permanente posterior a comprar el segundo espacio: conserva el marco y elimina candado/requisitos.
    $slotUnlocked = New-TransparentBitmap
    try {
        $g = [Drawing.Graphics]::FromImage($slotUnlocked)
        try {
            $sample = New-Object Drawing.Rectangle(690,1400,165,45)
            $interior = New-Object Drawing.Rectangle(674,1100,222,345)
            $g.DrawImage($cleanSource,$interior,$sample,[Drawing.GraphicsUnit]::Pixel)
        }
        finally { $g.Dispose() }
        $slotUnlocked.Save((Join-Path $outputFullPath 'D2_Pacts_SecondSlotUnlockedOverlay_v1.png'), [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $slotUnlocked.Dispose() }
}
finally { $source.Dispose(); $cleanSource.Dispose() }

Get-ChildItem -LiteralPath $outputFullPath -Filter 'D2_Pacts_*_v1.png' | Select-Object Name,Length
