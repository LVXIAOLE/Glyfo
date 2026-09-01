# Contact sheet for the Store display images: each size on a light and a dark ground, plus the
# 300 px one shown at the size the Store actually draws it in a search result.
Add-Type -AssemblyName System.Drawing
$src = Join-Path (Split-Path -Parent $PSScriptRoot) 'docs\store-images'

$W = 900; $Row = 380
$canvas = New-Object Drawing.Bitmap $W, ($Row * 2)
$g = [Drawing.Graphics]::FromImage($canvas)
$g.SmoothingMode = 'AntiAlias'
$g.InterpolationMode = 'HighQualityBicubic'
$g.Clear([Drawing.Color]::FromArgb(0xF3, 0xF3, 0xF3))
$g.FillRectangle((New-Object Drawing.SolidBrush ([Drawing.Color]::FromArgb(0x20, 0x20, 0x20))), 0, $Row, $W, $Row)

$font = New-Object Drawing.Font 'Segoe UI', 12
# $band, not $row: PowerShell variable names are case-insensitive, so a loop variable called $row
# silently overwrites the $Row height above it and both bands land on top of each other.
foreach ($band in 0, 1) {
    $ink = if ($band -eq 0) { [Drawing.Color]::FromArgb(0x33, 0x33, 0x33) } else { [Drawing.Color]::FromArgb(0xE0, 0xE0, 0xE0) }
    $brush = New-Object Drawing.SolidBrush $ink
    $y = 40 + $band * $Row
    $x = 40
    foreach ($size in 300, 150, 71) {
        $img = [Drawing.Image]::FromFile((Join-Path $src "AppTile-${size}x${size}.png"))
        $g.DrawImage($img, $x, $y, $size, $size)
        $g.DrawString("${size}x${size}", $font, $brush, $x, ($y + $size + 6))
        $img.Dispose()
        $x += $size + 40
    }
    # 48 px is roughly how large a listing tile lands in Store search results.
    $img = [Drawing.Image]::FromFile((Join-Path $src 'AppTile-300x300.png'))
    $g.DrawImage($img, $x, $y, 48, 48)
    $g.DrawString('300 drawn at 48', $font, $brush, $x, ($y + 54))
    $img.Dispose()
    $brush.Dispose()
}
$g.Dispose()
$path = Join-Path $PSScriptRoot 'preview\store-images.png'
$canvas.Save($path, [Drawing.Imaging.ImageFormat]::Png)
$canvas.Dispose()
$path
