# The outermost pixel ring of a Windows 11 window is a semi-transparent frame border, so a screen
# capture of DWM's extended frame bounds paints it blended with whatever was behind — the desktop.
# On these shots that shows up as a one-pixel hairline of wallpaper colour down the left and right
# sides and across the top: sampled #2F4A9A, #3F4349, #B7703F against a #E9F9EF window.
#
# Rather than re-run the capture, each of those three lines is overwritten with the line just inside
# it. The corners are left alone: there the rounded clip already blended the ring into the backdrop,
# and squaring them off would be worse than the hairline.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

# Every shot in this batch was framed identically — shoot.ps1 printed "frame 168,70 1584x892" for
# all five — so the window rect is fixed rather than detected.
$L = 168; $T = 70; $W = 1584; $H = 892
$R = $L + $W - 1          # 1751
$B = $T + $H - 1          # 961
$Corner = 14              # skipped at both ends of every edge

$out = Join-Path $PSScriptRoot 'out'
foreach ($file in Get-ChildItem $out -Filter '*.png' | Sort-Object Name) {
    $img = [Drawing.Image]::FromFile($file.FullName)
    $bmp = New-Object Drawing.Bitmap $img
    $img.Dispose()

    for ($x = $L + $Corner; $x -le $R - $Corner; $x++) {
        $bmp.SetPixel($x, $T, $bmp.GetPixel($x, $T + 1))
    }
    for ($y = $T + $Corner; $y -le $B - $Corner; $y++) {
        $bmp.SetPixel($L, $y, $bmp.GetPixel($L + 1, $y))
        $bmp.SetPixel($R, $y, $bmp.GetPixel($R - 1, $y))
    }

    $bmp.Save($file.FullName, [Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    "trimmed  $($file.Name)"
}
