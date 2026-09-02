# Confirms the frame repair took: samples the outermost window pixel on the left, right and top of
# each finished shot and flags anything with a strong red/blue imbalance, which is what desktop
# bleeding through the translucent frame border looked like (#2F4A9A, #B7703F) against the app's
# near-neutral chrome.
Add-Type -AssemblyName System.Drawing
$L = 168; $R = 1751; $T = 70

foreach ($file in Get-ChildItem (Join-Path $PSScriptRoot 'out') -Filter '*.png' | Sort-Object Name) {
    $img = [Drawing.Image]::FromFile($file.FullName)
    $bmp = New-Object Drawing.Bitmap $img
    $img.Dispose()
    $bad = 0
    foreach ($y in 90, 200, 300, 500, 700, 900) {
        foreach ($x in $L, $R) {
            $c = $bmp.GetPixel($x, $y)
            if ([Math]::Abs($c.R - $c.B) -gt 24) { $bad++ }
        }
    }
    foreach ($x in 300, 600, 900, 1200, 1500) {
        $c = $bmp.GetPixel($x, $T)
        if ([Math]::Abs($c.R - $c.B) -gt 24) { $bad++ }
    }
    "{0,-26} {1}x{2}  suspect edge pixels: {3}" -f $file.Name, $bmp.Width, $bmp.Height, $bad
    $bmp.Dispose()
}
