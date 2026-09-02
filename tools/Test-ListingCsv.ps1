# Checks a Store-listings CSV for the things a strict importer would reject, so that a failed import
# can be blamed on the file or ruled out as the cause.
#
# Every check here is about *shape*, not wording: field counts, quoting, stray control characters,
# cell lengths, and the payload each language column carries.

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Path
)

$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\ListingCsv.ps1"

$rows = Read-ListingCsv -Path $Path
$header = $rows[0]
$cols = $header.Count
Write-Host "columns: $cols   records: $($rows.Count) (1 header + $($rows.Count - 1) data)"

# 1. Field count. A row with the wrong count means a quote went unbalanced somewhere, which is the
#    single most common way one of these files becomes unimportable.
$bad = 0
for ($r = 0; $r -lt $rows.Count; $r++) {
    if ($rows[$r].Count -ne $cols) { Write-Host "RAGGED: record $r has $($rows[$r].Count) fields" -ForegroundColor Red; $bad++ }
}
if (-not $bad) { Write-Host "field count : every record has $cols fields" -ForegroundColor Green }

# 2. Control characters. Tab, CR and LF are legal inside a quoted cell; nothing else below 0x20 is,
#    and a stray one is invisible in every editor anyone would open this file in.
$ctrl = 0
$zw = @{}
foreach ($row in $rows) {
    foreach ($c in $row) {
        foreach ($ch in $c.ToCharArray()) {
            $n = [int]$ch
            if ($n -lt 0x20 -and $n -ne 9 -and $n -ne 10 -and $n -ne 13) { $ctrl++ }
            # Invisible formatting characters: ZWNJ is ordinary Persian orthography, the bidi marks
            # are not supposed to be here at all. Counted rather than judged.
            if ($n -in 0x200B, 0x200C, 0x200D, 0x200E, 0x200F, 0xFEFF, 0x2060 -or ($n -ge 0x202A -and $n -le 0x202E)) {
                $k = 'U+{0:X4}' -f $n
                $zw[$k] = 1 + $zw[$k]
            }
        }
    }
}
if ($ctrl) { Write-Host "CONTROL CHARS: $ctrl" -ForegroundColor Red } else { Write-Host "control chars: none" -ForegroundColor Green }
if ($zw.Count) { Write-Host "invisible fmt: $(($zw.GetEnumerator() | Sort-Object Name | ForEach-Object { "$($_.Key) x$($_.Value)" }) -join ', ')" }
else { Write-Host "invisible fmt: none" -ForegroundColor Green }

# 3. Lengths against Partner Center's limits.
$Limit = @{ ShortDescription = 1000; Description = 10000; Feature = 200; Caption = 200; SearchTerm = 30 }
$iField = [Array]::IndexOf($header, 'Field')
$over = 0
$worst = @{}
for ($r = 1; $r -lt $rows.Count; $r++) {
    $f = $rows[$r][$iField]
    $lim = $null; $key = $null
    foreach ($k in $Limit.Keys) { if ($f -like "$k*") { $lim = $Limit[$k]; $key = $k } }
    if (-not $lim) { continue }
    for ($i = 0; $i -lt $cols; $i++) {
        $len = $rows[$r][$i].Length
        if ($len -gt $lim) { Write-Host "OVER LIMIT: $f / $($header[$i]) = $len > $lim" -ForegroundColor Red; $over++ }
        if ($len -gt [int]$worst[$key]) { $worst[$key] = $len }
    }
}
if (-not $over) { Write-Host "lengths     : all within limits" -ForegroundColor Green }
foreach ($k in $worst.Keys | Sort-Object) { "  longest {0,-17} {1,6} / {2}" -f $k, $worst[$k], $Limit[$k] }

# 4. What each language column actually asks the importer to do. A file that parses cleanly can
#    still be too much work to apply in one request, and image cells are the expensive ones.
$imageField = '^(DesktopScreenshot|MobileScreenshot|XboxScreenshot|SurfaceHubScreenshot|HoloLensScreenshot|StoreLogo|Trailer|Image)'
Write-Host ""
Write-Host "per-column payload:"
for ($i = 4; $i -lt $cols; $i++) {
    $chars = 0; $lines = 0; $cells = 0; $images = 0
    for ($r = 1; $r -lt $rows.Count; $r++) {
        $v = $rows[$r][$i]
        if (-not $v) { continue }
        $cells++
        $chars += $v.Length
        $lines += ([regex]::Matches($v, "`n")).Count
        if ($rows[$r][$iField] -match $imageField -and $rows[$r][$iField] -notmatch 'Caption') { $images++ }
    }
    "  {0,-8} cells {1,3}   images {2,3}   chars {3,6}   embedded newlines {4,4}" -f $header[$i], $cells, $images, $chars, $lines
}
