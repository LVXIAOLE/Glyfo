# Renders what the shell will actually draw, from the shipped files rather than from the vector
# definition: the packaged PNGs and the entries inside Glyfo.ico.
#
#   powershell -ExecutionPolicy Bypass -File tools\Preview-Shipped.ps1
#
# Output: tools\preview\shipped.png

Add-Type -AssemblyName System.Drawing

Add-Type -Namespace Shell -Name Ico -MemberDefinition @'
[DllImport("user32.dll", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode, SetLastError = true)]
public static extern IntPtr LoadImage(IntPtr inst, string name, uint type, int cx, int cy, uint load);
[DllImport("user32.dll")] public static extern bool DestroyIcon(IntPtr h);
'@

$root = Split-Path -Parent $PSScriptRoot
$imageDir = Join-Path $root 'Glyfo (Package)\Images'
$icoPath = Join-Path $root 'Glyfo\Assets\Glyfo.ico'
$outDir = Join-Path $PSScriptRoot 'preview'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

# LR_LOADFROMFILE. Asking for an exact size makes the loader pick that entry rather than scale
# a neighbour, which is what the tray and the taskbar do too.
function Get-IcoBitmap {
    param([int]$Size)
    $handle = [Shell.Ico]::LoadImage([IntPtr]::Zero, $icoPath, 1, $Size, $Size, 0x10)
    if ($handle -eq [IntPtr]::Zero) { throw "no $Size entry in $icoPath" }
    $icon = [System.Drawing.Icon]::FromHandle($handle)
    $bitmap = $icon.ToBitmap()
    [Shell.Ico]::DestroyIcon($handle) | Out-Null
    $bitmap
}

$W = 1180
$H = 640
$bmp = New-Object System.Drawing.Bitmap($W, $H, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
$g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
$g.Clear([System.Drawing.Color]::White)

$fontHead = New-Object System.Drawing.Font('Segoe UI Semibold', 12)
$fontTag = New-Object System.Drawing.Font('Segoe UI', 9)
$ink = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0x20, 0x20, 0x20))
$grey = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0x77, 0x77, 0x77))
$pale = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0x99, 0x99, 0x99))

function Draw-Png {
    param([string]$Name, [int]$X, [int]$Y, [int]$W2, [int]$H2, [string]$Label)
    $img = [System.Drawing.Image]::FromFile((Join-Path $imageDir $Name))
    $g.DrawImage($img, $X, $Y, $W2, $H2)
    $img.Dispose()
    if ($Label) { $g.DrawString($Label, $fontTag, $pale, [single]$X, [single]($Y + $H2 + 4)) }
}

$g.DrawString('Glyfo — shipped assets', $fontHead, $ink, 24, 20)

# --- MSIX tiles, drawn at 100% on the light shell -------------------------------------------
$g.DrawString('Start menu tiles  ·  Images\*.scale-100.png', $fontTag, $grey, 24, 56)
$tiles = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0xF3, 0xF3, 0xF3))
$g.FillRectangle($tiles, 24, 78, 800, 200)
Draw-Png 'Square71x71Logo.scale-100.png' 44 96 71 71 '71 small'
Draw-Png 'Square150x150Logo.scale-100.png' 140 96 150 150 '150 medium'
Draw-Png 'Wide310x150Logo.scale-100.png' 314 96 310 150 '310x150 wide'
Draw-Png 'StoreLogo.scale-100.png' 650 96 50 50 'store 50'
Draw-Png 'Square44x44Logo.scale-100.png' 720 96 44 44 'list 44'

# --- .ico entries at the sizes the tray and taskbar request ----------------------------------
$g.DrawString('Tray / taskbar / Alt+Tab  ·  Assets\Glyfo.ico, exact entries', $fontTag, $grey, 24, 306)

$lightBg = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0xF3, 0xF3, 0xF3))
$darkBg = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0x1F, 0x1F, 0x1F))
$sizes = 16, 20, 24, 32, 40, 48, 64

$rowY = 328
foreach ($panel in @(@(24, $lightBg, $grey, 'light shell'), @(430, $darkBg, $pale, 'dark shell'))) {
    $px = [int]$panel[0]
    $g.FillRectangle($panel[1], $px, $rowY, 380, 120)
    $g.DrawString([string]$panel[3], $fontTag, $panel[2], [single]($px + 12), [single]($rowY + 96))

    $x = $px + 16
    foreach ($s in $sizes) {
        $img = Get-IcoBitmap -Size $s
        $g.DrawImage($img, [int]$x, [int]($rowY + 40 - $s / 2), $s, $s)
        $img.Dispose()
        $g.DrawString([string]$s, $fontTag, $panel[2], [single]($x + $s / 2 - 7), [single]($rowY + 62))
        $x += $s + 22
    }
}

# --- lock screen badge, which is a silhouette rather than the tile ---------------------------
$g.DrawString('Lock screen badge  ·  LockScreenLogo (white silhouette)', $fontTag, $grey, 24, 470)
$g.FillRectangle($darkBg, 24, 492, 380, 100)
Draw-Png 'LockScreenLogo.scale-200.png' 60 512 48 48 ''
Draw-Png 'LockScreenLogo.scale-100.png' 140 526 24 24 ''

$g.DrawString('Splash screen', $fontTag, $grey, 430, 470)
Draw-Png 'SplashScreen.scale-100.png' 430 492 248 120 ''

$g.Dispose()
$path = Join-Path $outDir 'shipped.png'
$bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host "wrote $path"
