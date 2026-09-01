# Renders the "Store display images" Partner Center offers as an override for the logos it would
# otherwise pull out of the package: 1:1 at 300x300, 150x150 and 71x71.
#
#   powershell -ExecutionPolicy Bypass -File tools\Generate-StoreImages.ps1
#
# Writes docs\store-images\*.png. These are listing material, not package assets, so they do not go
# into "Glyfo (Package)\Images" — nothing here is referenced by the manifest.
#
# Drawn from the same vector definition as everything else rather than upscaled from a shipped PNG:
# the 300 px one would otherwise be a four-times blow-up of the 71 px tile.
#
# Full bleed, unlike the packaged tiles, which hold the mark at about half their height so it sits
# in the same rhythm as the first-party Start tiles. The Store lays its own padding around these and
# draws them small, so a mark inset inside the square a second time reads as a shrunken icon. The
# rounded corners are what gives it room, and the corners stay transparent so the same file sits
# correctly on the Store's light and dark grounds.
[CmdletBinding()]
param(
    [ValidateSet('A', 'B', 'C')][string]$Concept = 'B'
)

. "$PSScriptRoot\IconArt.ps1"

$out = Join-Path (Split-Path -Parent $PSScriptRoot) 'docs\store-images'
New-Item -ItemType Directory -Force -Path $out | Out-Null

foreach ($size in 300, 150, 71) {
    $art = Render-Icon -Concept $Concept -Size $size
    $path = Join-Path $out "AppTile-${size}x${size}.png"
    $art.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $art.Dispose()
}

Get-ChildItem $out -Filter *.png | Select-Object Name, Length | Format-Table -AutoSize
"written to $out"
