# Renders the three icon concepts side by side, at the sizes Windows actually uses them, on both
# a light and a dark shell background. Output: tools\preview\concepts.png
#
#   powershell -ExecutionPolicy Bypass -File tools\Preview-Icons.ps1

. "$PSScriptRoot\IconArt.ps1"

$outDir = Join-Path $PSScriptRoot 'preview'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$concepts = @(
    @{ Key = 'A'; Title = 'A  Selection frame'; Sub = 'brackets + text, no plate' },
    @{ Key = 'B'; Title = 'B  Solid tile';      Sub = 'filled square, white mark' },
    @{ Key = 'C'; Title = 'C  Page + frame';    Sub = 'layered, knockout gap' }
)

$sizes = @(48, 32, 24, 16)

$W = 1390
$headerH = 64
$rowH = 250
$H = $headerH + $rowH * $concepts.Count + 16

$bmp = New-Object System.Drawing.Bitmap($W, $H, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
$g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality

$g.Clear([System.Drawing.Color]::FromArgb(0xFF, 0xFF, 0xFF))

$fontTitle = New-Object System.Drawing.Font('Segoe UI Semibold', 15, [System.Drawing.FontStyle]::Regular)
$fontSub = New-Object System.Drawing.Font('Segoe UI', 10)
$fontTag = New-Object System.Drawing.Font('Segoe UI', 9)
$fontHead = New-Object System.Drawing.Font('Segoe UI Semibold', 12)
$inkDark = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0x20, 0x20, 0x20))
$inkGrey = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0x77, 0x77, 0x77))
$inkPale = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0x99, 0x99, 0x99))
$lightBg = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0xF3, 0xF3, 0xF3))
$darkBg = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0x1F, 0x1F, 0x1F))

$g.DrawString('Glyfo — icon concepts', $fontHead, $inkDark, 24, 22)
$g.DrawString('light shell  ·  #F3F3F3', $fontTag, $inkPale, 300, 26)
$g.DrawString('dark shell  ·  #1F1F1F', $fontTag, $inkPale, 848, 26)

$panelW = 520
$panelH = 218
$lightX = 292
$darkX = 840

for ($i = 0; $i -lt $concepts.Count; $i++) {
    $c = $concepts[$i]
    $rowY = $headerH + $rowH * $i
    $panelY = $rowY + 12

    $g.DrawString($c.Title, $fontTitle, $inkDark, 24, $panelY + 74)
    $g.DrawString($c.Sub, $fontSub, $inkGrey, 26, $panelY + 100)

    foreach ($panel in @(@($lightX, $lightBg, $inkGrey), @($darkX, $darkBg, $inkPale))) {
        $px = $panel[0]
        $path = New-GPath
        Add-RoundRect $path $px $panelY $panelW $panelH 10
        $g.FillPath($panel[1], $path)
        $path.Dispose()

        $big = Render-Icon -Concept $c.Key -Size 128
        $g.DrawImage($big, [int]($px + 32), [int]($panelY + ($panelH - 128) / 2), 128, 128)
        $big.Dispose()

        $x = $px + 212
        foreach ($s in $sizes) {
            $y = [int]($panelY + $panelH / 2 - $s / 2 - 6)
            $img = Render-Icon -Concept $c.Key -Size $s
            $g.DrawImage($img, [int]$x, $y, $s, $s)
            $img.Dispose()
            $g.DrawString("$s", $fontTag, $panel[2], [single]($x + $s / 2 - 8), [single]($panelY + $panelH / 2 + 44))
            $x += $s + 34
        }
    }
}

$g.Dispose()
$sheet = Join-Path $outDir 'concepts.png'
$bmp.Save($sheet, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

# A close-up of each concept, for judging the drawing itself rather than the legibility.
foreach ($c in $concepts) {
    $img = Render-Icon -Concept $c.Key -Size 512 -Supersample 2
    $pad = New-Object System.Drawing.Bitmap(560, 560, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $pg = [System.Drawing.Graphics]::FromImage($pad)
    $pg.Clear([System.Drawing.Color]::FromArgb(0xFA, 0xFA, 0xFA))
    $pg.DrawImage($img, 24, 24, 512, 512)
    $pg.Dispose()
    $pad.Save((Join-Path $outDir "concept-$($c.Key).png"), [System.Drawing.Imaging.ImageFormat]::Png)
    $pad.Dispose()
    $img.Dispose()
}

Write-Host "wrote $sheet"
