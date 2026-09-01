# Regenerates every packaged image and the application .ico from the vector definition in
# IconArt.ps1. Nothing in the repository is hand-drawn: change the artwork there and re-run this.
#
#   powershell -ExecutionPolicy Bypass -File tools\Generate-Icons.ps1
#
# Writes:
#   Glyfo (Package)\Images\*.png   the MSIX asset set, every scale and target size
#   Glyfo\Assets\Glyfo.ico      the executable icon, also used by the tray

[CmdletBinding()]
param(
    [ValidateSet('A', 'B', 'C')][string]$Concept = 'B'
)

. "$PSScriptRoot\IconArt.ps1"

$root = Split-Path -Parent $PSScriptRoot
$imageDir = Join-Path $root 'Glyfo (Package)\Images'
$assetDir = Join-Path $root 'Glyfo\Assets'
New-Item -ItemType Directory -Force -Path $imageDir, $assetDir | Out-Null

$png = [System.Drawing.Imaging.ImageFormat]::Png
$written = 0

<#
Composes the mark, at $IconSize pixels, centred on a transparent canvas of $W x $H.
#>
function Save-Asset {
    param([string]$Path, [int]$W, [int]$H, [int]$IconSize, [switch]$Badge)

    $canvas = New-Object System.Drawing.Bitmap($W, $H, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($canvas)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality

    $art = Render-Icon -Concept $Concept -Size $IconSize -Badge:$Badge
    $g.DrawImage($art, [int][Math]::Round(($W - $IconSize) / 2.0), [int][Math]::Round(($H - $IconSize) / 2.0), $IconSize, $IconSize)
    $art.Dispose()
    $g.Dispose()

    $canvas.Save($Path, $png)
    $canvas.Dispose()
    $script:written++
}

# ---------------------------------------------------------------------------------- MSIX assets

# Fill fractions follow Microsoft's app-icon construction guidance: tiles carry the mark at about
# half their height so it sits in the same optical rhythm as the first-party tiles, while the
# app-list and Store assets are nearly full-bleed.
$assets = @(
    @{ Name = 'Square44x44Logo';   W = 44;  H = 44;  Fill = 0.90 },
    @{ Name = 'Square71x71Logo';   W = 71;  H = 71;  Fill = 0.507 },
    @{ Name = 'Square150x150Logo'; W = 150; H = 150; Fill = 0.507 },
    @{ Name = 'Square310x310Logo'; W = 310; H = 310; Fill = 0.503 },
    @{ Name = 'Wide310x150Logo';   W = 310; H = 150; Fill = 0.507 },
    @{ Name = 'StoreLogo';         W = 50;  H = 50;  Fill = 0.90 },
    @{ Name = 'SplashScreen';      W = 620; H = 300; Fill = 0.60 },
    @{ Name = 'LockScreenLogo';    W = 24;  H = 24;  Fill = 1.00; Badge = $true }
)

$scales = 100, 125, 150, 200, 400

foreach ($a in $assets) {
    foreach ($scale in $scales) {
        # Half-up, not [Math]::Round: that is banker's rounding, and it made 71x71 at 150% come out
        # 106 px where the packager demands 107 (APPX1619).
        $w = [int][Math]::Floor($a.W * $scale / 100.0 + 0.5)
        $h = [int][Math]::Floor($a.H * $scale / 100.0 + 0.5)
        # The mark is measured against the short side, so a wide tile keeps the same mark as a square one.
        $icon = [int][Math]::Round([Math]::Min($w, $h) * $a.Fill)
        $path = Join-Path $imageDir "$($a.Name).scale-$scale.png"
        Save-Asset -Path $path -W $w -H $h -IconSize $icon -Badge:([bool]$a.Badge)
    }
}

# Target-size assets are what the taskbar, Alt+Tab, the tray and the app list actually draw. They
# carry no padding of their own — the shell adds it — so the mark fills the canvas.
#   (plain)              on the accent-coloured plate
#   _altform-unplated    no plate, dark shell
#   _altform-lightunplated   no plate, light shell
# The mark is a solid tile that holds up on any ground, so all three are the same drawing.
$targetSizes = 16, 20, 24, 32, 40, 48, 64, 256
$forms = '', '_altform-unplated', '_altform-lightunplated'

foreach ($size in $targetSizes) {
    foreach ($form in $forms) {
        $path = Join-Path $imageDir "Square44x44Logo.targetsize-$size$form.png"
        Save-Asset -Path $path -W $size -H $size -IconSize $size
    }
}

# ----------------------------------------------------------------------------------------- .ico

<#
Encodes a bitmap as an icon directory entry. Sizes up to 64 go in as a bottom-up 32bpp DIB, which
every Windows API reads without question; 128 and 256 go in as PNG, which is what keeps the file
from reaching a megabyte.
#>
function Get-IconEntryBytes {
    param([System.Drawing.Bitmap]$Bitmap, [bool]$AsPng)

    if ($AsPng) {
        $stream = New-Object System.IO.MemoryStream
        $Bitmap.Save($stream, $png)
        $bytes = $stream.ToArray()
        $stream.Dispose()
        return $bytes
    }

    $w = $Bitmap.Width
    $h = $Bitmap.Height
    $rect = New-Object System.Drawing.Rectangle(0, 0, $w, $h)
    $data = $Bitmap.LockBits($rect, [System.Drawing.Imaging.ImageLockMode]::ReadOnly,
        [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $pixels = New-Object byte[] ($data.Stride * $h)
    [System.Runtime.InteropServices.Marshal]::Copy($data.Scan0, $pixels, 0, $pixels.Length)
    $Bitmap.UnlockBits($data)

    $maskStride = [int]([Math]::Floor(($w + 31) / 32) * 4)
    $stream = New-Object System.IO.MemoryStream
    $writer = New-Object System.IO.BinaryWriter($stream)

    # BITMAPINFOHEADER. The height is doubled because the entry holds the colour bitmap and the
    # AND mask stacked; with 32bpp the mask is unused but the format still demands it.
    $writer.Write([int]40)
    $writer.Write([int]$w)
    $writer.Write([int]($h * 2))
    $writer.Write([int16]1)
    $writer.Write([int16]32)
    $writer.Write([int]0)
    $writer.Write([int]($w * $h * 4 + $maskStride * $h))
    $writer.Write([int]0); $writer.Write([int]0); $writer.Write([int]0); $writer.Write([int]0)

    # Bottom-up rows.
    for ($y = $h - 1; $y -ge 0; $y--) {
        $writer.Write($pixels, $y * $data.Stride, $w * 4)
    }
    $writer.Write((New-Object byte[] ($maskStride * $h)))

    $writer.Flush()
    $bytes = $stream.ToArray()
    $writer.Dispose()
    $bytes
}

$icoSizes = 16, 20, 24, 32, 40, 48, 64, 128, 256
$entries = @()
foreach ($size in $icoSizes) {
    $bitmap = Render-Icon -Concept $Concept -Size $size
    # The cast matters: PowerShell unrolls a returned byte[] into an object[], which BinaryWriter
    # will not accept as a buffer.
    $entries += [pscustomobject]@{
        Size = $size
        Data = [byte[]](Get-IconEntryBytes -Bitmap $bitmap -AsPng ($size -ge 128))
    }
    $bitmap.Dispose()
}

$icoPath = Join-Path $assetDir 'Glyfo.ico'
$stream = [System.IO.File]::Create($icoPath)
$writer = New-Object System.IO.BinaryWriter($stream)
$writer.Write([int16]0)                       # reserved
$writer.Write([int16]1)                       # type: icon
$writer.Write([int16]$entries.Count)

$offset = 6 + 16 * $entries.Count
foreach ($e in $entries) {
    $writer.Write([byte]($e.Size % 256))      # 256 is stored as 0
    $writer.Write([byte]($e.Size % 256))
    $writer.Write([byte]0)                    # palette size
    $writer.Write([byte]0)                    # reserved
    $writer.Write([int16]1)                   # colour planes
    $writer.Write([int16]32)                  # bits per pixel
    $writer.Write([int]$e.Data.Length)
    $writer.Write([int]$offset)
    $offset += $e.Data.Length
}
foreach ($e in $entries) { $writer.Write([byte[]]$e.Data, 0, $e.Data.Length) }
$writer.Flush()
$writer.Dispose()
$stream.Dispose()
$written++

Write-Host "$written files written"
Write-Host "  $imageDir"
Write-Host "  $icoPath"
